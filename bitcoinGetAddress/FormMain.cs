using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bitcoinGetAddress
{
    public partial class FormMain : Form
    {
        private HashSet<string> existingAddresses;
        private int LbCounter;
        private int LbTotali;
        private double cachedExchangeRate;
        private DateTime lastExchangeRateFetchTime;
        private readonly TimeSpan exchangeRateCacheDuration = TimeSpan.FromMinutes(10);
        private readonly HttpClient httpClient;
        private CancellationTokenSource cts;
        private static readonly string logFilePath = "log.txt";

        public FormMain()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            LoadExistingAddresses();
            LbCounter = 0;
            LbTotali = existingAddresses.Count;
            UpdateLabelTotal();
            cts = new CancellationTokenSource();
            RunContinuousFetch(cts.Token);
        }

        private void LoadExistingAddresses()
        {
            existingAddresses = File.Exists("address.txt")
                ? new HashSet<string>(File.ReadAllLines("address.txt"))
                : new HashSet<string>();
        }

        private async Task FetchAndDisplayTransactionAddressesAsync(string publicKey, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var transactions = await GetWithRetryAsync($"https://blockstream.info/api/address/{publicKey}/txs").ConfigureAwait(false);
                    var transactionsArray = JArray.Parse(transactions);

                    foreach (var transaction in transactionsArray)
                    {
                        await ProcessTransactionAsync(transaction).ConfigureAwait(false);
                    }
                    break; // Exit the loop if successful
                }
                catch (Exception ex)
                {
                    LogMsg($"An error occurred: {ex.Message}");
                    UpdateStatusLabel($"An error occurred: {ex.Message}");
                    publicKey = GetRandomAddress();
                    if (publicKey == null)
                    {
                        LogMsg("No more addresses to try.");
                        UpdateStatusLabel("No more addresses to try.");
                        break;
                    }
                }
            }
        }

        private async Task<string> GetWithRetryAsync(string requestUri)
        {
            int retryCount = 0;
            int maxRetries = 5;
            int delay = 1000;

            while (retryCount < maxRetries)
            {
                try
                {
                    var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        retryCount++;
                        await Task.Delay(delay * retryCount).ConfigureAwait(false);
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    retryCount++;
                    await Task.Delay(delay * retryCount).ConfigureAwait(false);
                }
            }
            throw new HttpRequestException("Too many requests. Please try again later.");
        }

        private async Task ProcessTransactionAsync(JToken transaction)
        {
            foreach (var output in transaction["vout"])
            {
                var address = output["scriptpubkey_address"]?.ToString();
                if (address != null)
                {
                    if (!existingAddresses.Contains(address) && await IsValidAccountWithBalanceAsync(address).ConfigureAwait(false))
                    {
                        var balance = await GetAccountBalanceAsync(address).ConfigureAwait(false);
                        if (balance > 0)
                        {
                            var exchangeRate = await GetBtcToUsdExchangeRateAsync().ConfigureAwait(false);
                            var balanceInUsd = balance * exchangeRate / 100000000; // Satoshi to BTC conversion
                            AddToDataGridView(address, balance, balanceInUsd);
                            existingAddresses.Add(address);
                            await WriteAddressToFileAsync(address).ConfigureAwait(false);
                            LbCounter++;
                            LbTotali++;
                            UpdateLabelCount();
                            UpdateLabelTotal();
                            LogMsg($"Address found and written: {address}");
                            UpdateStatusLabel($"Address found and written: {address}");
                        }
                    }
                }
            }

            string bitcoinAddress = GetRandomAddress();
            if (bitcoinAddress != null)
            {
                await FetchAndDisplayTransactionAddressesAsync(bitcoinAddress, CancellationToken.None).ConfigureAwait(false);
            }
        }

        private string GetRandomAddress()
        {
            var addresses = existingAddresses.ToList();
            if (addresses.Count != 0)
            {
                var random = new Random();
                int index = random.Next(addresses.Count);
                return addresses[index];
            }
            return null;
        }

        private async Task<bool> IsValidAccountWithBalanceAsync(string account)
        {
            var balance = await GetAccountBalanceAsync(account).ConfigureAwait(false);
            return balance > 0;
        }

        private async Task<long> GetAccountBalanceAsync(string account)
        {
            var response = await GetWithRetryAsync($"https://blockstream.info/api/address/{account}").ConfigureAwait(false);
            var accountInfo = JObject.Parse(response);
            var fundedTxoSum = accountInfo["chain_stats"]["funded_txo_sum"].Value<long>();
            var spentTxoSum = accountInfo["chain_stats"]["spent_txo_sum"].Value<long>();
            var balance = fundedTxoSum - spentTxoSum;
            return balance;
        }

        private async Task<double> GetBtcToUsdExchangeRateAsync()
        {
            if ((DateTime.UtcNow - lastExchangeRateFetchTime) < exchangeRateCacheDuration && cachedExchangeRate > 0)
            {
                return cachedExchangeRate;
            }

            var response = await GetWithRetryAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd").ConfigureAwait(false);
            var json = JObject.Parse(response);
            cachedExchangeRate = json["bitcoin"]["usd"].Value<double>();
            lastExchangeRateFetchTime = DateTime.UtcNow;
            return cachedExchangeRate;
        }

        private void AddToDataGridView(string address, long balance, double balanceInUsd)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => dataGridView1.Rows.Add(address, balance, balanceInUsd)));
            }
            else
            {
                dataGridView1.Rows.Add(address, balance, balanceInUsd);
            }
        }

        private static async Task WriteAddressToFileAsync(string address)
        {
            int maxRetries = 10;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    using (var fs = new FileStream("address.txt", FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            await sw.WriteLineAsync(address).ConfigureAwait(false);
                            await sw.FlushAsync().ConfigureAwait(false);
                        }
                    }
                    return; // Exit the method if successful
                }
                catch (IOException ioEx)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        
                        return; // Exit the method if max retries reached
                    }
                    await Task.Delay(500).ConfigureAwait(false); // Wait for half a second before retrying
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        private void UpdateLabelCount()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LbCount.Text = $"Valid addresses found: {LbCounter}"));
            }
            else
            {
                LbCount.Text = $"Valid addresses found: {LbCounter}";
            }
        }

        private void UpdateLabelTotal()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LbTot.Text = $"Total addresses: {LbTotali}"));
            }
            else
            {
                LbTot.Text = $"Total addresses: {LbTotali}";
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string initPath = @"address.txt";
                string bitcoinPublicKey = await LoadBitcoinAddressFromFileAsync(initPath).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(bitcoinPublicKey))
                {
                    await FetchAndDisplayTransactionAddressesAsync(bitcoinPublicKey, cts.Token).ConfigureAwait(false);
                }
                else
                {
                    LogMsg("Bitcoin address not found in file.");
                    UpdateStatusLabel("Bitcoin address not found in file.");
                }
            }
            catch (Exception ex)
            {
                LogMsg($"Error loading form: {ex.Message}");
                UpdateStatusLabel($"Error loading form: {ex.Message}");
            }
        }

        private async Task<string> LoadBitcoinAddressFromFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string bitcoinAddress = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
                    return bitcoinAddress.Trim();
                }
                else
                {
                    return "1111111111111111111114oLvT2";
                }
            }
            catch (Exception ex)
            {
                return "1111111111111111111114oLvT2";
            }
        }

        private async void RunContinuousFetch(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                string bitcoinAddress = GetRandomAddress();
                if (bitcoinAddress != null)
                {
                    await FetchAndDisplayTransactionAddressesAsync(bitcoinAddress, token).ConfigureAwait(false);
                }
                await Task.Delay(TimeSpan.FromMinutes(1), token).ConfigureAwait(false); // Delay between fetches
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            Dispose();
        }

        private void LogMsg(string message)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Handle logging error if necessary
            }
        }

        private void UpdateStatusLabel(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => statusLabel.Text = message));
            }
            else
            {
                statusLabel.Text = message;
            }
        }
    }
}
