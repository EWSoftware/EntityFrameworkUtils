//===============================================================================================================
// System  : Entity Framework Test Application
// File    : DemoDataTableForm.cs
// Author  : Eric Woodruff
// Updated : 02/15/2025
//
// This file contains the form used to edit the data in the demo table
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/16/2024  EFW  Created the code
//===============================================================================================================

using System.Drawing.Imaging;
using System.Xml.Linq;

using EntityFrameworkNet8TestApp.Database;

using EWSoftware.EntityFramework;

namespace EntityFrameworkNet8TestApp
{
    public partial class DemoDataTableForm : Form
    {
        #region Private data members
        //=====================================================================

        private readonly DemoDatabaseDataContext dc;

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public DemoDataTableForm()
        {
            InitializeComponent();

            dc = new DemoDatabaseDataContext();

            // This table has an identity column.  To make changes trackable, we need to convert it to a binding
            // list.
            bsDemoData.DataSource = dc.LoadAll<DemoTable>().ToTrackingBindingList(dc);
        }
        #endregion

        #region Event handlers
        //=====================================================================

        /// <summary>
        /// Close without saving any pending changes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Save any pending changes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // The demo table keys are immutable so we can use the extension method that uses the insert, update,
            // and delete stored procedure attributes to submit all changes.
            dc.SubmitChanges<DemoTable>();
        }

        /// <summary>
        /// Handled delete grid view button clicks
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void dgvDemoData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex == btncDelete.Index)
                bsDemoData.RemoveCurrent();
        }

        /// <summary>
        /// Show the content of the fields not editable in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bsDemoData_CurrentChanged(object sender, EventArgs e)
        {
            var row = (DemoTable)bsDemoData.Current;

            if(row != null)
            {
                txtXmlValue.Text = row.XmlValue?.ToString() ?? "(Not set)";
                txtGuidValue.Text = row.GuidValue?.ToString() ?? "(Not set)";

                pbImage.Image?.Dispose();
                pbImage.Image = null;

                if((row.ImageValue?.Length ?? 0) != 0)
                {
                    using var ms = new MemoryStream(row.ImageValue!);
                    pbImage.Image = Image.FromStream(ms);
                }
            }
        }

        /// <summary>
        /// Set the XML column value to test XML column handling
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnSetXml_Click(object sender, EventArgs e)
        {
            var row = (DemoTable)bsDemoData.Current;

            if(row != null)
            {
                row.XmlValue = new XElement("TestElement",
                    new XAttribute("Date", DateTime.Today.ToString("d")),
                    new XElement("Time", DateTime.Now.ToString("T")));

                // Change tracking doesn't work because the column is unmapped.  We'll force it by marking
                // a different column as modified.
                dc.Entry(row).Property(nameof(DemoTable.TextValue)).IsModified = true;

                this.bsDemoData_CurrentChanged(sender, e);
            }
        }

        /// <summary>
        /// Set the GUID column value to test GUID column handling
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnSetGuid_Click(object sender, EventArgs e)
        {
            var row = (DemoTable)bsDemoData.Current;

            if(row != null)
            {
                row.GuidValue = Guid.NewGuid();

                this.bsDemoData_CurrentChanged(sender, e);
            }
        }

        /// <summary>
        /// Set the image value
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnSetImage_Click(object sender, EventArgs e)
        {
            var row = (DemoTable)bsDemoData.Current;

            if(row != null)
            {
                var random = new Random();
                var bmp = new Bitmap(100, 100);

                using Graphics g = Graphics.FromImage(bmp);

                for(int y = 0; y < 100; y += 20)
                {
                    for(int x = 0; x < 100; x += 20)
                    {
                        Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                        using Brush brush = new SolidBrush(randomColor);

                        g.FillRectangle(brush, x, y, 20, 20);
                    }
                }

                using var ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                row.ImageValue = ms.ToArray();

                this.bsDemoData_CurrentChanged(sender, e);
            }
        }

        /// <summary>
        /// Clear the XML, GUID, and image values
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnClearValues_Click(object sender, EventArgs e)
        {
            var row = (DemoTable)bsDemoData.Current;

            if(row != null)
            {
                row.XmlValue = null;
                row.GuidValue = null;
                row.ImageValue = null;

                this.bsDemoData_CurrentChanged(sender, e);
            }
        }
        #endregion
    }
}
