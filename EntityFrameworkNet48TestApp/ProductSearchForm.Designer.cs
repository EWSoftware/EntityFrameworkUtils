using System.Drawing;
using System.Windows.Forms;

namespace EntityFrameworkNet48TestApp
{
    partial class ProductSearchForm
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            label1 = new Label();
            txtProductName = new TextBox();
            label2 = new Label();
            cboCategoryName = new ComboBox();
            cboCompanyName = new ComboBox();
            label3 = new Label();
            dgvDemoData = new DataGridView();
            btncEdit = new DataGridViewButtonColumn();
            productIDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            productNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            categoryNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            companyNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            unitPriceDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            discontinuedDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            bsProducts = new BindingSource(components);
            btnClose = new Button();
            btnSearch = new Button();
            btnAdd = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvDemoData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsProducts).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(14, 14);
            label1.Name = "label1";
            label1.Size = new Size(163, 23);
            label1.TabIndex = 0;
            label1.Text = "&Product Name Contains";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(183, 12);
            txtProductName.MaxLength = 40;
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(316, 27);
            txtProductName.TabIndex = 1;
            // 
            // label2
            // 
            label2.Location = new Point(87, 47);
            label2.Name = "label2";
            label2.Size = new Size(90, 23);
            label2.TabIndex = 2;
            label2.Text = "&Category";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cboCategoryName
            // 
            cboCategoryName.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategoryName.FormattingEnabled = true;
            cboCategoryName.Location = new Point(183, 45);
            cboCategoryName.Name = "cboCategoryName";
            cboCategoryName.Size = new Size(316, 28);
            cboCategoryName.TabIndex = 3;
            // 
            // cboCompanyName
            // 
            cboCompanyName.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCompanyName.FormattingEnabled = true;
            cboCompanyName.Location = new Point(636, 45);
            cboCompanyName.Name = "cboCompanyName";
            cboCompanyName.Size = new Size(316, 28);
            cboCompanyName.TabIndex = 5;
            // 
            // label3
            // 
            label3.Location = new Point(505, 47);
            label3.Name = "label3";
            label3.Size = new Size(125, 23);
            label3.TabIndex = 4;
            label3.Text = "Company &Name";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // dgvDemoData
            // 
            dgvDemoData.AllowUserToAddRows = false;
            dgvDemoData.AllowUserToDeleteRows = false;
            dgvDemoData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDemoData.AutoGenerateColumns = false;
            dgvDemoData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDemoData.Columns.AddRange(new DataGridViewColumn[] { btncEdit, productIDDataGridViewTextBoxColumn, productNameDataGridViewTextBoxColumn, categoryNameDataGridViewTextBoxColumn, companyNameDataGridViewTextBoxColumn, unitPriceDataGridViewTextBoxColumn, discontinuedDataGridViewCheckBoxColumn });
            dgvDemoData.DataSource = bsProducts;
            dgvDemoData.Location = new Point(12, 79);
            dgvDemoData.Name = "dgvDemoData";
            dgvDemoData.ReadOnly = true;
            dgvDemoData.RowHeadersWidth = 25;
            dgvDemoData.RowTemplate.Height = 32;
            dgvDemoData.Size = new Size(958, 424);
            dgvDemoData.TabIndex = 6;
            dgvDemoData.CellContentClick += this.dgvDemoData_CellContentClick;
            // 
            // btncEdit
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new Padding(2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            btncEdit.DefaultCellStyle = dataGridViewCellStyle1;
            btncEdit.HeaderText = "";
            btncEdit.MinimumWidth = 6;
            btncEdit.Name = "btncEdit";
            btncEdit.ReadOnly = true;
            btncEdit.Text = "Edit";
            btncEdit.ToolTipText = "Edit product details";
            btncEdit.UseColumnTextForButtonValue = true;
            btncEdit.Width = 80;
            // 
            // productIDDataGridViewTextBoxColumn
            // 
            productIDDataGridViewTextBoxColumn.DataPropertyName = "ProductID";
            productIDDataGridViewTextBoxColumn.HeaderText = "ProductID";
            productIDDataGridViewTextBoxColumn.MinimumWidth = 6;
            productIDDataGridViewTextBoxColumn.Name = "productIDDataGridViewTextBoxColumn";
            productIDDataGridViewTextBoxColumn.ReadOnly = true;
            productIDDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            productIDDataGridViewTextBoxColumn.Visible = false;
            productIDDataGridViewTextBoxColumn.Width = 125;
            // 
            // productNameDataGridViewTextBoxColumn
            // 
            productNameDataGridViewTextBoxColumn.DataPropertyName = "ProductName";
            productNameDataGridViewTextBoxColumn.HeaderText = "Product Name";
            productNameDataGridViewTextBoxColumn.MinimumWidth = 6;
            productNameDataGridViewTextBoxColumn.Name = "productNameDataGridViewTextBoxColumn";
            productNameDataGridViewTextBoxColumn.ReadOnly = true;
            productNameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            productNameDataGridViewTextBoxColumn.Width = 200;
            // 
            // categoryNameDataGridViewTextBoxColumn
            // 
            categoryNameDataGridViewTextBoxColumn.DataPropertyName = "CategoryName";
            categoryNameDataGridViewTextBoxColumn.HeaderText = "Category";
            categoryNameDataGridViewTextBoxColumn.MinimumWidth = 6;
            categoryNameDataGridViewTextBoxColumn.Name = "categoryNameDataGridViewTextBoxColumn";
            categoryNameDataGridViewTextBoxColumn.ReadOnly = true;
            categoryNameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            categoryNameDataGridViewTextBoxColumn.Width = 150;
            // 
            // companyNameDataGridViewTextBoxColumn
            // 
            companyNameDataGridViewTextBoxColumn.DataPropertyName = "CompanyName";
            companyNameDataGridViewTextBoxColumn.HeaderText = "Company";
            companyNameDataGridViewTextBoxColumn.MinimumWidth = 6;
            companyNameDataGridViewTextBoxColumn.Name = "companyNameDataGridViewTextBoxColumn";
            companyNameDataGridViewTextBoxColumn.ReadOnly = true;
            companyNameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            companyNameDataGridViewTextBoxColumn.Width = 250;
            // 
            // unitPriceDataGridViewTextBoxColumn
            // 
            unitPriceDataGridViewTextBoxColumn.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            unitPriceDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            unitPriceDataGridViewTextBoxColumn.HeaderText = "Unit Price";
            unitPriceDataGridViewTextBoxColumn.MinimumWidth = 6;
            unitPriceDataGridViewTextBoxColumn.Name = "unitPriceDataGridViewTextBoxColumn";
            unitPriceDataGridViewTextBoxColumn.ReadOnly = true;
            unitPriceDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            unitPriceDataGridViewTextBoxColumn.Width = 80;
            // 
            // discontinuedDataGridViewCheckBoxColumn
            // 
            discontinuedDataGridViewCheckBoxColumn.DataPropertyName = "Discontinued";
            discontinuedDataGridViewCheckBoxColumn.HeaderText = "Discontinued";
            discontinuedDataGridViewCheckBoxColumn.MinimumWidth = 6;
            discontinuedDataGridViewCheckBoxColumn.Name = "discontinuedDataGridViewCheckBoxColumn";
            discontinuedDataGridViewCheckBoxColumn.ReadOnly = true;
            discontinuedDataGridViewCheckBoxColumn.Width = 110;
            // 
            // bsProducts
            // 
            bsProducts.DataSource = typeof(Database.spProductSearchResult);
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(882, 509);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 32);
            btnClose.TabIndex = 9;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += this.btnClose_Click;
            // 
            // btnSearch
            // 
            btnSearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSearch.Location = new Point(12, 509);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(88, 32);
            btnSearch.TabIndex = 7;
            btnSearch.Text = "&Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += this.ProductSearchForm_Activated;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAdd.Location = new Point(106, 509);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(88, 32);
            btnAdd.TabIndex = 8;
            btnAdd.Text = "&Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += this.btnAdd_Click;
            // 
            // ProductSearchForm
            // 
            this.AcceptButton = btnSearch;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.CancelButton = btnClose;
            this.ClientSize = new Size(982, 553);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dgvDemoData);
            this.Controls.Add(cboCompanyName);
            this.Controls.Add(label3);
            this.Controls.Add(cboCategoryName);
            this.Controls.Add(label2);
            this.Controls.Add(txtProductName);
            this.Controls.Add(label1);
            this.Name = "ProductSearchForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Search for Products";
            this.Activated += this.ProductSearchForm_Activated;
            ((System.ComponentModel.ISupportInitialize)dgvDemoData).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsProducts).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboCategoryName;
        private System.Windows.Forms.ComboBox cboCompanyName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvDemoData;
        private System.Windows.Forms.BindingSource bsProducts;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnAdd;
        private DataGridViewButtonColumn btncEdit;
        private DataGridViewTextBoxColumn productIDDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn productNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn categoryNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn companyNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn unitPriceDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn discontinuedDataGridViewCheckBoxColumn;
    }
}