using System.Drawing;
using System.Windows.Forms;

namespace EntityFrameworkNet48TestApp
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
            listKeyDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            labelDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            textValueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dateValueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            boolValueDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            lastModifiedDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            btncDelete = new DataGridViewButtonColumn();
            bsDemoData = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)dgvDemoData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDemoData).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(782, 509);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 32);
            btnClose.TabIndex = 2;
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
            btnSave.TabIndex = 1;
            btnSave.Text = "&Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += this.btnSave_Click;
            // 
            // dgvDemoData
            // 
            dgvDemoData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDemoData.AutoGenerateColumns = false;
            dgvDemoData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDemoData.Columns.AddRange(new DataGridViewColumn[] { listKeyDataGridViewTextBoxColumn, labelDataGridViewTextBoxColumn, textValueDataGridViewTextBoxColumn, dateValueDataGridViewTextBoxColumn, boolValueDataGridViewCheckBoxColumn, lastModifiedDataGridViewTextBoxColumn, btncDelete });
            dgvDemoData.DataSource = bsDemoData;
            dgvDemoData.Location = new Point(12, 12);
            dgvDemoData.Name = "dgvDemoData";
            dgvDemoData.RowHeadersWidth = 51;
            dgvDemoData.RowTemplate.Height = 32;
            dgvDemoData.Size = new Size(858, 491);
            dgvDemoData.TabIndex = 0;
            dgvDemoData.CellContentClick += this.dgvDemoData_CellContentClick;
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
            // bsDemoData
            // 
            bsDemoData.DataSource = typeof(Database.DemoTable);
            // 
            // DemoDataTableForm
            // 
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.ClientSize = new Size(882, 553);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnSave);
            this.Controls.Add(dgvDemoData);
            this.Name = "DemoDataTableForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Edit Demo Data";
            ((System.ComponentModel.ISupportInitialize)dgvDemoData).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDemoData).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvDemoData;
        private System.Windows.Forms.BindingSource bsDemoData;
        private DataGridViewTextBoxColumn listKeyDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn labelDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn textValueDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dateValueDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn boolValueDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn lastModifiedDataGridViewTextBoxColumn;
        private DataGridViewButtonColumn btncDelete;
    }
}
