//===============================================================================================================
// System  : Entity Framework Test Application
// File    : DemoDataTableViewModel.cs
// Author  : Eric Woodruff
// Updated : 08/17/2025
//
// This file contains the view model used to edit the demo table
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/17/2025  EFW  Created the code
//===============================================================================================================

using System.Runtime.InteropServices;
using System.Xml.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using AvaloniaTestApp.Database;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using EWSoftware.EntityFramework;

namespace AvaloniaTestApp.ViewModels
{
    internal partial class DemoDataTableViewModel : ObservableObject, IDisposable
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
        /// This read-only property is used to get the demo table rows
        /// </summary>
        public TrackingObservableCollection<DemoTable> DemoTableRows { get; } = null!;

        /// <summary>
        /// This is used to get or set the selected item in the data grid
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedItemImage))]
        public partial DemoTable? SelectedItem { get; set; }

        /// <summary>
        /// Get the image for the selected item
        /// </summary>
        public Bitmap? SelectedItemImage
        {
            get
            {
                if((this.SelectedItem?.ImageValue?.Length ?? 0) != 0)
                {
                    using var ms = new MemoryStream(this.SelectedItem!.ImageValue!);
                    return new Bitmap(ms);
                }

                return null;
            }
        }
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public DemoDataTableViewModel()
        {
            dc = new();

            // It can't find the database in design mode
            if(!Design.IsDesignMode)
            {
                // This table has an identity column.  To make changes trackable, we need to convert it to a
                // binding list.
                this.DemoTableRows = dc.LoadAll<DemoTable>().ToTrackingCollection(dc);
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
        public void AddRow()
        {
            DemoTable newRow = new()
            {
                Label = "New label",
                TextValue = "New text Value"
            };

            this.DemoTableRows.Add(newRow);
            this.SelectedItem = newRow;
        }

        /// <summary>
        /// Delete a row
        /// </summary>
        /// <param name="row">The row to delete</param>
        [RelayCommand]
        public void DeleteRow(DemoTable row)
        {
            if(row != null)
                this.DemoTableRows.Remove(row);
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
            // The demo table keys are immutable so we can use the extension method that uses the insert, update,
            // and delete stored procedure attributes to submit all changes.
            dc.SubmitChanges<DemoTable>();
        }

        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanExecuteSave))]
        public async Task SaveAsynchronous()
        {
            // Disable saving again until this operation is complete
            this.CanSave = false;

            await dc.SubmitChangesAsync<DemoTable>();

            this.CanSave = true;
        }

        /// <summary>
        /// Set the XML column value to test XML column handling
        /// </summary>
        [RelayCommand]
        public void SetXmlValue()
        {
            if(this.SelectedItem != null)
            {
                this.SelectedItem.XmlValue = new XElement("TestElement",
                    new XAttribute("Date", DateTime.Today.ToString("d")),
                    new XElement("Time", DateTime.Now.ToString("T")));

                // Change tracking doesn't work because the column is unmapped.  We'll force it by marking
                // a different column as modified.
                dc.Entry(this.SelectedItem).Property(nameof(DemoTable.TextValue)).IsModified = true;
            }
        }

        /// <summary>
        /// Set the GUID column value to test GUID column handling
        /// </summary>
        [RelayCommand]
        public void SetGuidValue()
        {
            this.SelectedItem?.GuidValue = Guid.NewGuid();
        }

        /// <summary>
        /// Set the image value
        /// </summary>
        [RelayCommand]
        public void SetImageValue()
        {
            if(this.SelectedItem != null)
            {
                var random = new Random();
                int width = 100, height = 100, pixelSize = 20;
                byte[] pixels = new byte[width * height * 4];

                for(int y = 0; y < height; y += pixelSize)
                {
                    for(int x = 0; x < width; x += pixelSize)
                    {
                        byte r = (byte)random.Next(256);
                        byte g = (byte)random.Next(256);
                        byte b = (byte)random.Next(256);
                        byte a = 255;

                        for(int py = 0; py < pixelSize; py++)
                        {
                            for(int px = 0; px < pixelSize; px++)
                            {
                                int pxX = x + px, pxY = y + py;

                                if(pxX < width && pxY < height)
                                {
                                    int idx = (pxY * width + pxX) * 4;

                                    pixels[idx + 0] = b;
                                    pixels[idx + 1] = g;
                                    pixels[idx + 2] = r;
                                    pixels[idx + 3] = a;
                                }
                            }
                        }
                    }
                }

                using(WriteableBitmap bitmap = new(new PixelSize(width, height), new Vector(96, 96),
                  PixelFormat.Bgra8888, AlphaFormat.Premul))
                {
                    using(var frameBuffer = bitmap.Lock())
                    {
                        Marshal.Copy(pixels, 0, frameBuffer.Address, pixels.Length);
                    }

                    using var ms = new MemoryStream();
                    {
                        bitmap.Save(ms);
                        this.SelectedItem.ImageValue = ms.ToArray();
                    }
                }

                this.OnPropertyChanged(nameof(SelectedItemImage));
            }
        }

        /// <summary>
        /// Clear the XML, GUID, and image values
        /// </summary>
        [RelayCommand]
        public void ClearValues()
        {
            if(this.SelectedItem != null)
            {
                this.SelectedItem.XmlValue = null;
                this.SelectedItem.GuidValue = null;
                this.SelectedItem.ImageValue = null;
                this.OnPropertyChanged(nameof(SelectedItemImage));
            }
        }
        #endregion
    }
}
