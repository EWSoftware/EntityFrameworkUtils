namespace EntityFrameworkNet8TestApp
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            btnClose = new Button();
            btnSave = new Button();
            dgvDemoData = new DataGridView();
            bsDemoData = new BindingSource(components);
            btnSetXml = new Button();
            btnSetGuid = new Button();
            pbImage = new PictureBox();
            btnSetImage = new Button();
            label1 = new Label();
            txtXmlValue = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtGuidValue = new TextBox();
            btnClearValues = new Button();
            btnSaveAsync = new Button();
            listKeyDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            labelDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            textValueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dateValueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            boolValueDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            DecimalValue = new DataGridViewTextBoxColumn();
            lastModifiedDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            btncDelete = new DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)dgvDemoData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDemoData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbImage).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(882, 509);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 32);
            btnClose.TabIndex = 12;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += this.btnClose_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.Location = new Point(12, 509);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 32);
            btnSave.TabIndex = 6;
            btnSave.Text = "&Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += this.btnSave_Click;
            // 
            // dgvDemoData
            // 
            dgvDemoData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDemoData.AutoGenerateColumns = false;
            dgvDemoData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDemoData.Columns.AddRange(new DataGridViewColumn[] { listKeyDataGridViewTextBoxColumn, labelDataGridViewTextBoxColumn, textValueDataGridViewTextBoxColumn, dateValueDataGridViewTextBoxColumn, boolValueDataGridViewCheckBoxColumn, DecimalValue, lastModifiedDataGridViewTextBoxColumn, btncDelete });
            dgvDemoData.DataSource = bsDemoData;
            dgvDemoData.Location = new Point(12, 12);
            dgvDemoData.Name = "dgvDemoData";
            dgvDemoData.RowHeadersWidth = 51;
            dgvDemoData.RowTemplate.Height = 32;
            dgvDemoData.Size = new Size(958, 341);
            dgvDemoData.TabIndex = 0;
            dgvDemoData.CellContentClick += this.dgvDemoData_CellContentClick;
            // 
            // bsDemoData
            // 
            bsDemoData.DataSource = typeof(Database.DemoTable);
            bsDemoData.CurrentChanged += this.bsDemoData_CurrentChanged;
            // 
            // btnSetXml
            // 
            btnSetXml.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSetXml.Location = new Point(305, 509);
            btnSetXml.Name = "btnSetXml";
            btnSetXml.Size = new Size(88, 32);
            btnSetXml.TabIndex = 8;
            btnSetXml.Text = "Set &XML";
            btnSetXml.UseVisualStyleBackColor = true;
            btnSetXml.Click += this.btnSetXml_Click;
            // 
            // btnSetGuid
            // 
            btnSetGuid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSetGuid.Location = new Point(399, 509);
            btnSetGuid.Name = "btnSetGuid";
            btnSetGuid.Size = new Size(88, 32);
            btnSetGuid.TabIndex = 9;
            btnSetGuid.Text = "Set &GUID";
            btnSetGuid.UseVisualStyleBackColor = true;
            btnSetGuid.Click += this.btnSetGuid_Click;
            // 
            // pbImage
            // 
            pbImage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pbImage.BorderStyle = BorderStyle.FixedSingle;
            pbImage.Location = new Point(870, 359);
            pbImage.Name = "pbImage";
            pbImage.Size = new Size(100, 100);
            pbImage.TabIndex = 7;
            pbImage.TabStop = false;
            // 
            // btnSetImage
            // 
            btnSetImage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSetImage.Location = new Point(493, 509);
            btnSetImage.Name = "btnSetImage";
            btnSetImage.Size = new Size(88, 32);
            btnSetImage.TabIndex = 10;
            btnSetImage.Text = "Set &Image";
            btnSetImage.UseVisualStyleBackColor = true;
            btnSetImage.Click += this.btnSetImage_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.Location = new Point(12, 359);
            label1.Name = "label1";
            label1.Size = new Size(111, 25);
            label1.TabIndex = 1;
            label1.Text = "XML Value";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtXmlValue
            // 
            txtXmlValue.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtXmlValue.Location = new Point(129, 359);
            txtXmlValue.Multiline = true;
            txtXmlValue.Name = "txtXmlValue";
            txtXmlValue.ReadOnly = true;
            txtXmlValue.ScrollBars = ScrollBars.Both;
            txtXmlValue.Size = new Size(619, 100);
            txtXmlValue.TabIndex = 2;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label2.Location = new Point(753, 360);
            label2.Name = "label2";
            label2.Size = new Size(111, 25);
            label2.TabIndex = 3;
            label2.Text = "Image Value";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.Location = new Point(12, 466);
            label3.Name = "label3";
            label3.Size = new Size(111, 25);
            label3.TabIndex = 4;
            label3.Text = "GUID Value";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtGuidValue
            // 
            txtGuidValue.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtGuidValue.Location = new Point(129, 465);
            txtGuidValue.Name = "txtGuidValue";
            txtGuidValue.ReadOnly = true;
            txtGuidValue.Size = new Size(619, 27);
            txtGuidValue.TabIndex = 5;
            // 
            // btnClearValues
            // 
            btnClearValues.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClearValues.Location = new Point(587, 509);
            btnClearValues.Name = "btnClearValues";
            btnClearValues.Size = new Size(120, 32);
            btnClearValues.TabIndex = 11;
            btnClearValues.Text = "Clear &Values";
            btnClearValues.UseVisualStyleBackColor = true;
            btnClearValues.Click += this.btnClearValues_Click;
            // 
            // btnSaveAsync
            // 
            btnSaveAsync.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSaveAsync.Location = new Point(106, 509);
            btnSaveAsync.Name = "btnSaveAsync";
            btnSaveAsync.Size = new Size(120, 32);
            btnSaveAsync.TabIndex = 7;
            btnSaveAsync.Text = "Save &Async";
            btnSaveAsync.UseVisualStyleBackColor = true;
            btnSaveAsync.Click += this.btnSaveAsync_Click;
            // 
            // listKeyDataGridViewTextBoxColumn
            // 
            listKeyDataGridViewTextBoxColumn.DataPropertyName = "ListKey";
            listKeyDataGridViewTextBoxColumn.HeaderText = "ListKey";
            listKeyDataGridViewTextBoxColumn.MinimumWidth = 6;
            listKeyDataGridViewTextBoxColumn.Name = "listKeyDataGridViewTextBoxColumn";
            listKeyDataGridViewTextBoxColumn.ReadOnly = true;
            listKeyDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            listKeyDataGridViewTextBoxColumn.Width = 75;
            // 
            // labelDataGridViewTextBoxColumn
            // 
            labelDataGridViewTextBoxColumn.DataPropertyName = "Label";
            labelDataGridViewTextBoxColumn.HeaderText = "Label";
            labelDataGridViewTextBoxColumn.MaxInputLength = 40;
            labelDataGridViewTextBoxColumn.MinimumWidth = 6;
            labelDataGridViewTextBoxColumn.Name = "labelDataGridViewTextBoxColumn";
            labelDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            labelDataGridViewTextBoxColumn.Width = 200;
            // 
            // textValueDataGridViewTextBoxColumn
            // 
            textValueDataGridViewTextBoxColumn.DataPropertyName = "TextValue";
            textValueDataGridViewTextBoxColumn.HeaderText = "TextValue";
            textValueDataGridViewTextBoxColumn.MaxInputLength = 40;
            textValueDataGridViewTextBoxColumn.MinimumWidth = 6;
            textValueDataGridViewTextBoxColumn.Name = "textValueDataGridViewTextBoxColumn";
            textValueDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textValueDataGridViewTextBoxColumn.Width = 150;
            // 
            // dateValueDataGridViewTextBoxColumn
            // 
            dateValueDataGridViewTextBoxColumn.DataPropertyName = "DateValue";
            dateValueDataGridViewTextBoxColumn.HeaderText = "DateValue";
            dateValueDataGridViewTextBoxColumn.MaxInputLength = 25;
            dateValueDataGridViewTextBoxColumn.MinimumWidth = 6;
            dateValueDataGridViewTextBoxColumn.Name = "dateValueDataGridViewTextBoxColumn";
            dateValueDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dateValueDataGridViewTextBoxColumn.Width = 140;
            // 
            // boolValueDataGridViewCheckBoxColumn
            // 
            boolValueDataGridViewCheckBoxColumn.DataPropertyName = "BoolValue";
            boolValueDataGridViewCheckBoxColumn.HeaderText = "BoolValue";
            boolValueDataGridViewCheckBoxColumn.MinimumWidth = 6;
            boolValueDataGridViewCheckBoxColumn.Name = "boolValueDataGridViewCheckBoxColumn";
            boolValueDataGridViewCheckBoxColumn.Width = 90;
            // 
            // DecimalValue
            // 
            DecimalValue.DataPropertyName = "DecimalValue";
            DecimalValue.HeaderText = "DecimalValue";
            DecimalValue.MinimumWidth = 6;
            DecimalValue.Name = "DecimalValue";
            DecimalValue.Width = 125;
            // 
            // lastModifiedDataGridViewTextBoxColumn
            // 
            lastModifiedDataGridViewTextBoxColumn.DataPropertyName = "LastModified";
            lastModifiedDataGridViewTextBoxColumn.HeaderText = "LastModified";
            lastModifiedDataGridViewTextBoxColumn.MinimumWidth = 6;
            lastModifiedDataGridViewTextBoxColumn.Name = "lastModifiedDataGridViewTextBoxColumn";
            lastModifiedDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            lastModifiedDataGridViewTextBoxColumn.Visible = false;
            lastModifiedDataGridViewTextBoxColumn.Width = 125;
            // 
            // btncDelete
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new Padding(2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            btncDelete.DefaultCellStyle = dataGridViewCellStyle1;
            btncDelete.HeaderText = "";
            btncDelete.MinimumWidth = 6;
            btncDelete.Name = "btncDelete";
            btncDelete.Text = "Delete";
            btncDelete.ToolTipText = "Delete this code";
            btncDelete.UseColumnTextForButtonValue = true;
            btncDelete.Width = 80;
            // 
            // DemoDataTableForm
            // 
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.ClientSize = new Size(982, 553);
            this.Controls.Add(btnSaveAsync);
            this.Controls.Add(btnClearValues);
            this.Controls.Add(txtGuidValue);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(txtXmlValue);
            this.Controls.Add(label1);
            this.Controls.Add(btnSetImage);
            this.Controls.Add(pbImage);
            this.Controls.Add(btnSetGuid);
            this.Controls.Add(btnSetXml);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnSave);
            this.Controls.Add(dgvDemoData);
            this.Name = "DemoDataTableForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Edit Demo Data";
            ((System.ComponentModel.ISupportInitialize)dgvDemoData).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDemoData).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbImage).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvDemoData;
        private System.Windows.Forms.BindingSource bsDemoData;
        private Button btnSetXml;
        private Button btnSetGuid;
        private PictureBox pbImage;
        private Button btnSetImage;
        private Label label1;
        private TextBox txtXmlValue;
        private Label label2;
        private Label label3;
        private TextBox txtGuidValue;
        private Button btnClearValues;
        private Button btnSaveAsync;
        private DataGridViewTextBoxColumn listKeyDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn labelDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn textValueDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dateValueDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn boolValueDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn DecimalValue;
        private DataGridViewTextBoxColumn lastModifiedDataGridViewTextBoxColumn;
        private DataGridViewButtonColumn btncDelete;
    }
}