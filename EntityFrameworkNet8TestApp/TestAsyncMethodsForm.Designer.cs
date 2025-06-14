namespace EntityFrameworkNet8TestApp
{
    partial class TestAsyncMethodsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbRows = new ListBox();
            btnLoadAll = new Button();
            btnClose = new Button();
            btnLoadByKey = new Button();
            btnClearResults = new Button();
            btnCRUDMethods = new Button();
            btnExecuteMethodQueryAsync = new Button();
            this.SuspendLayout();
            // 
            // lbRows
            // 
            lbRows.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbRows.FormattingEnabled = true;
            lbRows.IntegralHeight = false;
            lbRows.Location = new Point(12, 12);
            lbRows.Name = "lbRows";
            lbRows.Size = new Size(958, 388);
            lbRows.TabIndex = 0;
            // 
            // btnLoadAll
            // 
            btnLoadAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoadAll.Location = new Point(138, 406);
            btnLoadAll.Name = "btnLoadAll";
            btnLoadAll.Size = new Size(120, 32);
            btnLoadAll.TabIndex = 2;
            btnLoadAll.Text = "Load &All";
            btnLoadAll.UseVisualStyleBackColor = true;
            btnLoadAll.Click += this.btnLoadAll_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(882, 406);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 32);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += this.btnClose_Click;
            // 
            // btnLoadByKey
            // 
            btnLoadByKey.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoadByKey.Location = new Point(264, 406);
            btnLoadByKey.Name = "btnLoadByKey";
            btnLoadByKey.Size = new Size(120, 32);
            btnLoadByKey.TabIndex = 3;
            btnLoadByKey.Text = "Load By &Key";
            btnLoadByKey.UseVisualStyleBackColor = true;
            btnLoadByKey.Click += this.btnLoadByKey_Click;
            // 
            // btnClearResults
            // 
            btnClearResults.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClearResults.Location = new Point(12, 406);
            btnClearResults.Name = "btnClearResults";
            btnClearResults.Size = new Size(120, 32);
            btnClearResults.TabIndex = 1;
            btnClearResults.Text = "&Clear Results";
            btnClearResults.UseVisualStyleBackColor = true;
            btnClearResults.Click += this.btnClearResults_Click;
            // 
            // btnCRUDMethods
            // 
            btnCRUDMethods.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCRUDMethods.Location = new Point(390, 406);
            btnCRUDMethods.Name = "btnCRUDMethods";
            btnCRUDMethods.Size = new Size(140, 32);
            btnCRUDMethods.TabIndex = 4;
            btnCRUDMethods.Text = "C&RUD Methods";
            btnCRUDMethods.UseVisualStyleBackColor = true;
            btnCRUDMethods.Click += this.btnCRUDMethods_Click;
            // 
            // btnExecuteMethodQueryAsync
            // 
            btnExecuteMethodQueryAsync.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExecuteMethodQueryAsync.Location = new Point(536, 406);
            btnExecuteMethodQueryAsync.Name = "btnExecuteMethodQueryAsync";
            btnExecuteMethodQueryAsync.Size = new Size(180, 32);
            btnExecuteMethodQueryAsync.TabIndex = 5;
            btnExecuteMethodQueryAsync.Text = "&Execute Method Query";
            btnExecuteMethodQueryAsync.UseVisualStyleBackColor = true;
            btnExecuteMethodQueryAsync.Click += this.btnExecuteMethodQueryAsync_Click;
            // 
            // TestAsyncMethodsForm
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = btnClose;
            this.ClientSize = new Size(982, 450);
            this.Controls.Add(btnExecuteMethodQueryAsync);
            this.Controls.Add(btnCRUDMethods);
            this.Controls.Add(btnClearResults);
            this.Controls.Add(btnLoadByKey);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnLoadAll);
            this.Controls.Add(lbRows);
            this.Name = "TestAsyncMethodsForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Test Async Methods Form";
            this.ResumeLayout(false);
        }

        #endregion

        private ListBox lbRows;
        private Button btnLoadAll;
        private Button btnClose;
        private Button btnLoadByKey;
        private Button btnClearResults;
        private Button btnCRUDMethods;
        private Button btnExecuteMethodQueryAsync;
    }
}