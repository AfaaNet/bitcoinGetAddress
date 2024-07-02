# Bitcoin Address Fetcher

This project is a Windows Forms application that fetches and displays Bitcoin transaction addresses, processes transactions, and logs activities. The application is designed to run continuously, 24/7.

## Features

- Fetch and display Bitcoin transaction addresses.
- Process transactions and display address balances in BTC and USD.
- Retry mechanism for handling network requests.
- Continuous operation with periodic address fetching.
- Logging of activities and errors to a file.
- Display of status messages in the UI.

## Getting Started

### Prerequisites

- .NET Framework (version 4.7.2 or later)
- Visual Studio 2019 or later

### Installation

1. Clone the repository:

    ```sh
    git clone https://github.com/AfaaNet/bitcoin-address-fetcher.git
    cd bitcoin-address-fetcher
    ```

2. Open the solution file `BitcoinAddressFetcher.sln` in Visual Studio.

3. Build the solution to restore the NuGet packages.

### Usage

1. Run the application from Visual Studio.

2. The application will start fetching and displaying Bitcoin transaction addresses.

3. Logs will be written to `log.txt` in the application's directory.

4. The status messages will be displayed in the UI.

## Configuration

- You can configure the address file by modifying the `address.txt` file in the application's directory. The file should contain one Bitcoin address per line.

- The exchange rate is cached for 10 minutes to reduce the number of API requests. This can be adjusted by modifying the `exchangeRateCacheDuration` in the `FormMain` class.

## Logging

- All activities and errors are logged to `log.txt`.

- Logs include timestamps and detailed error messages.

## Contributing

1. Fork the repository.

2. Create a new branch:

    ```sh
    git checkout -b feature/your-feature-name
    ```

3. Make your changes and commit them:

    ```sh
    git commit -m "Add your commit message"
    ```

4. Push to the branch:

    ```sh
    git push origin feature/your-feature-name
    ```

5. Open a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any questions or issues, please open an issue on GitHub.

