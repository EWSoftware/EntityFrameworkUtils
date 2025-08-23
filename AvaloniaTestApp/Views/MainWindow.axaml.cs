//===============================================================================================================
// System  : Entity Framework Test Application
// File    : MainWindow.cs
// Author  : Eric Woodruff
// Updated : 08/18/2025
//
// This file contains the main form for the test application
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/15/2025  EFW  Created the code
//===============================================================================================================

using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaTestApp.Database;

namespace AvaloniaTestApp.Views
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // It can't find the database in design mode
            if(!Design.IsDesignMode)
            {
                DemoDatabaseDataContext.DatabaseLocation = Path.GetFullPath(@"..\..\..\..\DemoData.mdf");

                if(!File.Exists(DemoDatabaseDataContext.DatabaseLocation))
                    throw new InvalidOperationException("Unable to locate test database.  It should be in the main project folder");
            }
        }

        /// <summary>
        /// Edit the state codes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnEditStateCodes_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new StateCodesWindow();
            dlg.ShowDialog(this);
        }

        /// <summary>
        /// Edit the demo data table
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnDemoDataTable_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new DemoDataTableWindow();
            dlg.ShowDialog(this);
        }

        /// <summary>
        /// Test product search and edit
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnProductSearch_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ProductSearchWindow();
            dlg.ShowDialog(this);
        }

        /// <summary>
        /// Test the asynchronous extension methods
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnTestAsyncMethods_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TestAsyncMethodsWindow();
            dlg.ShowDialog(this);
        }
    }
}