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
            ((System.ComponentModel.ISupportInitialize)(this.dgvDemoData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDemoData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(782, 509);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 32);
            this.btnClose.TabIndex = 2;
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
            this.btnSave.TabIndex = 1;
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
            this.dgvDemoData.Size = new System.Drawing.Size(858, 491);
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
            // 
            // DemoDataTableForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(882, 553);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvDemoData);
            this.Name = "DemoDataTableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Demo Data";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDemoData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDemoData)).EndInit();
            this.ResumeLayout(false);

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
    }
}