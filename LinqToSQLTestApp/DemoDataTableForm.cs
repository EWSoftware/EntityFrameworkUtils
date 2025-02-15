//===============================================================================================================
// System  : LINQ to SQL Test App
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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using LinqToSQLTestApp.Database;

namespace LinqToSQLTestApp
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

            // This table has an identity column
            bsDemoData.DataSource = dc.spDemoTableData();
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
            dc.SubmitChanges();
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
                    using(var ms = new MemoryStream(row.ImageValue.ToArray()))
                    {
                        pbImage.Image = Image.FromStream(ms);
                    }
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

                using(Graphics g = Graphics.FromImage(bmp))
                {
                    for(int y = 0; y < 100; y += 20)
                    {
                        for(int x = 0; x < 100; x += 20)
                        {
                            Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                            using(Brush brush = new SolidBrush(randomColor))
                            {
                                g.FillRectangle(brush, x, y, 20, 20);
                            }
                        }
                    }
                }

                using(var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    row.ImageValue = ms.ToArray();
                }

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
