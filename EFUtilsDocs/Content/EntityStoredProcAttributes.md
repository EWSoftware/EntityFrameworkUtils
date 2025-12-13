---
uid: 869efb41-378c-426e-a84a-b014b72d1772
alt-uid: EntityStoredProcAttributes
title: Entity Stored Procedure Attributes
---

This topic describes the general and stored procedure data annotation attributes that can be applied
to entities and their properties.


> [!NOTE]
> The load all, load by key, insert, update, and delete stored procedure attributes are all derived
> from a common base class ([](@T:EWSoftware.EntityFramework.DataAnnotations.StoredProcedureAttribute)).
> All take a stored procedure name as the constructor parameter and have an optional
> `ParameterNamePrefix` property that allows you to specify a common parameter name prefix
> applied to its parameters.  If specified, it will override any corresponding parameter name prefix defined on the
> data context.  If set to an empty string, it will effectively remove the prefix from parameters for the stored
> procedure even if one is defined on the data context.
> 
>

<autoOutline excludeRelatedTopics="true" lead="none" />


## NeverTrackAttribute

If change tracking is enabled on a data context, the stored procedure extension methods that load,
insert, update, and delete entities will automatically adjust the entity state with the change tracker.  This
attribute is used to mark an entity type as never tracked so that the stored procedure extension methods do not
alter the change tracking state for it.  This is useful for entities that are not modified and/or do not
implement property change notification and thus do not require change tracking.


``` cs
// Used to display state codes in a drop-down list.  This is never modified and does
// not need change tracking.
[NeverTrack]
public sealed class StateCode
{
    // The state code
    public string StateCode { get; set; }
    
    // The state description
    public string StateDesc { get; set; }
}
				
```


## ParameterNameAttribute

Similar to the `ColumnAttribute`, a `ParameterNameAttribute`
can be applied to a property to specify the stored procedure parameter name to use for it.  If specified along
with a `ColumnAttribute`, the `ParameterNameAttribute` will take
precedence.  This is useful when the name in the result set differs from the parameter name as shown in the
example below.


``` csharp
public sealed class CaseInformation
{
    // The name in the result set contains a space.  For the parameter name,
    // it uses an underscore.
    [Column("Case Number"), ParameterName("Case_Number")]
    public string CaseNumber { get; set; }
    .
    .
    .
}
				
```


## IgnoreAttribute

Unlike entities loaded by Entity Framework, the stored procedure extension methods do not require
a one to one mapping between the result set columns and entity properties.  As such, additional properties can
be added to the entities that do not appear in the result set and vice versa.  This attribute can be applied to
entity properties to indicate whether or not they should be included for inserts and/or updates.  It takes two
Boolean parameters, the first for whether or not to include it for inserts, and the second for updates.
Typically, both parameters are set to false but it is possible to set one or the other to true to include the
property for insert or updates if required.


The attribute will usually be applied to properties in the type but can be applied to the class
or structure with a property name specified to ignore inherited properties that you do not have direct access to
and should not be considered for inserts and updates.  The attribute can be applied multiple times if there are
multiple properties to ignore.


``` csharp
[LoadAllStoredProcedure("spStateCodes"), InsertStoredProcedure("spStateCodeAddUpdate"),
  UpdateStoredProcedure("spStateCodeAddUpdate"), DeleteStoredProcedure("spStateCodeDelete")]
public sealed class StateCode : ChangeTrackingEntity
{
    // The state code
    [Key]
    public string State
    {
        get;
        set => this.SetWithNotify(value, ref field);
    } = String.Empty;

    // The state description
    public string StateDesc
    {
        get;
        set => this.SetWithNotify(value, ref field);
    } = String.Empty;

    // True if in use and cannot be deleted, false if not.  This is an extra column in
    // the load all stored procedure and we'll ignore it for inserts and updates.
    [Ignore(true, true)]
    public bool IsInUse
    {
        get;
        set => this.SetWithNotify(value, ref field);
    }
}

// Example Load All usage:
using var dataContext = new DemoDatabaseDataContext();

var stateCodes = dc.LoadAll<StateCode>().ToList();
				
```

