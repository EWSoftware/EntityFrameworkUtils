//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : InsertEntityStoredProcedureAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/25/2024
//
// This file contains an attribute used to specify the stored procedure used to insert entities for the
// associated type.
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/25/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to specify the stored procedure used to insert entities for the associated type
    /// </summary>
    /// <remarks>The stored procedure must have one or more parameters representing all of the properties
    /// on the entity type except those marked with an <see cref="IgnoreAttribute"/> for inserts.  It should
    /// not return a value or a result set.  Parameters related to properties that are part of the primary key
    /// or are marked with the <see cref="TimestampAttribute"/> are defined as input/out parameters.  All
    /// other parameters are input only.</remarks>
    /// <inheritdoc cref="LoadByKeyStoredProcedureAttribute" path="example" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class InsertEntityStoredProcedureAttribute : StoredProcedureAttribute
    {
        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given stored procedure name
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure name used to insert entities</param>
        public InsertEntityStoredProcedureAttribute(string storedProcedureName) : base(storedProcedureName)
        {
        }
        #endregion
    }
}
