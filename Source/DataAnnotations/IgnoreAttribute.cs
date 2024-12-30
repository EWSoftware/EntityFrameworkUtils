//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : IgnoreAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/24/2024
//
// This file contains an attribute used to mark properties that should be ignored as a parameter for an insert
// and/or update stored procedure for an entity.
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/24/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to mark properties that should be ignored as a parameter for an insert and/or
    /// update stored procedure for an entity.
    /// </summary>
    /// <remarks>This allows additional properties not present in the result set or the underlying table being
    /// updated to be excluded when performing inserts and updates.  Typically, both parameters are set to false
    /// but it is possible to set one or the other to true to include the property for insert or updates if
    /// required.</remarks>
    ///
    /// <example>
    /// <code language="csharp">
    /// [LoadAllStoredProcedure("spStateCodes"), InsertStoredProcedure("spStateCodeAddUpdate"),
    ///   UpdateStoredProcedure("spStateCodeAddUpdate"), DeleteStoredProcedure("spStateCodeDelete")]
    /// public sealed class StateCode : ChangeTrackingEntity
    /// {
    ///     // The state code
    ///     [Key]
    ///     public string State
    ///     {
    ///         get;
    ///         set => this.SetWithNotify(value, ref field);
    ///     } = String.Empty;
    /// 
    ///     // The state description
    ///     public string StateDesc
    ///     {
    ///         get;
    ///         set => this.SetWithNotify(value, ref field);
    ///     } = String.Empty;
    /// 
    ///     // True if in use and cannot be deleted, false if not.  This is a column in the
    ///     // load all stored procedure and we'll ignore it for inserts and updates.    
    ///     [Ignore(true, true)]
    ///     public bool IsInUse
    ///     {
    ///         get;
    ///         set => this.SetWithNotify(value, ref field);
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property gets the ignored state of the property for insert stored procedures
        /// </summary>
        public bool ForInsert { get; }

        /// <summary>
        /// This read-only property gets the ignored state of the property for update stored procedures
        /// </summary>
        public bool ForUpdate { get; }

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given settings
        /// </summary>
        /// <param name="forInsert">True to ignore the property as a parameter for insert stored
        /// procedures, false to include it.</param>
        /// <param name="forUpdate">True to ignore the property as a parameter for update stored
        /// procedures, false to include it.</param>
        public IgnoreAttribute(bool forInsert, bool forUpdate)
        {
            this.ForInsert = forInsert;
            this.ForUpdate = forUpdate;
        }
        #endregion
    }
}