``` csharp
// This entity has a HasErrors property inherited from ObservableValidator that needs to be
// ignored for inserts and updates.  Since we don't have direct access to it, we use the
// attribute on the entity type and specify the property name as well.
[LoadAllStoredProcedure("spStateCodes"), InsertStoredProcedure("spStateCodeAddUpdate"),
  UpdateStoredProcedure("spStateCodeAddUpdate"), DeleteStoredProcedure("spStateCodeDelete"),
  Ignore(true, true, PropertyName = nameof(HasErrors))]
public sealed class StateCode : ObservableValidator
{
    ...
}
				
```
<!-- StoredProcedureAttribute is the base -->


## LoadAllStoredProcedureAttribute

This attribute can be applied to an entity type to define a stored procedure used to load them that
takes no parameters.  The 
[](@M:EWSoftware.EntityFramework.DatabaseExtensions.LoadAll``1(Microsoft.EntityFrameworkCore.DbContext)){prefer-overload="true"}
extension method uses this attribute to determine the stored procedure it should call.  See the
`IgnoreAttribute` section above for an example.


If the attribute is omitted, the stored procedure name is assumed to be the same as the entity type
name without the [](@P:EWSoftware.EntityFramework.DatabaseExtensions.ResultSetSuffix)
property value.



## LoadByKeyStoredProcedureAttribute

This attribute can be applied to an entity type to define a stored procedure used to load them
using the values for the key properties defined on the type.  The 
[](@M:EWSoftware.EntityFramework.DatabaseExtensions.LoadByKey``1(Microsoft.EntityFrameworkCore.DbContext,System.Object[])){prefer-overload="true"}
extension method uses this attribute to determine the stored procedure it should call.  The `PrimaryKey`
attribute on the type or the `Key` attributes on the properties are used to determine the
key for the type.  There must be one stored procedure parameter for each key property on the type.


If the attribute is omitted, the stored procedure name is assumed to be the same as the entity type
name without the [](@P:EWSoftware.EntityFramework.DatabaseExtensions.ResultSetSuffix)
property value.


``` csharp
[LoadByKeyStoredProcedure("spProductInfo"), InsertEntityStoredProcedure("spProductAddUpdate"),
  UpdateEntityStoredProcedure("spProductAddUpdate"), DeleteEntityStoredProcedure("spProductDelete")]
public sealed class ProductInfo : ChangeTrackingEntity
{
    // The primary key
    [Key]
    public int ProductID
    {
        get;
        set => this.SetWithNotify(value, ref field);
    }
    
    // Product name
    public string? ProductName
    {
        get;
        set => this.SetWithNotify(value, ref field);
    }

    ...
}

// Example Load By Key usage:
using var dataContext = new DemoDatabaseDataContext();

var productInfo = dc.LoadByKey<ProductInfo>(productID).Single();

// Add a new entity
var newProduct = new ProductInfo { ProductName = "New Product" };

dataContext.InsertEntity(newProduct);

// Update an existing entity
productInfo.ProductName = "Updated Product";

dataContext.UpdateEntity(productInfo);

// Delete an entity
dataContext.DeleteEntity(productInfo);

// Since change tracking is enabled, we could also have just submitted the changes:
// dataContext.SubmitChanges<ProductInfo>();
				
```


## InsertEntityStoredProcedureAttribute

This attribute is used to specify the stored procedure used to insert entities for the associated
type.  The stored procedure should have one or more parameters representing all of the properties
on the entity type except those marked with an `IgnoreAttribute` for inserts.  It should
not return a value or a result set.  Parameters related to properties that are part of the primary key
or are marked with the `TimestampAttribute` are defined as input/out parameters.  All
other parameters are input only.  See the `LoadByKeyAttribute` section above for an example.



## UpdateEntityStoredProcedureAttribute

This attribute is used to specify the stored procedure used to update entities for the associated
type.  The stored procedure should have one or more parameters representing all of the properties
on the entity type except those marked with an `IgnoreAttribute` for updates.  It should
not return a value or a result set.  Parameters marked with the `TimestampAttribute` are
defined as input/out parameters.  All other parameters are input only.  See the `LoadByKeyAttribute`
section above for an example.



## DeleteEntityStoredProcedureAttribute

This attribute is used to specify the stored procedure used to delete entities for the associated
type.  The stored procedure should have one or more parameters representing the key columns on the entity type
identified with a `PrimaryKeyAttribute` or one or more properties with a
`KeyAttribute` or defined by the data context.  It should not return a value or a result
set.  All parameters are input only.  See the `LoadByKeyAttribute` section above for an
example.



## See Also


**Other Resources**  
[](@48c2006f-d738-40a1-a486-d53fbdf7208c)  
