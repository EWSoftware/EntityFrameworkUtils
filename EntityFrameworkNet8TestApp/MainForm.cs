//===============================================================================================================
// System  : Entity Framework Test Application
// File    : MainForm.cs
// Author  : Eric Woodruff
// Updated : 06/08/2025
//
// This file contains the main form for the test application
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/16/2024  EFW  Created the code
//===============================================================================================================

using EntityFrameworkNet8TestApp.Database;

namespace EntityFrameworkNet8TestApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            DemoDatabaseDataContext.DatabaseLocation = Path.GetFullPath(@"..\..\..\..\DemoData.mdf");

            if(!File.Exists(DemoDatabaseDataContext.DatabaseLocation))
            {
                MessageBox.Show("Unable to locate test database.  It should be in the main project folder",
                    "Entity Framework Test App", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnEditStateCodes_Click(object sender, EventArgs e)
        {
            using var dlg = new StateCodesForm();
            dlg.ShowDialog();
        }

        private void btnEditDemoDataTable_Click(object sender, EventArgs e)
        {
            using var dlg = new DemoDataTableForm();
            dlg.ShowDialog();
        }

        private void btnProductSearch_Click(object sender, EventArgs e)
        {
            using var dlg = new ProductSearchForm();
            dlg.ShowDialog();
        }

        private void btnTestSyncMethods_Click(object sender, EventArgs e)
        {
            using var dlg = new TestAsyncMethodsForm();
            dlg.ShowDialog();
        }
    }
}
