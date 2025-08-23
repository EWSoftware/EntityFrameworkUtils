//===============================================================================================================
// System  : Entity Framework Test Application
// File    : ProductSearchViewModel.cs
// Author  : Eric Woodruff
// Updated : 08/19/2025
//
// This file contains the view model used to search for products
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/18/2025  EFW  Created the code
//===============================================================================================================

using Avalonia.Controls;

using AvaloniaTestApp.Database;

using CommunityToolkit.Mvvm.ComponentModel;

using EWSoftware.EntityFramework;

namespace AvaloniaTestApp.ViewModels
{
    internal partial class ProductAddEditViewModel : ObservableObject, IDisposable
    {
        #region Private data members
        //=====================================================================

        private readonly DemoDatabaseDataContext dc;

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property is used to get the product information
        /// </summary>
        [ObservableProperty]
        public partial ProductInfo Product { get; private set; } = null!;

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductAddEditViewModel() : this(null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productId">The product ID to edit or null to add a new one</param>
        public ProductAddEditViewModel(int? productId)
        {
            dc = new DemoDatabaseDataContext();

            // It can't find the database in design mode
            if(!Design.IsDesignMode)
            {
                dc.NoTracking();

                if(productId != null)
                    this.Product = dc.LoadByKey<ProductInfo>(productId).Single();
                else
                    this.Product = new ProductInfo();
            }
        }
        #endregion

        #region IDisposable implementation
        //=====================================================================

        /// <inheritdoc />
        public void Dispose()
        {
            dc?.Dispose();
        }
        #endregion

        #region Commands
        //=====================================================================

        /// <summary>
        /// Add or update the product
        /// </summary>
        public void SaveChanges()
        {
            // For this version we can use the update extension method.  The stored procedure is the same for
            // adding or updating in this case so we can use UpdateEntity for both.
            dc.UpdateEntity(this.Product);
        }

        /// <summary>
        /// Delete the product
        /// </summary>
        public void Delete()
        {
            dc.DeleteEntity(this.Product);
        }
        #endregion
    }
}
