//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : PropertyInfo.cs
// Author  : Eric Woodruff
// Updated : 12/16/2024
//
// This file contains a class used to contain entity type property information from a DBML file
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/27/2024  EFW  Created the code
//===============================================================================================================

namespace DbmlToEntityFrameworkConverter
{
    /// <summary>
    /// This class is used to contain entity type property information from a DBML file
    /// </summary>
    internal sealed class PropertyInfo
    {
        /// <summary>
        /// This is used to convert types in the DBML file to their equivalent C# type
        /// </summary>
        internal static Dictionary<string, string> PropertyTypes { get; } = new()
        {
            { "System.Boolean", "bool" },
            { "System.Byte", "byte" },
            { "System.Byte[]", "byte[]" },
            { "System.Char", "char" },
            { "System.DateTime", "DateTime" },
            { "System.DateTimeOffset", "DateTimeOffset" },
            { "System.Decimal", "decimal" },
            { "System.Double", "double" },
            { "System.Guid", "Guid" },
            { "System.Int16", "short" },
            { "System.Int32", "int" },
            { "System.Int64", "long" },
            { "System.Object", "object" },
            { "System.SByte", "sbyte" },
            { "System.Single", "float" },
            { "System.String", "string" },
            { "System.UInt16", "ushort" },
            { "System.UInt32", "uint" },
            { "System.UInt64", "ulong" },
            { "System.TimeSpan", "TimeSpan" },
            { "System.Data.Linq.Binary", "byte[]" },
            { "System.Xml.Linq.XDocument", "System.Xml.Linq.XDocument" },
            { "System.Xml.Linq.XElement", "System.Xml.Linq.XElement" }
        };

        /// <summary>
        /// The property name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The backing field name if used
        /// </summary>
        public string BackingFieldName { get; set; }

        /// <summary>
        /// The property type
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// True if the property is a primary key field, false if not
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// True if the property is nullable, false if not
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// The default value to use if enforcing nullable reference types
        /// </summary>
        public string? ReferenceTypeDefault { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyInfo">The element from which to get the property information</param>
        public PropertyInfo(XElement propertyInfo)
        {
            this.PropertyName = propertyInfo.Attribute("Name")!.Value;
            this.BackingFieldName = $"_{Char.ToLowerInvariant(this.PropertyName[0])}{this.PropertyName[1..]}";
            this.PropertyType = PropertyTypes[propertyInfo.Attribute("Type")!.Value];
            this.IsPrimaryKey = (bool?)propertyInfo.Attribute("IsPrimaryKey") ?? false;
            this.IsNullable = (bool?)propertyInfo.Attribute("CanBeNull") ?? true;

            if(!this.IsNullable)
            {
                switch(this.PropertyType)
                {
                    case "string":
                        this.ReferenceTypeDefault = "String.Empty";
                        break;

                    case "byte[]":
                        this.ReferenceTypeDefault = "[]";
                        break;

                    case "System.Xml.Linq.XDocument":
                        this.ReferenceTypeDefault = "new XDocument()";
                        break;

                    case "System.Xml.Linq.XElement":
                        this.ReferenceTypeDefault = "new XElement(\"Empty\")";
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
