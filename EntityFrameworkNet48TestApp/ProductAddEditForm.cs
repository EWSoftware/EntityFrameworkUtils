//===============================================================================================================
// System  : Entity Framework Test Application
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

using EntityFrameworkNet48TestApp.Database;

using EWSoftware.EntityFramework;

namespace EntityFrameworkNet48TestApp
{
    public partial class ProductAddEditForm : Form
    {
        #region Private data members
        //=====================================================================

        private readonly int? productID;
        
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

            // We could use data binding but this is just a simple test application so we'll do it unbound
            // with no tracking.
            using(var dc = new DemoDatabaseDataContext().NoTracking())
            {
                if(productID != null)
                {
                    this.Text = "Edit Product Info";

                    var productInfo = dc.LoadByKey<ProductInfo>(productID).Single();

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
            using(var dc = new DemoDatabaseDataContext().NoTracking())
            {
                // For this version we can use the update extension method.  The stored procedure is the same for
                // adding or updating in this case so we can use UpdateEntity for both.
                dc.UpdateEntity(new ProductInfo
                {
                    ProductID = productID ?? 0,
                    ProductName = txtProductName.Text.NullIfWhiteSpace(),
                    CategoryName = txtCategoryName.Text.NullIfWhiteSpace(),
                    CompanyName = txtCompany.Text.NullIfWhiteSpace(),
                    QuantityPerUnit = txtQtyPerUnit.Text.NullIfWhiteSpace(),
                    UnitPrice = udcUnitPrice.Value,
                    UnitsInStock = (short)udcUnitsInStock.Value,
                    UnitsOnOrder = (short)udcUnitsOnOrder.Value,
                    ReorderLevel = (short)udcReorderLevel.Value,
                    Discontinued = chkDiscontinued.Checked
                });

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
                using(var dc = new DemoDatabaseDataContext().NoTracking())
                {
                    // For this version we can use the delete extension method or we could add a method to the
                    // data context and call it.
                    dc.DeleteEntity(new ProductInfo { ProductID = productID.Value });

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
        #endregion
    }
}
