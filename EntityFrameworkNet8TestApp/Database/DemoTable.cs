// Code generated by DBML To Entity Framework Converter tool
// https://github.com/EWSoftware/EntityFrameworkUtils

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

using EWSoftware.EntityFramework;
using EWSoftware.EntityFramework.DataAnnotations;

namespace EntityFrameworkNet8TestApp.Database
{
    [LoadAllStoredProcedure("spDemoTableData"), LoadByKeyStoredProcedure("spDemoTableInfo"),
      InsertEntityStoredProcedure("spDemoTableAddUpdate"), UpdateEntityStoredProcedure("spDemoTableAddUpdate"),
      DeleteEntityStoredProcedure("spDemoTableDelete")]
    public sealed class DemoTable : ChangeTrackingEntity
    {
        [Key]
        public int ListKey
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }

        public string Label
        {
            get;
            set => this.SetWithNotify(value, ref field);
        } = String.Empty;

        public string TextValue
        {
            get;
            set => this.SetWithNotify(value, ref field);
        } = String.Empty;

        public DateTime DateValue
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }

        public bool BoolValue
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }

        public decimal DecimalValue
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }

        /// <summary>
        /// Entity Framework does not support XML properties in entities so we need to ignore it.  The stored
        /// procedure extension methods do support them so we don't need an additional property to handle it.
        /// </summary>
        [NotMapped]
        public XElement? XmlValue
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }

        public Guid? GuidValue
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }

        public byte[]? ImageValue
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }

        [Ignore(true, true)]
        public byte[] LastModified
        {
            get;
            set => this.SetWithNotify(value, ref field);
        } = [];
    }
}
