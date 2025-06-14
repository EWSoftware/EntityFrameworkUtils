//===============================================================================================================
// System  : Entity Framework Test Application
// File    : ProductSearchForm.cs
// Author  : Eric Woodruff
// Updated : 12/17/2024
//
// This file contains the form used to search for products
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/17/2024  EFW  Created the code
//===============================================================================================================

using System;
using System.Linq;
using System.Windows.Forms;

using EntityFrameworkNet48TestApp.Database;

using EWSoftware.EntityFramework;

namespace EntityFrameworkNet48TestApp
{
    public partial class ProductSearchForm : Form
    {
        #region Constructor
        //=====================================================================

        public ProductSearchForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        //=====================================================================

        /// <summary>
        /// Search on activation and on the Search button click
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void ProductSearchForm_Activated(object sender, EventArgs e)
        {
            using var dc = new DemoDatabaseDataContext().NoTracking().Open();
            
            if(cboCategoryName.Items.Count == 0)
            {
                cboCategoryName.Items.Add(String.Empty);
                cboCategoryName.Items.AddRange(dc.LoadAll<spCategoriesResult>().Select(c => c.CategoryName).ToArray());

                cboCompanyName.Items.Add(String.Empty);
                cboCompanyName.Items.AddRange(dc.LoadAll<spCompaniesResult>().Select(c => c.CompanyName).ToArray());

                cboCategoryName.SelectedIndex = cboCompanyName.SelectedIndex = 0;
            }

            bsProducts.DataSource = dc.spProductSearch(txtProductName.Text.NullIfWhiteSpace(),
                cboCategoryName.SelectedItem.ToStringOrNull(), cboCompanyName.SelectedItem.ToStringOrNull()).ToList();
        }

        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new ProductAddEditForm(null);
            
            if(dlg.ShowDialog() == DialogResult.OK)
                this.ProductSearchForm_Activated(sender, e);
        }

        /// <summary>
        /// Handle grid view button clicks
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void dgvDemoData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex == btncEdit.Index)
            {
                var product = (spProductSearchResult)bsProducts.Current;

                using var dlg = new ProductAddEditForm(product.ProductID);
                
                if(dlg.ShowDialog() == DialogResult.OK)
                    this.ProductSearchForm_Activated(sender, e);
            }
        }
        #endregion
    }
}
