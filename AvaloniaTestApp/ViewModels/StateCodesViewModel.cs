//===============================================================================================================
// System  : Entity Framework Test Application
// File    : StateCodesViewModel.cs
// Author  : Eric Woodruff
// Updated : 08/17/2025
//
// This file contains the view model used to edit state codes
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/17/2025  EFW  Created the code
//===============================================================================================================

using Avalonia.Controls;

using AvaloniaTestApp.Database;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using EWSoftware.EntityFramework;

namespace AvaloniaTestApp.ViewModels
{
    internal partial class StateCodesViewModel : ObservableObject, IDisposable
    {
        #region Private data members
        //=====================================================================

        private readonly DemoDatabaseDataContext dc;

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// True if save is enabled, false if not
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand)), NotifyCanExecuteChangedFor(nameof(SaveAsynchronousCommand))]
        public partial bool CanSave { get; set; } = true;

        /// <summary>
        /// This read-only property is used to get the state codes
        /// </summary>
        public TrackingObservableCollection<StateCode> StateCodes { get; } = null!;

        /// <summary>
        /// This is used to get or set the selected item in the data grid
        /// </summary>
        [ObservableProperty]
        public partial StateCode? SelectedItem { get; set; }

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public StateCodesViewModel()
        {
            dc = new();

            // It can't find the database in design mode
            if(!Design.IsDesignMode)
            {
                // This table has a modifiable key so it uses a fake primary key to allow editing them.  To make
                // changes trackable, we need to convert it to a tracking collection.
                this.StateCodes = dc.LoadAll<StateCode>().ToTrackingCollection(dc);
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
        /// Add a new row since the grid doesn't support adding rows directly
        /// </summary>
        [RelayCommand]
        public void AddRow()
        {
            StateCode stateCode = new()
            {
                State = "XX",
                StateDesc = "New State Code"
            };

            this.StateCodes.Add(stateCode);
            this.SelectedItem = stateCode;
        }

        /// <summary>
        /// Delete a row
        /// </summary>
        /// <param name="state">The state to delete</param>
        [RelayCommand]
        public void DeleteRow(StateCode state)
        {
            if(!(state?.IsInUse ?? true))
                this.StateCodes.Remove(state);
        }

        /// <summary>
        /// Indicate whether or not the save command can execute
        /// </summary>
        /// <retrns>True if the save command can be executed, false if not</retrns>
        private bool CanExecuteSave()
        {
            return this.CanSave;
        }

        /// <summary>
        /// Save changes synchronously
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanExecuteSave))]
        public void Save()
        {
            // Save changes.  The state code keys are mutable so we'll handle updates manually by calling the
            // stored procedures ourself.  However, we can use the extension method to handle the details of
            // getting the changes and setting their disposition after the updates.
            dc.SubmitChanges<StateCode>(
                se =>
                {
                    // Insert a new state code
                    dc.spStateCodeAddUpdate(null, se.Entity.State, se.Entity.StateDesc);
                    return true;
                },
                se =>
                {
                    // Update an existing state code possibly changing the key
                    dc.spStateCodeAddUpdate((string?)se.OriginalValues[nameof(StateCode.State)],
                        se.Entity.State, se.Entity.StateDesc);
                    return true;
                },
                se =>
                {
                    // Delete an existing state code
                    dc.spStateCodeDelete((string?)se.OriginalValues[nameof(StateCode.State)]);
                    return true;
                });
        }

        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanExecuteSave))]
        public async Task SaveAsynchronous()
        {
            // Disable saving again until this operation is complete
            this.CanSave = false;

            // Save changes.  The state code keys are mutable so we'll handle updates manually by calling the
            // stored procedures ourself.  However, we can use the extension method to handle the details of
            // getting the changes and setting their disposition after the updates.
            await dc.SubmitChangesAsync<StateCode>(
                async se =>
                {
                    // Insert a new state code
                    await dc.spStateCodeAddUpdateAsync(null, se.Entity.State, se.Entity.StateDesc);
                    return true;
                },
                async se =>
                {
                    // Update an existing state code possibly changing the key
                    await dc.spStateCodeAddUpdateAsync((string?)se.OriginalValues[nameof(StateCode.State)],
                        se.Entity.State, se.Entity.StateDesc);
                    return true;
                },
                async se =>
                {
                    // Delete an existing state code
                    await dc.spStateCodeDeleteAsync((string?)se.OriginalValues[nameof(StateCode.State)]);
                    return true;
                });

            this.CanSave = true;
        }
        #endregion
    }
}
