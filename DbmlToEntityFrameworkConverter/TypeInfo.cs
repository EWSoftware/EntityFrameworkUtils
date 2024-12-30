//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : TypeInfo.cs
// Author  : Eric Woodruff
// Updated : 12/29/2024
//
// This file contains a class used to contain entity type information from a DBML file
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/27/2024  EFW  Created the code
//===============================================================================================================

namespace DbmlToEntityFrameworkConverter
{
    /// <summary>
    /// This class is used to contain entity type information from a DBML file
    /// </summary>
    internal sealed class TypeInfo
    {
        /// <summary>
        /// The type name
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// The type's insert function element if defined
        /// </summary>
        public XElement? InsertFunction { get; set; }

        /// <summary>
        /// The type's update function element if defined
        /// </summary>
        public XElement? UpdateFunction { get; set; }

        /// <summary>
        /// The type's delete function element if defined
        /// </summary>
        public XElement? DeleteFunction { get; set; }

        /// <summary>
        /// The properties on this type
        /// </summary>
        public List<PropertyInfo> Properties { get; set; } = [];

        /// <summary>
        /// The relationships for this type
        /// </summary>
        public List<RelationshipInfo> Relationships { get; set; } = [];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <param name="xmlNamespace">The XML namespace</param>
        /// <param name="typeElement">The element from which to get the type info</param>
        public TypeInfo(XNamespace xmlNamespace, XElement typeElement)
        {
            this.TypeName = typeElement.Attribute("Name")!.Value;
            this.InsertFunction = typeElement.Parent!.Element(xmlNamespace + "InsertFunction");
            this.UpdateFunction = typeElement.Parent.Element(xmlNamespace + "UpdateFunction");
            this.DeleteFunction = typeElement.Parent.Element(xmlNamespace + "DeleteFunction");

            foreach(var property in typeElement.Elements(xmlNamespace + "Column"))
                this.Properties.Add(new PropertyInfo(property));

            foreach(var relationship in typeElement.Elements(xmlNamespace + "Association"))
                this.Relationships.Add(new RelationshipInfo(relationship));
        }
    }
}
