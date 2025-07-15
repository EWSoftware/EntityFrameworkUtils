//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : LoadAllStoredProcedureAttribute.cs
// Author  : Eric Woodruff
// Updated : 07/14/2025
//
// This file contains an attribute used to specify the stored procedure used to load all entities for the
// associated type.
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/22/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to specify the stored procedure used to load all entities for the associated type
    /// </summary>
    /// <remarks>If the stored procedure name matches the entity type name exactly or ends with the value of the
    /// <see cref="DatabaseExtensions.ResultSetSuffix"/> property, the attribute can be omitted.</remarks>
    /// <example>
    /// <code language="cs">
    /// [LoadAllStoredProcedure("spStateCodes")]
    /// public sealed class StateCode
    /// {
    ///     // The state code
    ///     public string State { get; set; }
    /// 
    ///     // The state description
    ///     public string StateDesc { get; set; }
    /// }
    ///
    /// // Example usage:
    /// using var dataContext = new DemoDatabaseDataContext().NoTracking();
    ///
    /// var stateCodes = dc.LoadAll&lt;StateCode&gt;().ToList();
    /// </code></example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class LoadAllStoredProcedureAttribute : StoredProcedureAttribute
    {
        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given stored procedure name
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure should not have any parameters or only
        /// parameters with acceptable default values in order to return all rows.</param>
        public LoadAllStoredProcedureAttribute(string storedProcedureName) : base(storedProcedureName)
        {
        }
        #endregion
    }
}
