//===============================================================================================================
// System  : Entity Framework Test Application
// File    : StateCodesWindow.cs
// Author  : Eric Woodruff
// Updated : 08/17/2025
//
// This file contains the window used to edit state codes
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/15/2025  EFW  Created the code
//===============================================================================================================

using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaTestApp.ViewModels;

namespace AvaloniaTestApp;

public partial class StateCodesWindow : Window
{
    /// <summary>
    /// Constructor
    /// </summary>
    public StateCodesWindow()
    {
        InitializeComponent();

        StateCodesViewModel vm = new();

        if(!Design.IsDesignMode && vm.StateCodes.Count != 0)
            vm.SelectedItem = vm.StateCodes[0];

        this.DataContext = vm;
    }

    /// <summary>
    /// Dispose of the view model when closed
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private void StateCodesWindow_Closed(object? sender, EventArgs e)
    {
        ((IDisposable)this.DataContext!).Dispose();
    }

    /// <summary>
    /// Add a new state code
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    /// <remarks>We have to do this manually since the grid control doesn't support adding a new row directly and
    /// we want it to scroll into view afterwards.</remarks>
    private void btnAdd_Click(object? sender, RoutedEventArgs e)
    {
        var vm = (StateCodesViewModel)this.DataContext!;
       
        vm.AddRow();

        dgStateCodes.ScrollIntoView(vm.SelectedItem, null);
        dgStateCodes.SelectedIndex = vm.StateCodes.Count - 1;
        dgStateCodes.Focus();
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
