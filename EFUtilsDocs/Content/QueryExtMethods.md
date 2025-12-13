---
uid: fcc7cc28-ad7d-463d-84aa-723766215811
alt-uid: QueryExtMethods
title: Query Data Context Extension Methods
---
<!-- Ignore Spelling: se -->

This topic describes the query related data context extension methods that work in conjunction with
the [](@869efb41-378c-426e-a84a-b014b72d1772) to load, add, update, or delete rows.


> [!NOTE]
> In all of the extension methods below, if the data context connection is not in an open state, it
> is opened temporarily while performing the action and closed again afterwards.
> 
> 
> If change tracking is enabled on the data context, any loaded
> entities will be added to the change tracker.  Likewise, changes to the entities will be tracked and their
> state updated accordingly when added, updated, or deleted when submitting changes to the data context.  If change
> tracking is not enabled on the data context or the entity is marked with the `NeverTrackAttribute`,
> they will not be tracked.
> 
> 
> Asynchronous versions of all the methods are also available.
> 
>

<autoOutline excludeRelatedTopics="true" lead="none" />


## LoadAll / LoadAllAsync

This extension method is used to load all entities of the given type using a stored procedure
defined on the entity type.  The stored procedure name is determined by looking for the
`LoadAllStoredProcedureAttribute` on the entity type.  The stored procedure should not
have any parameters or only parameters with acceptable default values in order to return all rows.


If the attribute is omitted from the entity type, the stored procedure name is assumed to be the
same as the entity type name without the [](@P:EWSoftware.EntityFramework.DatabaseExtensions.ResultSetSuffix)
property value.


``` cs
using var dataContext = new MyDbContext();

var watchList = dataContext.LoadAll<WatchList>().ToList();
				
```


## LoadByKey / LoadByKeyAsync

This extension method is used to load all entities of the given type using a stored procedure
defined on the entity type using the given key value(s).  The stored procedure name is determined by looking for
the `LoadByKeyStoredProcedureAttribute` on the entity type.  The stored procedure should
have a parameter for each of the passed key values.  The parameter order typically matches the declared key order
on the entity or the parameter order of the stored procedure but does not have to.


If the attribute is omitted from the entity type, the stored procedure name is assumed to be the
same as the entity type name without the [](@P:EWSoftware.EntityFramework.DatabaseExtensions.ResultSetSuffix)
property value.


``` cs
using var dataContext = new MyDbContext();

var assetInfo = dataContext.LoadByKey<Asset>(assetKey).Single();
				
```


## InsertEntity / InsertEntityAsync

This extension method is used to insert the given entity using a stored procedure defined on the
entity type.  The stored procedure name is determined by looking for the
`InsertEntityStoredProcedureAttribute` on the entity type.  The stored procedure must
have parameters for each of the entity properties except those marked with the
`IgnoreAttribute` for inserts.  It should not return a value or a result set.  Parameters
related to the primary key (single column, integer only) or marked with the `TimestampAttribute`
are defined as input/out parameters.  All other parameters are input only.


``` cs
using var dataContext = new MyDbContext();

if(watchListItem.WatchID == 0)
    dataContext.InsertEntity(watchListItem);
else
    dataContext.UpdateEntity(watchListItem);
				
```


## UpdateEntity / UpdateEntityAsync

This extension method is used to update the given entity using a stored procedure defined on the
entity type.  The stored procedure name is determined by looking for the
`UpdateEntityStoredProcedureAttribute` on the entity type.  The stored procedure must
have parameters for each of the entity properties except those marked with the
`IgnoreAttribute` for updates.  It should not return a value or a result set.  Parameters
marked with the `TimestampAttribute` are defined as input/out parameters.  All other
parameters are input only.


``` cs
using var dataContext = new MyDbContext();

if(watchListItem.WatchID == 0)
    dataContext.InsertEntity(watchListItem);
else
    dataContext.UpdateEntity(watchListItem);
				
```


## DeleteEntity / DeleteEntityAsync

This extension method is used to delete the given entity using a stored procedure defined on the
entity type.  The stored procedure name is determined by looking for the
`DeleteEntityStoredProcedureAttribute` on the entity type.  The stored procedure must have
one or more parameters representing the key columns on the entity type identified with a
`PrimaryKeyAttribute` or one or more properties with a `KeyAttribute`
or defined by the data context.  It should not return a value or a result set.  All parameters are input only.


``` cs
using var dataContext = new MyDbContext();

dataContext.DeleteEntity(watchListItem);
				
```


## SubmitChanges / SubmitChangesAsync (2 overloads each)

The `SubmitChanges` methods are used to submit all tracked add, update, and
delete changes for the given entity type.


The first overload is similar to the `SaveChanges` method on the data
context except that it only submits changes for the specified entity type.  The insert, update, and delete
stored procedures are determined by looking for the `InsertEntityStoredProcedureAttribute`,
`UpdateEntityStoredProcedureAttribute`, and `DeleteEntityStoredProcedureAttribute`
attributes on the entity type.  It will get the changed entities from the data context's change tracker and
submit them accordingly using the extension methods described above.  The state of the entities is also updated
to reflect that they are in an unchanged state after being added or updated or detached if deleted.


``` cs
if(dataContext.HasChanges())
    dataContext.SubmitEntityChanges<Account>();
				
```

The second overload allows you to specify functions that allow for custom handling of insert,
update, and delete operations for the given entity type.  This will get the changed entities from the data
context's change tracker and submit them accordingly using the given functions.  If the corresponding function
returns true, the state of the entity is updated to reflect that it is in an unchanged state after being added or
updated or detached if deleted.  If it returns false, the entity is left in its changed state and no action is
taken.  If a function delegate is not supplied for an operation, no action is taken.  The functions are passed
the entity entry from the change tracker so that they can determine what to do based on the changed state.


``` cs
if(dataContext.HasChanges())
{
    // Submit changes using stored procedure methods on the data context
    dataContext.SubmitChanges<StateCode>(
        se =>
        {
            // Insert a new state code
            dataContext.spStateCodeAddUpdate(null, se.Entity.State, se.Entity.StateDesc);
            return true;
        },
        se =>
        {
            // Update an existing state code possibly changing the key
            dataContext.spStateCodeAddUpdate((string?)se.OriginalValues[nameof(StateCode.State)],
                se.Entity.State, se.Entity.StateDesc);
            return true;
        },
        se =>
        {
            // Delete an existing state code
            dataContext.spStateCodeDelete((string?)se.OriginalValues[nameof(StateCode.State)]);
            return true;
        });
}
				
```


## See Also


**Other Resources**  
[](@48c2006f-d738-40a1-a486-d53fbdf7208c)  
