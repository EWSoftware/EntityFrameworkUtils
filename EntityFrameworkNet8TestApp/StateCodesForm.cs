﻿//===============================================================================================================
// System  : Entity Framework Test Application
// File    : StateCodesForm.cs
// Author  : Eric Woodruff
// Updated : 12/16/2024
//
// This file contains the form used to edit state codes
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/16/2024  EFW  Created the code
//===============================================================================================================

using EntityFrameworkNet8TestApp.Database;

using EWSoftware.EntityFramework;

namespace EntityFrameworkNet8TestApp
{
    public partial class StateCodesForm : Form
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
        public StateCodesForm()
        {
            InitializeComponent();

            dc = new DemoDatabaseDataContext();

            // This table has a modifiable key so it uses a fake primary key to allow editing them.  To make
            // changes trackable, we need to convert it to a binding list.
            bsStateCodes.DataSource = dc.LoadAll<StateCode>().ToTrackingBindingList(dc);
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
            // Save changes.  The state code keys are mutable so we'll handle updates manually by calling the
            // stored procedures ourself.  However, we can use the extension method to handle the details of
            // getting the changes and set their disposition after the updates.
            dc.SubmitChanges<StateCode>(
                e =>
                {
                    // Insert a new state code
                    dc.spStateCodeAddUpdate(null, e.Entity.State, e.Entity.StateDesc);
                    return true;
                },
                e =>
                {
                    // Update an existing state code possibly changing the key
                    dc.spStateCodeAddUpdate((string?)e.OriginalValues[nameof(StateCode.State)],
                        e.Entity.State, e.Entity.StateDesc);
                    return true;
                },
                e =>
                {
                    // Delete an existing state code
                    dc.spStateCodeDelete((string?)e.OriginalValues[nameof(StateCode.State)]);
                    return true;
                });
        }

        /// <summary>
        /// Handled delete grid view button clicks
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void dgvStateCodes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex == btncDelete.Index)
            {
                var stateCode = (StateCode)bsStateCodes.Current;

                if(stateCode.IsInUse)
                {
                    MessageBox.Show("State code is in use and cannot be deleted", "LINQ to SQL Test App",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                    bsStateCodes.RemoveCurrent();
            }
        }
        #endregion
    }
}