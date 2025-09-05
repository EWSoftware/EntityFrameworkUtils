//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : MethodStoredProcedureAttribute.cs
// Author  : Eric Woodruff
// Updated : 07/14/2025
//
// This file contains an attribute used to specify the stored procedure executed by a method on a data context
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/24/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to specify the stored procedure executed by a method on a data context
    /// </summary>
    /// <remarks>This attribute is optional.  If not specified on a data context stored procedure method, the
    /// name of the stored procedure is assumed to be the same as the data context method's name.  If the method
    /// name ends with the value of the <see cref="DatabaseExtensions.AsyncMethodSuffix"/> property, the
    /// suffix will be removed from the method name to obtain the stored procedure name.</remarks>
    /// <example>
    /// <code language="cs">
    /// // Use the attribute to specify the stored procedure name when it differs from the method name
    /// [MethodStoredProcedure("spStateCodeAddUpdate")]
    /// public int AddOrUpdateStateCode(string? oldState, string? state, string? stateDesc)
    /// {
    ///     return this.ExecuteMethodNonQuery(this.GetMethodInfo(), oldState, state, stateDesc).ReturnValue;
    /// }
    /// 
    /// [MethodStoredProcedure("spStateCodeDelete")]
    /// public int DeleteStateCode(string? state)
    /// {
    ///     return this.ExecuteMethodNonQuery(this.GetMethodInfo(), state).ReturnValue;
    /// }
    /// 
    /// [MethodStoredProcedure("spProductSearch")]
    /// public IEnumerable&lt;ProductSearchResult&gt; SearchForProducts(string? productName, string? categoryName,
    ///     string? companyName)
    /// {
    ///     return this.ExecuteMethodQuery&lt;ProductSearchResult&gt;(this.GetMethodInfo(),
    ///         productName, categoryName, companyName);
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MethodStoredProcedureAttribute : StoredProcedureAttribute
    {
        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given stored procedure name
        /// </summary>
        /// <param name="storedProcedureName">The name of the method's stored procedure</param>
        public MethodStoredProcedureAttribute(string storedProcedureName) : base(storedProcedureName)
        {
        }
        #endregion
    }
}
