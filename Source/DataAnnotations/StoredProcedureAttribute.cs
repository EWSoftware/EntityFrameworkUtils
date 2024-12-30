//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : StoredProcedureAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/28/2024
//
// This file contains an abstract base class used to define stored procedure attributes applied to entity types
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/24/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This class serves as the base class for stored procedure attributes applied to entity types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class StoredProcedureAttribute : Attribute
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the name of the stored procedure to execute when loading entities
        /// </summary>
        public string StoredProcedureName { get; }

        /// <summary>
        /// This can be used to define an optional parameter name prefix that will be applied to all stored
        /// procedure parameters.
        /// </summary>
        /// <value>As an example, if set to "param" and an entity property name is <c>AccountKey</c>, the
        /// stored procedure parameter name will be set to <c>@paramAccountKey</c>.</value>
        /// <remarks>This will override any prefix defined on the data context.  If set to an empty string it
        /// will effectively remove any defined prefix on the data context.</remarks>
        public string? ParameterNamePrefix { get; set; }

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given stored procedure name
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure name used to load entities</param>
        internal StoredProcedureAttribute(string storedProcedureName)
        {
            this.StoredProcedureName = storedProcedureName ?? throw new ArgumentNullException(nameof(storedProcedureName));
        }
        #endregion
    }
}
