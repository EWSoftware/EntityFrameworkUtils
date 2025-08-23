//===============================================================================================================
// System  : Entity Framework Test Application
// File    : ProductSearchWindow.cs
// Author  : Eric Woodruff
// Updated : 08/19/2025
//
// This file contains the window used to search for products
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/18/2025  EFW  Created the code
//===============================================================================================================

using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaTestApp.Database;
using AvaloniaTestApp.ViewModels;

namespace AvaloniaTestApp;

public partial class ProductSearchWindow : Window
{
    public ProductSearchWindow()
    {
        InitializeComponent();

        this.DataContext = new ProductSearchViewModel();
    }

    /// <summary>
    /// Dispose of the view model when closed
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private void ProductSearchWindow_Closed(object? sender, EventArgs e)
    {
        ((IDisposable)this.DataContext!).Dispose();
    }

    /// <summary>
    /// Add a new product
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private async void btnAdd_Click(object? sender, RoutedEventArgs e)
    {
        ProductAddEditWindow dlg = new();
        
        if(await dlg.ShowDialog<bool>(this))
            ((ProductSearchViewModel)this.DataContext!).Search();
    }

    /// <summary>
    /// Edit a product
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private async void btnEdit_Click(object? sender, RoutedEventArgs e)
    {
        var vm = (ProductSearchViewModel)this.DataContext!;
        vm.SelectedItem = (spProductSearchResult)((Button)sender!).Tag!;

        ProductAddEditWindow dlg = new(vm.SelectedItem.ProductID);

        if(await dlg.ShowDialog<bool>(this))
            vm.Search();
    }

    /// <summary>
    /// Close the window
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private void btnClose_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}