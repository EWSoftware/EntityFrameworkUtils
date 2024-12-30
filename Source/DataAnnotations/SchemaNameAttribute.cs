//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : SchemaNameAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/28/2024
//
// This file contains an attribute used to define a common schema name on the data context
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/25/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to define a common schema name that will be used for all stored procedures
    /// called by the data context extension methods.  It should be applied to the data context class.
    /// </summary>
    /// <remarks>When a stored procedure name defined in an attribute already contains a schema name, it is
    /// returned as is.  If it does not and the data context has this attribute applied to it, the schema name
    /// from it is added to the stored procedure name.  If not, the stored procedure name is returned as is.</remarks>
    /// <example>
    /// <code language="cs">
    /// [SchemaName("Demo")]
    /// public sealed class DemoDatabaseDataContext : DbContext
    /// {
    ///     ... Data context definition ...
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SchemaNameAttribute : Attribute
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the schema name that will be applied to all stored procedure names
        /// </summary>
        /// <value>As an example, if set to <c>Financial</c> and an entity stored procedure name is
        /// <c>GetAccounts</c>, the stored procedure called will be <c>Financial.GetAccounts</c>.</value>
        public string SchemaName { get; }

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given schema name
        /// </summary>
        /// <param name="schemaName">The common schema name to use</param>
        public SchemaNameAttribute(string schemaName)
        {
            this.SchemaName = schemaName ?? throw new ArgumentNullException(nameof(schemaName));
        }
        #endregion
    }
}
