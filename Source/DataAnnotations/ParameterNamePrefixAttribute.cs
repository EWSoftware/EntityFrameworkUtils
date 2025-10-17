//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : ParameterNamePrefixAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/25/2024
//
// This file contains an attribute used to define a common stored procedure name prefix on the data context
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/25/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to define a common stored procedure parameter name prefix that will be used
    /// for all stored procedures called by the data context extension methods.  It should be applied to the
    /// data context class.
    /// </summary>
    /// <remarks>As an example, if set to "param" and an entity property name is <c>AccountKey</c>, the
    /// stored procedure parameter name will be set to <c>@paramAccountKey</c>.  If not defined on a data context
    /// the parameter will be named after the property (<c>@AccountKey</c> in the preceding example).</remarks>
    /// <example>
    /// <code language="cs">
    /// [ParameterNamePrefix("param")]
    /// public sealed class DemoDatabaseDataContext : DbContext
    /// {
    ///     ... Data context definition ...
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ParameterNamePrefixAttribute : Attribute
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the parameter name prefix that will be applied to all stored
        /// procedure parameters.
        /// </summary>
        /// <value>As an example, if set to "param" and an entity property name is <c>AccountKey</c>, the
        /// stored procedure parameter name will be set to <c>@paramAccountKey</c>.  If not defined on a data
        /// context the parameter will be named after the property (<c>@AccountKey</c> in the preceding
        /// example).</value>
        public string Prefix { get; }

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given parameter name prefix
        /// </summary>
        /// <param name="prefix">The common parameter name prefix to use</param>
        public ParameterNamePrefixAttribute(string prefix)
        {
            this.Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }
        #endregion
    }
}
