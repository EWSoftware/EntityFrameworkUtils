//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : DeleteEntityStoredProcedureAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/28/2024
//
// This file contains an attribute used to specify the stored procedure used to delete entities for the
// associated type.
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/22/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to specify the stored procedure used to delete entities for the associated type
    /// </summary>
    /// <remarks>The stored procedure must have one or more parameters representing the key columns on the
    /// entity type identified with a <c>PrimaryKeyAttribute</c> or one or more properties with a
    /// <c>KeyAttribute</c> or defined by the data context.  It should not return a value or a result set.  All
    /// parameters are input only.</remarks>
    /// <inheritdoc cref="LoadByKeyStoredProcedureAttribute" path="example" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DeleteEntityStoredProcedureAttribute : StoredProcedureAttribute
    {
        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given stored procedure name
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure name used to delete entities</param>
        public DeleteEntityStoredProcedureAttribute(string storedProcedureName) : base(storedProcedureName)
        {
        }
        #endregion
    }
}
