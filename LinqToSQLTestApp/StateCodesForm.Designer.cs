namespace LinqToSQLTestApp
{
    partial class StateCodesForm
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
            this.dgvStateCodes = new System.Windows.Forms.DataGridView();
            this.stateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stateDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btncDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lastModifiedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsStateCodes = new System.Windows.Forms.BindingSource(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStateCodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsStateCodes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvStateCodes
            // 
            this.dgvStateCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStateCodes.AutoGenerateColumns = false;
            this.dgvStateCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStateCodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.stateDataGridViewTextBoxColumn,
            this.stateDescDataGridViewTextBoxColumn,
            this.btncDelete,
            this.lastModifiedDataGridViewTextBoxColumn});
            this.dgvStateCodes.DataSource = this.bsStateCodes;
            this.dgvStateCodes.Location = new System.Drawing.Point(12, 12);
            this.dgvStateCodes.Name = "dgvStateCodes";
            this.dgvStateCodes.RowHeadersWidth = 51;
            this.dgvStateCodes.RowTemplate.Height = 32;
            this.dgvStateCodes.Size = new System.Drawing.Size(758, 491);
            this.dgvStateCodes.TabIndex = 0;
            this.dgvStateCodes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStateCodes_CellContentClick);
            // 
            // stateDataGridViewTextBoxColumn
            // 
            this.stateDataGridViewTextBoxColumn.DataPropertyName = "State";
            this.stateDataGridViewTextBoxColumn.HeaderText = "State";
            this.stateDataGridViewTextBoxColumn.MaxInputLength = 2;
            this.stateDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.stateDataGridViewTextBoxColumn.Name = "stateDataGridViewTextBoxColumn";
            this.stateDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.stateDataGridViewTextBoxColumn.Width = 90;
            // 
            // stateDescDataGridViewTextBoxColumn
            // 
            this.stateDescDataGridViewTextBoxColumn.DataPropertyName = "StateDesc";
            this.stateDescDataGridViewTextBoxColumn.HeaderText = "StateDesc";
            this.stateDescDataGridViewTextBoxColumn.MaxInputLength = 20;
            this.stateDescDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.stateDescDataGridViewTextBoxColumn.Name = "stateDescDataGridViewTextBoxColumn";
            this.stateDescDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.stateDescDataGridViewTextBoxColumn.Width = 300;
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
            // bsStateCodes
            // 
            this.bsStateCodes.DataSource = typeof(LinqToSQLTestApp.Database.StateCode);
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
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(682, 509);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 32);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // StateCodesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvStateCodes);
            this.Name = "StateCodesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit State Codes";
            ((System.ComponentModel.ISupportInitialize)(this.dgvStateCodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsStateCodes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStateCodes;
        private System.Windows.Forms.BindingSource bsStateCodes;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn stateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn stateDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn btncDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastModifiedDataGridViewTextBoxColumn;
    }
}