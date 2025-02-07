﻿//===============================================================================================================
// System  : LINQ to SQL Test App
// File    : MainForm.cs
// Author  : Eric Woodruff
// Updated : 12/17/2024
//
// This file contains the main form for the test application
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/16/2024  EFW  Created the code
//===============================================================================================================

using System;
using System.IO;
using System.Windows.Forms;

using LinqToSQLTestApp.Database;

namespace LinqToSQLTestApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            DemoDatabaseDataContext.DatabaseLocation = Path.GetFullPath(@"..\..\..\DemoData.mdf");

            if(!File.Exists(DemoDatabaseDataContext.DatabaseLocation))
            {
                MessageBox.Show("Unable to locate test database.  It should be in the main project folder",
                    "Entity Framework Test App", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnEditStateCodes_Click(object sender, EventArgs e)
        {
            using(var dlg = new StateCodesForm())
            {
                dlg.ShowDialog();
            }
        }

        private void btnEditDemoDataTable_Click(object sender, EventArgs e)
        {
            using(var dlg = new DemoDataTableForm())
            {
                dlg.ShowDialog();
            }
        }

        private void btnProductSearch_Click(object sender, EventArgs e)
        {
            using(var dlg = new ProductSearchForm())
            {
                dlg.ShowDialog();
            }
        }
    }
}
