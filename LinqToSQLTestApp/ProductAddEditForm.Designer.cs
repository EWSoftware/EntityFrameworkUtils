namespace LinqToSQLTestApp
{
    partial class ProductAddEditForm
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
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblProductID = new System.Windows.Forms.Label();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.udcUnitPrice = new System.Windows.Forms.NumericUpDown();
            this.udcUnitsInStock = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.txtQtyPerUnit = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.udcUnitsOnOrder = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.udcReorderLevel = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.chkDiscontinued = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.udcUnitPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcUnitsInStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcUnitsOnOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcReorderLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(527, 221);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 32);
            this.btnClose.TabIndex = 21;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(12, 221);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 32);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(106, 221);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(88, 32);
            this.btnDelete.TabIndex = 20;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(143, 39);
            this.txtProductName.MaxLength = 40;
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(316, 22);
            this.txtProductName.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(31, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Product Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(43, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Product ID";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblProductID
            // 
            this.lblProductID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblProductID.Location = new System.Drawing.Point(143, 9);
            this.lblProductID.Name = "lblProductID";
            this.lblProductID.Size = new System.Drawing.Size(56, 23);
            this.lblProductID.TabIndex = 1;
            this.lblProductID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(143, 67);
            this.txtCategoryName.MaxLength = 15;
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(181, 22);
            this.txtCategoryName.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(58, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "&Category";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCompany
            // 
            this.txtCompany.Location = new System.Drawing.Point(143, 95);
            this.txtCompany.MaxLength = 40;
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(316, 22);
            this.txtCompany.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(61, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 23);
            this.label5.TabIndex = 6;
            this.label5.Text = "Co&mpany";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(61, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 23);
            this.label6.TabIndex = 10;
            this.label6.Text = "&Unit Price";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udcUnitPrice
            // 
            this.udcUnitPrice.DecimalPlaces = 2;
            this.udcUnitPrice.Location = new System.Drawing.Point(143, 151);
            this.udcUnitPrice.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udcUnitPrice.Name = "udcUnitPrice";
            this.udcUnitPrice.Size = new System.Drawing.Size(76, 22);
            this.udcUnitPrice.TabIndex = 11;
            this.udcUnitPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // udcUnitsInStock
            // 
            this.udcUnitsInStock.Location = new System.Drawing.Point(329, 151);
            this.udcUnitsInStock.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udcUnitsInStock.Name = "udcUnitsInStock";
            this.udcUnitsInStock.Size = new System.Drawing.Size(76, 22);
            this.udcUnitsInStock.TabIndex = 13;
            this.udcUnitsInStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(227, 150);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 23);
            this.label7.TabIndex = 12;
            this.label7.Text = "Units &In Stock";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtQtyPerUnit
            // 
            this.txtQtyPerUnit.Location = new System.Drawing.Point(143, 123);
            this.txtQtyPerUnit.MaxLength = 20;
            this.txtQtyPerUnit.Name = "txtQtyPerUnit";
            this.txtQtyPerUnit.Size = new System.Drawing.Size(233, 22);
            this.txtQtyPerUnit.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(34, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 23);
            this.label8.TabIndex = 8;
            this.label8.Text = "&Qty Per Unit";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udcUnitsOnOrder
            // 
            this.udcUnitsOnOrder.Location = new System.Drawing.Point(513, 151);
            this.udcUnitsOnOrder.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udcUnitsOnOrder.Name = "udcUnitsOnOrder";
            this.udcUnitsOnOrder.Size = new System.Drawing.Size(76, 22);
            this.udcUnitsOnOrder.TabIndex = 15;
            this.udcUnitsOnOrder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(411, 150);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 23);
            this.label9.TabIndex = 14;
            this.label9.Text = "Units &On Order";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udcReorderLevel
            // 
            this.udcReorderLevel.Location = new System.Drawing.Point(143, 179);
            this.udcReorderLevel.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udcReorderLevel.Name = "udcReorderLevel";
            this.udcReorderLevel.Size = new System.Drawing.Size(76, 22);
            this.udcReorderLevel.TabIndex = 17;
            this.udcReorderLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(41, 178);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 23);
            this.label10.TabIndex = 16;
            this.label10.Text = "&Reorder Level";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkDiscontinued
            // 
            this.chkDiscontinued.AutoSize = true;
            this.chkDiscontinued.Location = new System.Drawing.Point(230, 180);
            this.chkDiscontinued.Name = "chkDiscontinued";
            this.chkDiscontinued.Size = new System.Drawing.Size(107, 20);
            this.chkDiscontinued.TabIndex = 18;
            this.chkDiscontinued.Text = "Discon&tinued";
            this.chkDiscontinued.UseVisualStyleBackColor = true;
            // 
            // ProductAddEditForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(627, 265);
            this.Controls.Add(this.chkDiscontinued);
            this.Controls.Add(this.udcReorderLevel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.udcUnitsOnOrder);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtQtyPerUnit);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.udcUnitsInStock);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.udcUnitPrice);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtCompany);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCategoryName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblProductID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProductName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductAddEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add a Product";
            ((System.ComponentModel.ISupportInitialize)(this.udcUnitPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcUnitsInStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcUnitsOnOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcReorderLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblProductID;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown udcUnitPrice;
        private System.Windows.Forms.NumericUpDown udcUnitsInStock;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtQtyPerUnit;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown udcUnitsOnOrder;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown udcReorderLevel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkDiscontinued;
    }
}