//===============================================================================================================
// System  : Entity Framework Test Application
// File    : TestAsyncMethodsForm.cs
// Author  : Eric Woodruff
// Updated : 06/14/2025
//
// This file contains the form used to test the asynchronous methods in the Entity Framework Utilities library.
// Note that the SubmitChangesAsync and ExecuteNonQueryAsync methods are demonstrated on the Demo Data Table and
// State Codes forms.
//
//    Date     Who  Comments
// ==============================================================================================================
// 06/08/2025  EFW  Created the code
//===============================================================================================================

using System.Diagnostics;

using EWSoftware.EntityFramework;

using EntityFrameworkNet8TestApp.Database;

namespace EntityFrameworkNet8TestApp
{
    public partial class TestAsyncMethodsForm : Form
    {
        #region Private data members
        //=====================================================================

        private CancellationTokenSource cts = new();

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public TestAsyncMethodsForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        //=====================================================================

        /// <summary>
        /// Cancel any running tasks and close the form
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            cts.Cancel();

            this.Close();
        }

        /// <summary>
        /// Stop anything being loaded and clear the results
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnClearResults_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            lbRows.Items.Clear();
        }

        /// <summary>
        /// Test the asynchronous load all method
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private async void btnLoadAll_Click(object sender, EventArgs e)
        {
            if(cts.IsCancellationRequested)
                cts = new();

            try
            {
                using var dc = new DemoDatabaseDataContext().NoTracking();

                // await foreach(var d in dc.LoadAllAsync<DemoTable>().WithCancellation(cts.Token)) is also supported
                await foreach(var d in dc.LoadAllAsync<DemoTable>(cts.Token))
                {
                    lbRows.Items.Add($"Load All: {d.TextValue}, {d.DateValue:d}");

                    // This is just a test so delay for a bit or it loads too quickly
                    await Task.Delay(250, cts.Token);
                }
            }
            catch(OperationCanceledException)
            {
                // Ignore cancellation
                Debug.WriteLine("Async load all cancelled");
            }
        }

        /// <summary>
        /// Test the asynchronous load by method
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private async void btnLoadByKey_Click(object sender, EventArgs e)
        {
            if(cts.IsCancellationRequested)
                cts = new();

            try
            {
                using var dc = new DemoDatabaseDataContext().NoTracking();

                // Some invalid product IDs are included as well
                foreach(int productId in new int[] { 4, 9999, 17, -1, 37, 0, 51 })
                {
                    var p = await dc.LoadByKeyAsync<ProductInfo>([productId], cts.Token).SingleOrDefaultAsync(cts.Token);

                    if(p != null)
                        lbRows.Items.Add($"Load By Key: {p.ProductID}, {p.ProductName}, {p.UnitPrice:C2}");
                    else
                        lbRows.Items.Add($"Load By Key: Product ID {productId} not found");

                    // This is just a test so delay for a bit or it loads too quickly
                    await Task.Delay(250, cts.Token);
                }
            }
            catch(OperationCanceledException)
            {
                // Ignore cancellation
                Debug.WriteLine("Async load by key cancelled");
            }
        }

        /// <summary>
        /// Test the asynchronous CRUD methods
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private async void btnCRUDMethods_Click(object sender, EventArgs e)
        {
            if(cts.IsCancellationRequested)
                cts = new();

            try
            {
                using var dc = await new DemoDatabaseDataContext().NoTracking().OpenAsync(cts.Token);

                var newRow = new DemoTable
                {
                    Label = "New Row",
                    TextValue = "Test async insert",
                    DateValue = DateTime.Now
                };

                await dc.InsertEntityAsync(newRow, cts.Token);

                lbRows.Items.Add($"Inserted Async: {newRow.ListKey}, {newRow.TextValue} {newRow.DateValue:d}");

                // This is just a test so delay for a bit between each step or it runs too quickly
                await Task.Delay(250, cts.Token);

                var r = await dc.LoadByKeyAsync<DemoTable>([newRow.ListKey], cts.Token).SingleAsync(cts.Token);

                lbRows.Items.Add($"Loaded inserted row: {r.ListKey}, {r.TextValue}");

                r.TextValue = "Test async update";
                r.DateValue = r.DateValue.AddDays(7);

                await Task.Delay(250, cts.Token);

                await dc.UpdateEntityAsync(r, cts.Token);

                lbRows.Items.Add($"Updated Async: {r.ListKey}, {r.TextValue} {r.DateValue:d}");

                await Task.Delay(250, cts.Token);

                r = await dc.LoadByKeyAsync<DemoTable>([r.ListKey], cts.Token).SingleAsync(cts.Token);

                lbRows.Items.Add($"Loaded updated row: {r.ListKey}, {r.TextValue} {r.DateValue:d}");

                await dc.DeleteEntityAsync(r, cts.Token);

                lbRows.Items.Add($"Deleted Async: {r.ListKey}, {r.TextValue} {r.DateValue:d}");

                await Task.Delay(250, cts.Token);

                var d = await dc.LoadByKeyAsync<DemoTable>([r.ListKey], cts.Token).SingleOrDefaultAsync(cts.Token);

                if(d == null)
                    lbRows.Items.Add($"Row key {r.ListKey} deleted successfully");
                else
                    lbRows.Items.Add($"Row key {r.ListKey} not deleted successfully");

                await Task.Delay(250, cts.Token);
            }
            catch(OperationCanceledException)
            {
                // Ignore cancellation
                Debug.WriteLine("Async CRUD operations cancelled");
            }
        }

        /// <summary>
        /// Demonstrate the use of asynchronous method query execution
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private async void btnExecuteMethodQueryAsync_Click(object sender, EventArgs e)
        {
            if(cts.IsCancellationRequested)
                cts = new();

            try
            {
                using var dc = new DemoDatabaseDataContext().NoTracking();

                // We can't pass the cancellation token to the query method as it will look like a parameter to
                // the store procedure.  We need to use the WithCancellation() extension method instead.
                await foreach(var p in dc.spProductSearchAsync("ch", null, null).WithCancellation(cts.Token))
                {
                    lbRows.Items.Add($"Execute Method Query: {p.ProductID}, {p.ProductName}");

                    // This is just a test so delay for a bit or it loads too quickly
                    await Task.Delay(250, cts.Token);
                }
            }
            catch(OperationCanceledException)
            {
                // Ignore cancellation
                Debug.WriteLine("Execute method query cancelled");
            }
        }
        #endregion
    }
}
