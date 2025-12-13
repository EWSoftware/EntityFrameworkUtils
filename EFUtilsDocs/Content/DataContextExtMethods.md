---
uid: 9ce27e28-5613-49fb-a98c-38c289f7905e
alt-uid: DataContextExtMethods
title: General Data Context Extension Methods
---

This topic describes the general data context extension methods.

<autoOutline excludeRelatedTopics="true" lead="none" />


## NoTracking

This extension method is used to turn off query tracking in a data context when constructed or
later.  This can improve performance if all you are doing is reading data without any updates.  It can be chained
together with the `Open` extension method described below to open the connection at the
same time.


``` csharp
using var dataContext = new MyDbContext().NoTracking();

... Execute commands ...

using var dataContext = new MyDbContext().NoTracking().Open();

... Execute commands ...
				
```


## Open / OpenAsync

This extension method and its asynchronous counterpart are used to open a connection on a data
context when constructed or later.  It can be chained together with the `NoTracking`
extension method described above to turn off change tracking at the same time.


``` csharp
using var dataContext = new MyDbContext().Open();

... Execute commands ...

using var dataContext = new MyDbContext().NoTracking().Open();

... Execute commands ...

using var dataContext = await new MyDbContext().NoTracking().OpenAsync(cts.Token);

... Execute commands ...

				
```


## HasChanges

This extension method can be used to see if a data context has any unsaved changes.


``` cs
if(dataContext.HasChanges())
    dataContext.SaveChanges();
				
```


## GetCommand

This extension method is used to get a command object in a state ready to execute the specified
command text on the given data context.  The command will be associated with the connection from the data context
instance on which this method is called with the command timeout set to the value from the data context if one
has been set.  The command text can be a stored procedure with or without parameters or a literal SQL statement.
The returned command is ready to execute or modify further with parameters.  This is useful for situations in
which a `SqlDataReader` is needed, you need more control when executing a stored
procedure, or the stored procedure returns multiple result sets.


``` cs
using var dataContext = new MyDbContext().NoTracking();

// Set up the daily schedule command and data adapter
var cmdDailySchedule = dc.GetCommand("spDailyScheduleInfo",
    new SqlParameter("@paramBidGroup", SqlDbType.Char, 4),
    new SqlParameter("@paramDate", SqlDbType.DateTime));

var daDailySchedule = new SqlDataAdapter(cmdDailySchedule);
				
```


## CheckAndUpdateKeys

This extension method is used to check for null keys or incomplete rows added by controls and
either remove them or update them based on the passed in delegate methods.  In certain cases, some list controls
like data grids add a temporary row for new additions.  If the row is left without saving changes or the changes
are cancelled, the row is not always removed and still exists when changes are saved.  Because it has null keys
or missing values, it should not be kept.  This extension method can be used to find such rows and remove them.
It can also be used to update keys or other fields in new or existing rows if necessary.


The `nullKeyCheck` function delegate is used to check for these incomplete
rows.  If it returns true, the row is assumed to be an empty row and is detached.  If it returns false or is not
specified, the `updateKeys` action delegate is called to update keys or other fields as
needed.  If one is not specified, no further action is taken.


``` csharp
// If a list control added a row but left it incomplete, delete it.  If not, update the parent key.
dataContext.CheckAndUpdateKeys<ChildTable>(
    (entity, state) => state == EntityState.Added && entity.GroupId == null,
    (entity, state) => entity.ParentId = parent.ParentId);
				
```


## GetMethodInfo

This extension method is used within stored procedure data context methods to get the method
information for the calling method.  This information is used to get the parameter names and data types for use
in the stored procedure method call.  This extension method can be used to obtain the necessary method
information whether the stored procedure method is called synchronously or asynchronously.


``` csharp
[MethodStoredProcedure("spStateCodeAddUpdate")]
public int AddOrUpdateStateCode(string? oldState, string? state, string? stateDesc)
{
    return this.ExecuteMethodNonQuery(this.GetMethodInfo(), oldState, state, stateDesc).ReturnValue;
}
				
```


## See Also


**Other Resources**  
[](@48c2006f-d738-40a1-a486-d53fbdf7208c)  
