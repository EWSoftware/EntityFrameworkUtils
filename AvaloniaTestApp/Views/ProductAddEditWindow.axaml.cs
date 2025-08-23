//===============================================================================================================
// System  : Entity Framework Test Application
// File    : ProductAddEditWindow.cs
// Author  : Eric Woodruff
// Updated : 08/20/2025
//
// This file contains the window used to add or edit a product
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/20/2025  EFW  Created the code
//===============================================================================================================

using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaTestApp.ViewModels;

namespace AvaloniaTestApp;

public partial class ProductAddEditWindow : Window
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public ProductAddEditWindow() : this(null)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="productId">The product ID to edit or null to add a new entry</param>
    public ProductAddEditWindow(int? productId)
    {
        InitializeComponent();

        this.DataContext = new ProductAddEditViewModel(productId);

        if(productId != null)
            this.Title = "Edit a Product";
    }

    /// <summary>
    /// Save the changes and close the window
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private void btnSave_Click(object? sender, RoutedEventArgs e)
    {
        var vm = (ProductAddEditViewModel)this.DataContext!;

        vm.SaveChanges();

        this.Close(true);
    }

    /// <summary>
    /// Delete the product
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private void btnDelete_Click(object? sender, RoutedEventArgs e)
    {
        var vm = (ProductAddEditViewModel)this.DataContext!;

        vm.Delete();

        this.Close(true);
    }

    /// <summary>
    /// Close the window
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private void btnClose_Click(object? sender, RoutedEventArgs e)
    {
        this.Close(false);
    }
}