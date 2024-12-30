//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : RelationshipInfo.cs
// Author  : Eric Woodruff
// Updated : 12/29/2024
//
// This file contains a class used to contain entity relationship information from a DBML file
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/29/2024  EFW  Created the code
//===============================================================================================================

namespace DbmlToEntityFrameworkConverter
{
    /// <summary>
    /// This class is used to contain entity relationship information from a DBML file
    /// </summary>
    internal sealed class RelationshipInfo
    {
        /// <summary>
        /// The relationship name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The member name
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// The relationship type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// True if this is a foreign key relationship, false if not
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// This key in this type
        /// </summary>
        public string? ThisKey { get; set; }
        
        /// <summary>
        /// The key in the other type
        /// </summary>
        public string? OtherKey { get; set; }
        
        /// <summary>
        /// The relationship's cardinality
        /// </summary>
        public string? Cardinality { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="relationship">The element from which to get the relationship info</param>
        public RelationshipInfo(XElement relationship)
        {
            this.Name = relationship.Attribute("Name")!.Value;
            this.Member = relationship.Attribute("Member")!.Value;
            this.Type = relationship.Attribute("Type")!.Value;
            this.ThisKey = relationship.Attribute("ThisKey")?.Value;
            this.OtherKey = relationship.Attribute("OtherKey")?.Value;
            this.Cardinality = relationship.Attribute("Cardinality")?.Value;

            if(Boolean.TryParse(relationship.Attribute("IsForeignKey")?.Value, out var isForeignKey))
                this.IsForeignKey = isForeignKey;
        }
    }
}
