namespace LinqToSQLTestApp
{
    partial class DemoDataTableForm
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
            if(disposing)
            {
                dc?.Dispose();
                components?.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgvDemoData = new System.Windows.Forms.DataGridView();
            this.listKeyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textValueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateValueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.boolValueDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lastModifiedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btncDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bsDemoData = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtXmlValue = new System.Windows.Forms.TextBox();
            this.txtGuidValue = new System.Windows.Forms.TextBox();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDemoData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDemoData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(782, 509);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 32);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(12, 509);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 32);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvDemoData
            // 
            this.dgvDemoData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDemoData.AutoGenerateColumns = false;
            this.dgvDemoData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDemoData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.listKeyDataGridViewTextBoxColumn,
            this.labelDataGridViewTextBoxColumn,
            this.textValueDataGridViewTextBoxColumn,
            this.dateValueDataGridViewTextBoxColumn,
            this.boolValueDataGridViewCheckBoxColumn,
            this.lastModifiedDataGridViewTextBoxColumn,
            this.btncDelete});
            this.dgvDemoData.DataSource = this.bsDemoData;
            this.dgvDemoData.Location = new System.Drawing.Point(12, 12);
            this.dgvDemoData.Name = "dgvDemoData";
            this.dgvDemoData.RowHeadersWidth = 51;
            this.dgvDemoData.RowTemplate.Height = 32;
            this.dgvDemoData.Size = new System.Drawing.Size(858, 341);
            this.dgvDemoData.TabIndex = 0;
            this.dgvDemoData.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDemoData_CellContentClick);
            // 
            // listKeyDataGridViewTextBoxColumn
            // 
            this.listKeyDataGridViewTextBoxColumn.DataPropertyName = "ListKey";
            this.listKeyDataGridViewTextBoxColumn.HeaderText = "ListKey";
            this.listKeyDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.listKeyDataGridViewTextBoxColumn.Name = "listKeyDataGridViewTextBoxColumn";
            this.listKeyDataGridViewTextBoxColumn.ReadOnly = true;
            this.listKeyDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.listKeyDataGridViewTextBoxColumn.Width = 75;
            // 
            // labelDataGridViewTextBoxColumn
            // 
            this.labelDataGridViewTextBoxColumn.DataPropertyName = "Label";
            this.labelDataGridViewTextBoxColumn.HeaderText = "Label";
            this.labelDataGridViewTextBoxColumn.MaxInputLength = 40;
            this.labelDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.labelDataGridViewTextBoxColumn.Name = "labelDataGridViewTextBoxColumn";
            this.labelDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.labelDataGridViewTextBoxColumn.Width = 200;
            // 
            // textValueDataGridViewTextBoxColumn
            // 
            this.textValueDataGridViewTextBoxColumn.DataPropertyName = "TextValue";
            this.textValueDataGridViewTextBoxColumn.HeaderText = "TextValue";
            this.textValueDataGridViewTextBoxColumn.MaxInputLength = 40;
            this.textValueDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.textValueDataGridViewTextBoxColumn.Name = "textValueDataGridViewTextBoxColumn";
            this.textValueDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.textValueDataGridViewTextBoxColumn.Width = 150;
            // 
            // dateValueDataGridViewTextBoxColumn
            // 
            this.dateValueDataGridViewTextBoxColumn.DataPropertyName = "DateValue";
            this.dateValueDataGridViewTextBoxColumn.HeaderText = "DateValue";
            this.dateValueDataGridViewTextBoxColumn.MaxInputLength = 25;
            this.dateValueDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.dateValueDataGridViewTextBoxColumn.Name = "dateValueDataGridViewTextBoxColumn";
            this.dateValueDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dateValueDataGridViewTextBoxColumn.Width = 140;
            // 
            // boolValueDataGridViewCheckBoxColumn
            // 
            this.boolValueDataGridViewCheckBoxColumn.DataPropertyName = "BoolValue";
            this.boolValueDataGridViewCheckBoxColumn.HeaderText = "BoolValue";
            this.boolValueDataGridViewCheckBoxColumn.MinimumWidth = 6;
            this.boolValueDataGridViewCheckBoxColumn.Name = "boolValueDataGridViewCheckBoxColumn";
            this.boolValueDataGridViewCheckBoxColumn.Width = 90;
            // 
            // lastModifiedDataGridViewTextBoxColumn
            // 
            this.lastModifiedDataGridViewTextBoxColumn.DataPropertyName = "LastModified";
            this.lastModifiedDataGridViewTextBoxColumn.HeaderText = "LastModified";
            this.lastModifiedDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.lastModifiedDataGridViewTextBoxColumn.Name = "lastModifiedDataGridViewTextBoxColumn";
            this.lastModifiedDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.lastModifiedDataGridViewTextBoxColumn.Visible = false;
            this.lastModifiedDataGridViewTextBoxColumn.Width = 125;
            // 
            // btncDelete
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.btncDelete.DefaultCellStyle = dataGridViewCellStyle1;
            this.btncDelete.HeaderText = "";
            this.btncDelete.MinimumWidth = 6;
            this.btncDelete.Name = "btncDelete";
            this.btncDelete.Text = "Delete";
            this.btncDelete.ToolTipText = "Delete this code";
            this.btncDelete.UseColumnTextForButtonValue = true;
            this.btncDelete.Width = 80;
            // 
            // bsDemoData
            // 
            this.bsDemoData.DataSource = typeof(LinqToSQLTestApp.Database.DemoTable);
            this.bsDemoData.CurrentChanged += new System.EventHandler(this.bsDemoData_CurrentChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(12, 359);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "XML Value";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(653, 360);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Image Value";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Location = new System.Drawing.Point(12, 464);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "GUID Value";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtXmlValue
            // 
            this.txtXmlValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXmlValue.Location = new System.Drawing.Point(129, 359);
            this.txtXmlValue.Multiline = true;
            this.txtXmlValue.Name = "txtXmlValue";
            this.txtXmlValue.ReadOnly = true;
            this.txtXmlValue.Size = new System.Drawing.Size(519, 100);
            this.txtXmlValue.TabIndex = 2;
            // 
            // txtGuidValue
            // 
            this.txtGuidValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGuidValue.Location = new System.Drawing.Point(129, 465);
            this.txtGuidValue.Name = "txtGuidValue";
            this.txtGuidValue.ReadOnly = true;
            this.txtGuidValue.Size = new System.Drawing.Size(519, 22);
            this.txtGuidValue.TabIndex = 5;
            // 
            // pbImage
            // 
            this.pbImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage.Location = new System.Drawing.Point(770, 359);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(100, 100);
            this.pbImage.TabIndex = 8;
            this.pbImage.TabStop = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(106, 509);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 32);
            this.button1.TabIndex = 7;
            this.button1.Text = "Set &XML";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSetXml_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(200, 509);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 32);
            this.button2.TabIndex = 8;
            this.button2.Text = "Set &GUID";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnSetGuid_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(294, 509);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 32);
            this.button3.TabIndex = 9;
            this.button3.Text = "Set &Image";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnSetImage_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(388, 509);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(120, 32);
            this.button4.TabIndex = 10;
            this.button4.Text = "Clear &Values";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnClearValues_Click);
            // 
            // DemoDataTableForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(882, 553);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.txtGuidValue);
            this.Controls.Add(this.txtXmlValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvDemoData);
            this.Name = "DemoDataTableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Demo Data";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDemoData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDemoData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvDemoData;
        private System.Windows.Forms.BindingSource bsDemoData;
        private System.Windows.Forms.DataGridViewTextBoxColumn listKeyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn labelDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn textValueDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateValueDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn boolValueDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastModifiedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn btncDelete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtXmlValue;
        private System.Windows.Forms.TextBox txtGuidValue;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}