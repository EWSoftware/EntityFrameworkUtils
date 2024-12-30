//===============================================================================================================
// System  : LINQ to SQL Test App
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

using System;
using System.Windows.Forms;

using LinqToSQLTestApp.Database;

namespace LinqToSQLTestApp
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

            // This table has a modifiable key so it uses a fake primary key to allow editing them
            bsStateCodes.DataSource = dc.spStateCodes();
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
            dc.SubmitChanges();
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
