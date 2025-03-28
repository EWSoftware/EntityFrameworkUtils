// Code generated by DBML To Entity Framework Converter tool
// https://github.com/EWSoftware/EntityFrameworkUtils
//
// This code will need to be reviewed and tested to fix up any issues.

// TODO: Add or remove using statements as needed
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using EWSoftware.EntityFramework;
using EWSoftware.EntityFramework.DataAnnotations;

namespace EntityFrameworkNet8TestApp.Database
{
    [InsertEntityStoredProcedure("spDemoTableAddUpdate"), UpdateEntityStoredProcedure("spDemoTableAddUpdate"), DeleteEntityStoredProcedure("spDemoTableDelete")]
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

        public System.Xml.Linq.XElement? XmlValue
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
