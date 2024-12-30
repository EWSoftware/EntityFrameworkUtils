namespace LinqToSQLTestApp
{
    partial class MainForm
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
            this.btnEditStateCodes = new System.Windows.Forms.Button();
            this.btnEditDemoDataTable = new System.Windows.Forms.Button();
            this.btnProductSearch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEditStateCodes
            // 
            this.btnEditStateCodes.Location = new System.Drawing.Point(80, 12);
            this.btnEditStateCodes.Name = "btnEditStateCodes";
            this.btnEditStateCodes.Size = new System.Drawing.Size(262, 32);
            this.btnEditStateCodes.TabIndex = 0;
            this.btnEditStateCodes.Text = "Edit &State Codes";
            this.btnEditStateCodes.UseVisualStyleBackColor = true;
            this.btnEditStateCodes.Click += new System.EventHandler(this.btnEditStateCodes_Click);
            // 
            // btnEditDemoDataTable
            // 
            this.btnEditDemoDataTable.Location = new System.Drawing.Point(80, 50);
            this.btnEditDemoDataTable.Name = "btnEditDemoDataTable";
            this.btnEditDemoDataTable.Size = new System.Drawing.Size(262, 32);
            this.btnEditDemoDataTable.TabIndex = 1;
            this.btnEditDemoDataTable.Text = "Edit &Demo Data Table";
            this.btnEditDemoDataTable.UseVisualStyleBackColor = true;
            this.btnEditDemoDataTable.Click += new System.EventHandler(this.btnEditDemoDataTable_Click);
            // 
            // btnProductSearch
            // 
            this.btnProductSearch.Location = new System.Drawing.Point(80, 88);
            this.btnProductSearch.Name = "btnProductSearch";
            this.btnProductSearch.Size = new System.Drawing.Size(262, 32);
            this.btnProductSearch.TabIndex = 2;
            this.btnProductSearch.Text = "&Product Search";
            this.btnProductSearch.UseVisualStyleBackColor = true;
            this.btnProductSearch.Click += new System.EventHandler(this.btnProductSearch_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(422, 138);
            this.Controls.Add(this.btnProductSearch);
            this.Controls.Add(this.btnEditDemoDataTable);
            this.Controls.Add(this.btnEditStateCodes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LINQ to SQL Test Application";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEditStateCodes;
        private System.Windows.Forms.Button btnEditDemoDataTable;
        private System.Windows.Forms.Button btnProductSearch;
    }
}

