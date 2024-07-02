namespace bitcoinGetAddress
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            dataGridView1 = new DataGridView();
            AddressColumn = new DataGridViewTextBoxColumn();
            BalanceColumn = new DataGridViewTextBoxColumn();
            BalanceInUsdColumn = new DataGridViewTextBoxColumn();
            LbCount = new Label();
            LbTot = new Label();
            btnExit = new Button();
            statusLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { AddressColumn, BalanceColumn, BalanceInUsdColumn });
            dataGridView1.Location = new Point(12, 12);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(776, 376);
            dataGridView1.TabIndex = 0;
            // 
            // AddressColumn
            // 
            AddressColumn.HeaderText = "Address";
            AddressColumn.Name = "AddressColumn";
            AddressColumn.ReadOnly = true;
            AddressColumn.Width = 250;
            // 
            // BalanceColumn
            // 
            BalanceColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            BalanceColumn.HeaderText = "Balance (Satoshis)";
            BalanceColumn.Name = "BalanceColumn";
            BalanceColumn.ReadOnly = true;
            // 
            // BalanceInUsdColumn
            // 
            BalanceInUsdColumn.HeaderText = "Balance (USD)";
            BalanceInUsdColumn.Name = "BalanceInUsdColumn";
            BalanceInUsdColumn.ReadOnly = true;
            // 
            // LbCount
            // 
            LbCount.AutoSize = true;
            LbCount.Location = new Point(13, 401);
            LbCount.Name = "LbCount";
            LbCount.Size = new Size(122, 15);
            LbCount.TabIndex = 1;
            LbCount.Text = "Valid address found: 0";
            // 
            // LbTot
            // 
            LbTot.AutoSize = true;
            LbTot.Location = new Point(12, 426);
            LbTot.Name = "LbTot";
            LbTot.Size = new Size(98, 15);
            LbTot.TabIndex = 2;
            LbTot.Text = "Total addresses: 0";
            // 
            // btnExit
            // 
            btnExit.Location = new Point(713, 445);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(75, 23);
            btnExit.TabIndex = 3;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += button1_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(16, 454);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(42, 15);
            statusLabel.TabIndex = 4;
            statusLabel.Text = "Ready!";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 480);
            Controls.Add(statusLabel);
            Controls.Add(btnExit);
            Controls.Add(LbTot);
            Controls.Add(LbCount);
            Controls.Add(dataGridView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Text = "bitcoin-GetAddress";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn AddressColumn;
        private DataGridViewTextBoxColumn BalanceColumn;
        private DataGridViewTextBoxColumn BalanceInUsdColumn;
        private Label LbCount;
        private Label LbTot;
        private Button btnExit;
        private Label statusLabel;
    }
}
