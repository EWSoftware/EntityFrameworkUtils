//===============================================================================================================
// System  : Entity Framework Test Application
// File    : DemoDataTableForm.cs
// Author  : Eric Woodruff
// Updated : 12/16/2024
//
// This file contains the form used to edit the data in the demo table
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/16/2024  EFW  Created the code
//===============================================================================================================

using System;
using System.Windows.Forms;

using EntityFrameworkNet48TestApp.Database;

using EWSoftware.EntityFramework;

namespace EntityFrameworkNet48TestApp
{
    public partial class DemoDataTableForm : Form
    {
        #region Private data members
        //=====================================================================

        private readonly DemoDatabaseDataContext dc;

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public DemoDataTableForm()
        {
            InitializeComponent();

            dc = new DemoDatabaseDataContext();

            // This table has an identity column.  To make changes trackable, we need to convert it to a binding
            // list.
            bsDemoData.DataSource = dc.LoadAll<DemoTable>().ToTrackingBindingList(dc);
        }
        #endregion

        #region Event handlers
        //=====================================================================

        /// <summary>
        /// Close without saving any pending changes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Save any pending changes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // The demo table keys are immutable so we can use the extension method that uses the insert, update,
            // and delete stored procedure attributes to submit all changes.
            dc.SubmitChanges<DemoTable>();
        }

        /// <summary>
        /// Handled delete grid view button clicks
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void dgvDemoData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex == btncDelete.Index)
                bsDemoData.RemoveCurrent();
        }
        #endregion
    }
}
