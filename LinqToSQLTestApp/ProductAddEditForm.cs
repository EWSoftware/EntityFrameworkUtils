//===============================================================================================================
// System  : LINQ to SQL Test App
// File    : ProductAddEditForm.cs
// Author  : Eric Woodruff
// Updated : 12/17/2024
//
// This file contains the form used to add or edit products
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/17/2024  EFW  Created the code
//===============================================================================================================

using System;
using System.Linq;
using System.Windows.Forms;

using LinqToSQLTestApp.Database;

namespace LinqToSQLTestApp
{
    public partial class ProductAddEditForm : Form
    {
        #region Private data members
        //=====================================================================

        private int? productID;

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productID">The product ID to edit or null to add a new entry</param>
        public ProductAddEditForm(int? productID)
        {
            InitializeComponent();

            this.productID = productID;

            using(var dc = new DemoDatabaseDataContext())
            {
                dc.ObjectTrackingEnabled = false;
                dc.Connection.Open();

                if(productID != null)
                {
                    this.Text = "Edit Product Info";

                    var productInfo = dc.spProductInfo(productID).Single();

                    lblProductID.Text = productID.ToString();
                    txtProductName.Text = productInfo.ProductName;
                    txtCategoryName.Text = productInfo.CategoryName;
                    txtCompany.Text = productInfo.CompanyName;
                    txtQtyPerUnit.Text = productInfo.QuantityPerUnit;
                    udcUnitPrice.Value = productInfo.UnitPrice;
                    udcUnitsInStock.Value = productInfo.UnitsInStock;
                    udcUnitsOnOrder.Value = productInfo.UnitsOnOrder;
                    udcReorderLevel.Value = productInfo.ReorderLevel;
                    chkDiscontinued.Checked = productInfo.Discontinued;
                }
                else
                    btnDelete.Visible = false;
            }
        }

        /// <summary>
        /// Close the form without saving changes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Save the changes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            using(var dc = new DemoDatabaseDataContext())
            {
                dc.spProductAddUpdate(ref productID, txtProductName.Text.NullIfWhiteSpace(),
                    txtCategoryName.Text.NullIfWhiteSpace(), txtCompany.Text.NullIfWhiteSpace(),
                    txtQtyPerUnit.Text.NullIfWhiteSpace(), udcUnitPrice.Value, (short)udcUnitsInStock.Value,
                    (short)udcUnitsOnOrder.Value, (short)udcReorderLevel.Value, chkDiscontinued.Checked);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Delete the product
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete this product", "Delete Product",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) ==
              DialogResult.Yes)
            {
                using(var dc = new DemoDatabaseDataContext())
                {
                    dc.spProductDelete(productID);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
        #endregion
    }
}
