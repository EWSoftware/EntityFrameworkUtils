// Code generated by DBML To Entity Framework Converter tool
// https://github.com/EWSoftware/EntityFrameworkUtils

using System.ComponentModel.DataAnnotations;

using EWSoftware.EntityFramework;
using EWSoftware.EntityFramework.DataAnnotations;

namespace EntityFrameworkNet8TestApp.Database
{
    [LoadAllStoredProcedure("spDemoTableData"), InsertEntityStoredProcedure("spDemoTableAddUpdate"),
      UpdateEntityStoredProcedure("spDemoTableAddUpdate"), DeleteEntityStoredProcedure("spDemoTableDelete")]
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

        [Ignore(true, true)]
        public byte[] LastModified
        {
            get;
            set => this.SetWithNotify(value, ref field);
        } = [];

    }
}
