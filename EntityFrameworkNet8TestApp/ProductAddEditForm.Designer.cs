namespace EntityFrameworkNet8TestApp
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
            btnClose = new Button();
            btnSave = new Button();
            btnDelete = new Button();
            txtProductName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            lblProductID = new Label();
            txtCategoryName = new TextBox();
            label4 = new Label();
            txtCompany = new TextBox();
            label5 = new Label();
            label6 = new Label();
            udcUnitPrice = new NumericUpDown();
            udcUnitsInStock = new NumericUpDown();
            label7 = new Label();
            txtQtyPerUnit = new TextBox();
            label8 = new Label();
            udcUnitsOnOrder = new NumericUpDown();
            label9 = new Label();
            udcReorderLevel = new NumericUpDown();
            label10 = new Label();
            chkDiscontinued = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)udcUnitPrice).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udcUnitsInStock).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udcUnitsOnOrder).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udcReorderLevel).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(541, 251);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 32);
            btnClose.TabIndex = 21;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += this.btnClose_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.Location = new Point(12, 251);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 32);
            btnSave.TabIndex = 19;
            btnSave.Text = "&Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += this.btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDelete.Location = new Point(106, 251);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(88, 32);
            btnDelete.TabIndex = 20;
            btnDelete.Text = "&Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += this.btnDelete_Click;
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(143, 39);
            txtProductName.MaxLength = 40;
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(316, 27);
            txtProductName.TabIndex = 3;
            // 
            // label1
            // 
            label1.Location = new Point(31, 39);
            label1.Name = "label1";
            label1.Size = new Size(106, 23);
            label1.TabIndex = 2;
            label1.Text = "&Product Name";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.Location = new Point(43, 9);
            label2.Name = "label2";
            label2.Size = new Size(94, 23);
            label2.TabIndex = 0;
            label2.Text = "Product ID";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblProductID
            // 
            lblProductID.BorderStyle = BorderStyle.Fixed3D;
            lblProductID.Location = new Point(143, 9);
            lblProductID.Name = "lblProductID";
            lblProductID.Size = new Size(56, 23);
            lblProductID.TabIndex = 1;
            lblProductID.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtCategoryName
            // 
            txtCategoryName.Location = new Point(143, 72);
            txtCategoryName.MaxLength = 15;
            txtCategoryName.Name = "txtCategoryName";
            txtCategoryName.Size = new Size(181, 27);
            txtCategoryName.TabIndex = 5;
            // 
            // label4
            // 
            label4.Location = new Point(58, 72);
            label4.Name = "label4";
            label4.Size = new Size(79, 23);
            label4.TabIndex = 4;
            label4.Text = "&Category";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtCompany
            // 
            txtCompany.Location = new Point(143, 105);
            txtCompany.MaxLength = 40;
            txtCompany.Name = "txtCompany";
            txtCompany.Size = new Size(316, 27);
            txtCompany.TabIndex = 7;
            // 
            // label5
            // 
            label5.Location = new Point(61, 105);
            label5.Name = "label5";
            label5.Size = new Size(76, 23);
            label5.TabIndex = 6;
            label5.Text = "Co&mpany";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            label6.Location = new Point(43, 172);
            label6.Name = "label6";
            label6.Size = new Size(94, 23);
            label6.TabIndex = 10;
            label6.Text = "&Unit Price";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // udcUnitPrice
            // 
            udcUnitPrice.DecimalPlaces = 2;
            udcUnitPrice.Location = new Point(143, 171);
            udcUnitPrice.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            udcUnitPrice.Name = "udcUnitPrice";
            udcUnitPrice.Size = new Size(76, 27);
            udcUnitPrice.TabIndex = 11;
            udcUnitPrice.TextAlign = HorizontalAlignment.Right;
            // 
            // udcUnitsInStock
            // 
            udcUnitsInStock.Location = new Point(340, 171);
            udcUnitsInStock.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            udcUnitsInStock.Name = "udcUnitsInStock";
            udcUnitsInStock.Size = new Size(76, 27);
            udcUnitsInStock.TabIndex = 13;
            udcUnitsInStock.TextAlign = HorizontalAlignment.Right;
            // 
            // label7
            // 
            label7.Location = new Point(225, 172);
            label7.Name = "label7";
            label7.Size = new Size(109, 23);
            label7.TabIndex = 12;
            label7.Text = "Units &In Stock";
            label7.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtQtyPerUnit
            // 
            txtQtyPerUnit.Location = new Point(143, 138);
            txtQtyPerUnit.MaxLength = 20;
            txtQtyPerUnit.Name = "txtQtyPerUnit";
            txtQtyPerUnit.Size = new Size(233, 27);
            txtQtyPerUnit.TabIndex = 9;
            // 
            // label8
            // 
            label8.Location = new Point(34, 138);
            label8.Name = "label8";
            label8.Size = new Size(103, 23);
            label8.TabIndex = 8;
            label8.Text = "&Qty Per Unit";
            label8.TextAlign = ContentAlignment.MiddleRight;
            // 
            // udcUnitsOnOrder
            // 
            udcUnitsOnOrder.Location = new Point(539, 171);
            udcUnitsOnOrder.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            udcUnitsOnOrder.Name = "udcUnitsOnOrder";
            udcUnitsOnOrder.Size = new Size(76, 27);
            udcUnitsOnOrder.TabIndex = 15;
            udcUnitsOnOrder.TextAlign = HorizontalAlignment.Right;
            // 
            // label9
            // 
            label9.Location = new Point(422, 172);
            label9.Name = "label9";
            label9.Size = new Size(111, 23);
            label9.TabIndex = 14;
            label9.Text = "Units &On Order";
            label9.TextAlign = ContentAlignment.MiddleRight;
            // 
            // udcReorderLevel
            // 
            udcReorderLevel.Location = new Point(143, 204);
            udcReorderLevel.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            udcReorderLevel.Name = "udcReorderLevel";
            udcReorderLevel.Size = new Size(76, 27);
            udcReorderLevel.TabIndex = 17;
            udcReorderLevel.TextAlign = HorizontalAlignment.Right;
            // 
            // label10
            // 
            label10.Location = new Point(12, 205);
            label10.Name = "label10";
            label10.Size = new Size(125, 23);
            label10.TabIndex = 16;
            label10.Text = "&Reorder Level";
            label10.TextAlign = ContentAlignment.MiddleRight;
            // 
            // chkDiscontinued
            // 
            chkDiscontinued.AutoSize = true;
            chkDiscontinued.Location = new Point(242, 205);
            chkDiscontinued.Name = "chkDiscontinued";
            chkDiscontinued.Size = new Size(118, 24);
            chkDiscontinued.TabIndex = 18;
            chkDiscontinued.Text = "Discon&tinued";
            chkDiscontinued.UseVisualStyleBackColor = true;
            // 
            // ProductAddEditForm
            // 
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.CancelButton = btnClose;
            this.ClientSize = new Size(641, 295);
            this.Controls.Add(chkDiscontinued);
            this.Controls.Add(udcReorderLevel);
            this.Controls.Add(label10);
            this.Controls.Add(udcUnitsOnOrder);
            this.Controls.Add(label9);
            this.Controls.Add(txtQtyPerUnit);
            this.Controls.Add(label8);
            this.Controls.Add(udcUnitsInStock);
            this.Controls.Add(label7);
            this.Controls.Add(udcUnitPrice);
            this.Controls.Add(label6);
            this.Controls.Add(txtCompany);
            this.Controls.Add(label5);
            this.Controls.Add(txtCategoryName);
            this.Controls.Add(label4);
            this.Controls.Add(lblProductID);
            this.Controls.Add(label2);
            this.Controls.Add(txtProductName);
            this.Controls.Add(label1);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnSave);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductAddEditForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add a Product";
            ((System.ComponentModel.ISupportInitialize)udcUnitPrice).EndInit();
            ((System.ComponentModel.ISupportInitialize)udcUnitsInStock).EndInit();
            ((System.ComponentModel.ISupportInitialize)udcUnitsOnOrder).EndInit();
            ((System.ComponentModel.ISupportInitialize)udcReorderLevel).EndInit();
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