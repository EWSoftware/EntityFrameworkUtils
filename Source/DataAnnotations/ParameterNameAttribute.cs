//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : ParameterNameAttribute.cs
// Author  : Eric Woodruff
// Updated : 10/17/2025
//
// This file contains an attribute used to define a stored procedure parameter name on an entity property
//
//    Date     Who  Comments
// ==============================================================================================================
// 10/17/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to define a stored procedure parameter name that will be used for an entity
    /// property.
    /// </summary>
    /// <remarks>If specified, this will take precedence over a <see cref="ColumnAttribute"/> allowing for
    /// a different column name and stored procedure parameter name.</remarks>
    /// <example>
    /// <code language="cs">
    /// public sealed class CaseInformation
    /// {
    ///     // The name in the result set contains a space.  For the parameter
    ///     // name, it uses an underscore.
    ///     [Column("Case Number"), ParameterName("Case_Number")]
    ///     public string CaseNumber { get; set; }
    ///     .
    ///     .
    ///     .
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ParameterNameAttribute : Attribute
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the parameter name that will used
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given parameter name
        /// </summary>
        /// <param name="name">The parameter name to use</param>
        public ParameterNameAttribute(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        #endregion
    }
}
