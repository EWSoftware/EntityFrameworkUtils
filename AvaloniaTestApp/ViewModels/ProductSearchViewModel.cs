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

using System.Collections.ObjectModel;
using Avalonia.Controls;

using AvaloniaTestApp.Database;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using EWSoftware.EntityFramework;

namespace AvaloniaTestApp.ViewModels
{
    internal partial class ProductSearchViewModel : ObservableObject, IDisposable
    {
        #region Private data members
        //=====================================================================

        private readonly DemoDatabaseDataContext dc;

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// The product name for which to search
        /// </summary>
        [ObservableProperty]
        public partial string? ProductName { get; set; }

        /// <summary>
        /// The category name for which to search
        /// </summary>
        [ObservableProperty]
        public partial string? CategoryName { get; set; }

        /// <summary>
        /// The company name for which to search
        /// </summary>
        [ObservableProperty]
        public partial string? CompanyName { get; set; }

        /// <summary>
        /// This read-only property is used to get the products found by the search
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<spProductSearchResult> Products { get; private set; } = null!;

        /// <summary>
        /// This is used to get or set the selected item in the data grid
        /// </summary>
        [ObservableProperty]
        public partial spProductSearchResult? SelectedItem { get; set; }

        /// <summary>
        /// This read-only property is used to get the list of categories
        /// </summary>
        public List<string> Categories { get; } = null!;

        /// <summary>
        /// This read-only property is used to get the list of companies
        /// </summary>
        public List<string> Companies { get; } = null!;

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public ProductSearchViewModel()
        {
            dc = new DemoDatabaseDataContext();

            // It can't find the database in design mode
            if(!Design.IsDesignMode)
            {
                dc.NoTracking();

                this.Categories = [.. dc.LoadAll<spCategoriesResult>().Select(c => c.CategoryName)];
                this.Companies = [.. dc.LoadAll<spCompaniesResult>().Select(c => c.CompanyName)];

                this.Categories.Insert(0, String.Empty);
                this.Companies.Insert(0, String.Empty);

                this.Search();
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
        /// Search for products
        /// </summary>
        [RelayCommand]
        public void Search()
        {
            if(!Design.IsDesignMode)
            {
                this.Products = new ObservableCollection<spProductSearchResult>(
                    [.. dc.spProductSearch(this.ProductName.ToStringOrNull(), this.CategoryName.ToStringOrNull(),
                    this.CompanyName.ToStringOrNull())]);
            }
        }
        #endregion
    }
}
