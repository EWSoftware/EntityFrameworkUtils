//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : LoadByKeyStoredProcedureAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/28/2024
//
// This file contains an attribute used to specify the stored procedure used to load one or more entities for
// the associated type that have a specified key value.
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/22/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to specify the stored procedure used to load one or more entities for the
    /// associated type that have a specified key value.
    /// </summary>
    /// <remarks>The stored procedure must have one or more parameters representing the key columns on the
    /// entity type identified with a <c>PrimaryKeyAttribute</c> or one or more properties with a
    /// <c>KeyAttribute</c> or defined by the data context.  All parameters are input only.</remarks>
    /// <example>
    /// <code language="csharp">
    /// [LoadByKeyStoredProcedure("spProductInfo"), InsertEntityStoredProcedure("spProductAddUpdate"),
    ///   UpdateEntityStoredProcedure("spProductAddUpdate"), DeleteEntityStoredProcedure("spProductDelete")]
    /// public sealed class ProductInfo : ChangeTrackingEntity
    /// {
    ///     // The primary key
    ///     [Key]
    ///     public int ProductID
    ///     {
    ///     get;
    ///     set => this.SetWithNotify(value, ref field);
    ///     }
    /// 
    ///     // Product name
    ///     public string? ProductName
    ///     {
    ///     get;
    ///     set => this.SetWithNotify(value, ref field);
    ///     }
    /// 
    ///     ...
    /// }
    /// 
    /// // Example Load By Key usage:
    /// using var dataContext = new DemoDatabaseDataContext();
    /// 
    /// var productInfo = dc.LoadByKey&lt;ProductInfo&gt;(productID).Single();
    /// 
    /// // Add a new entity
    /// var newProduct = new ProductInfo { ProductName = "New Product" };
    /// 
    /// dataContext.InsertEntity(newProduct);
    /// 
    /// // Update an existing entity
    /// productInfo.ProductName = "Updated Product";
    /// 
    /// dataContext.UpdateEntity(productInfo);
    /// 
    /// // Delete an entity
    /// dataContext.DeleteEntity(productInfo);
    /// 
    /// // Since change tracking is enabled, we could also have just submitted the changes:
    /// // dataContext.SubmitChanges&lt;ProductInfo&gt;();
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class LoadByKeyStoredProcedureAttribute : StoredProcedureAttribute
    {
        #region Constructor
        //=====================================================================

        /// <summary>
        /// Initialize a new instance of the attribute using the given stored procedure name
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure name used to load entities</param>
        public LoadByKeyStoredProcedureAttribute(string storedProcedureName) : base(storedProcedureName)
        {
        }
        #endregion
    }
}
