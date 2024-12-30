using System.Drawing;
using System.Windows.Forms;

namespace EntityFrameworkNet48TestApp
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
            btnEditStateCodes = new Button();
            btnEditDemoDataTable = new Button();
            btnProductSearch = new Button();
            this.SuspendLayout();
            // 
            // btnEditStateCodes
            // 
            btnEditStateCodes.Location = new Point(80, 12);
            btnEditStateCodes.Name = "btnEditStateCodes";
            btnEditStateCodes.Size = new Size(262, 32);
            btnEditStateCodes.TabIndex = 0;
            btnEditStateCodes.Text = "Edit &State Codes";
            btnEditStateCodes.UseVisualStyleBackColor = true;
            btnEditStateCodes.Click += this.btnEditStateCodes_Click;
            // 
            // btnEditDemoDataTable
            // 
            btnEditDemoDataTable.Location = new Point(80, 50);
            btnEditDemoDataTable.Name = "btnEditDemoDataTable";
            btnEditDemoDataTable.Size = new Size(262, 32);
            btnEditDemoDataTable.TabIndex = 1;
            btnEditDemoDataTable.Text = "Edit &Demo Data Table";
            btnEditDemoDataTable.UseVisualStyleBackColor = true;
            btnEditDemoDataTable.Click += this.btnEditDemoDataTable_Click;
            // 
            // btnProductSearch
            // 
            btnProductSearch.Location = new Point(80, 88);
            btnProductSearch.Name = "btnProductSearch";
            btnProductSearch.Size = new Size(262, 32);
            btnProductSearch.TabIndex = 2;
            btnProductSearch.Text = "&Product Search";
            btnProductSearch.UseVisualStyleBackColor = true;
            btnProductSearch.Click += this.btnProductSearch_Click;
            // 
            // MainForm
            // 
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.ClientSize = new Size(422, 138);
            this.Controls.Add(btnProductSearch);
            this.Controls.Add(btnEditDemoDataTable);
            this.Controls.Add(btnEditStateCodes);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Entity Framework .NET 8 or Later Test Application";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnEditStateCodes;
        private System.Windows.Forms.Button btnEditDemoDataTable;
        private Button btnProductSearch;
    }
}

