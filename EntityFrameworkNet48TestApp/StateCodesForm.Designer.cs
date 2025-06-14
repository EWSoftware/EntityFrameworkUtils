using System.Drawing;
using System.Windows.Forms;

namespace EntityFrameworkNet48TestApp
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            dgvStateCodes = new DataGridView();
            stateDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            stateDescDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            btncDelete = new DataGridViewButtonColumn();
            lastModifiedDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bsStateCodes = new BindingSource(components);
            btnSave = new Button();
            btnClose = new Button();
            btnSaveAsync = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvStateCodes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsStateCodes).BeginInit();
            this.SuspendLayout();
            // 
            // dgvStateCodes
            // 
            dgvStateCodes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvStateCodes.AutoGenerateColumns = false;
            dgvStateCodes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStateCodes.Columns.AddRange(new DataGridViewColumn[] { stateDataGridViewTextBoxColumn, stateDescDataGridViewTextBoxColumn, btncDelete, lastModifiedDataGridViewTextBoxColumn });
            dgvStateCodes.DataSource = bsStateCodes;
            dgvStateCodes.Location = new Point(12, 12);
            dgvStateCodes.Name = "dgvStateCodes";
            dgvStateCodes.RowHeadersWidth = 51;
            dgvStateCodes.RowTemplate.Height = 32;
            dgvStateCodes.Size = new Size(758, 491);
            dgvStateCodes.TabIndex = 0;
            dgvStateCodes.CellContentClick += this.dgvStateCodes_CellContentClick;
            // 
            // stateDataGridViewTextBoxColumn
            // 
            stateDataGridViewTextBoxColumn.DataPropertyName = "State";
            stateDataGridViewTextBoxColumn.HeaderText = "State";
            stateDataGridViewTextBoxColumn.MaxInputLength = 2;
            stateDataGridViewTextBoxColumn.MinimumWidth = 6;
            stateDataGridViewTextBoxColumn.Name = "stateDataGridViewTextBoxColumn";
            stateDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            stateDataGridViewTextBoxColumn.Width = 90;
            // 
            // stateDescDataGridViewTextBoxColumn
            // 
            stateDescDataGridViewTextBoxColumn.DataPropertyName = "StateDesc";
            stateDescDataGridViewTextBoxColumn.HeaderText = "StateDesc";
            stateDescDataGridViewTextBoxColumn.MaxInputLength = 20;
            stateDescDataGridViewTextBoxColumn.MinimumWidth = 6;
            stateDescDataGridViewTextBoxColumn.Name = "stateDescDataGridViewTextBoxColumn";
            stateDescDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            stateDescDataGridViewTextBoxColumn.Width = 300;
            // 
            // btncDelete
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new Padding(2);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
            btncDelete.DefaultCellStyle = dataGridViewCellStyle2;
            btncDelete.HeaderText = "";
            btncDelete.MinimumWidth = 6;
            btncDelete.Name = "btncDelete";
            btncDelete.Text = "Delete";
            btncDelete.ToolTipText = "Delete this code";
            btncDelete.UseColumnTextForButtonValue = true;
            btncDelete.Width = 80;
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
            // bsStateCodes
            // 
            bsStateCodes.DataSource = typeof(Database.StateCode);
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.Location = new Point(12, 509);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 32);
            btnSave.TabIndex = 1;
            btnSave.Text = "&Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += this.btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(682, 509);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 32);
            btnClose.TabIndex = 3;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += this.btnClose_Click;
            // 
            // btnSaveAsync
            // 
            btnSaveAsync.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSaveAsync.Location = new Point(106, 509);
            btnSaveAsync.Name = "btnSaveAsync";
            btnSaveAsync.Size = new Size(120, 32);
            btnSaveAsync.TabIndex = 2;
            btnSaveAsync.Text = "Save &Async";
            btnSaveAsync.UseVisualStyleBackColor = true;
            btnSaveAsync.Click += this.btnSaveAsync_Click;
            // 
            // StateCodesForm
            // 
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.ClientSize = new Size(782, 553);
            this.Controls.Add(btnSaveAsync);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnSave);
            this.Controls.Add(dgvStateCodes);
            this.Name = "StateCodesForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Edit State Codes";
            ((System.ComponentModel.ISupportInitialize)dgvStateCodes).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsStateCodes).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStateCodes;
        private System.Windows.Forms.BindingSource bsStateCodes;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private DataGridViewTextBoxColumn stateDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn stateDescDataGridViewTextBoxColumn;
        private DataGridViewButtonColumn btncDelete;
        private DataGridViewTextBoxColumn lastModifiedDataGridViewTextBoxColumn;
        private Button btnSaveAsync;
    }
}
