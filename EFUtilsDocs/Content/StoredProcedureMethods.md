---
uid: 3070da37-6ca8-48e0-b350-618489d67bc5
alt-uid: StoredProcedureMethods
title: Stored Procedure Data Context Methods
---
<!-- Ignore Spelling: bool foreach cts -->

When executing a stored procedure that does not return a result set or does not conform to the key
field or parameter expectations of the other extension methods, you can add stored procedure methods to the
Entity Framework data context that mimic the behavior of the stored procedure methods used by LINQ to SQL data
contexts.  These are called just like normal class methods and are passed one or more parameters if needed.  The
data context method call is translated to a stored procedure call using the given parameter values.  The called
stored procedure can be a non-query that performs an action such as an insert, update, delete, or a lookup and
returns the number of rows affected, a return value, and/or output parameter values rather than a result set.
For query stored procedure methods, the return value is an enumerable list of the given entity type created from
the query's result set.


> [!NOTE]
> In the extension methods below, if the data context connection is not in an open state, it is
> opened temporarily while performing the action and closed again afterwards.
> 
> 
> If change tracking is enabled on the data context, any entities loaded by
> `ExecuteMethodQuery` will be added to the change tracker.  Likewise, changes to the
> entities will be tracked and their state updated accordingly when added, updated, or deleted when submitting
> changes to the data context.  If change tracking is not enabled on the data context or the entity is marked with
> the `NeverTrackAttribute`, they will not be tracked.
> 
>

<autoOutline excludeRelatedTopics="true" lead="none" />


## MethodStoredProcedureAttribute

This optional attribute is used to define the stored procedure name that will be called for the
data context method.  If not specified on a data context stored procedure method, the name of the stored
procedure is assumed to be the same as the data context method's name.  If the method name ends with the value of 
the [](@P:EWSoftware.EntityFramework.DatabaseExtensions.AsyncMethodSuffix)
property, the suffix will be removed from the method name to obtain the stored procedure name.


``` csharp
// For these the stored procedure name is assumed to be the same as the method name	
public int spStateCodeAddUpdate(string? oldState, string? state, string? stateDesc)
{
    return this.ExecuteMethodNonQuery(this.GetMethodInfo(), oldState, state, stateDesc).ReturnValue;
}

public int spStateCodeDelete(string? state)
{
    return this.ExecuteMethodNonQuery(this.GetMethodInfo(), state).RowsAffected;
}

public IEnumerable<spProductSearchResult> spProductSearch(string? productName, string? categoryName,
    string? companyName)
{
    return this.ExecuteMethodQuery<spProductSearchResult>(this.GetMethodInfo(), productName,
        categoryName, companyName);
}

// Or we can change the method name to something else and specify the stored
// procedure name with the attribute.
[MethodStoredProcedure("spStateCodeAddUpdate")]
public int AddOrUpdateStateCode(string? oldState, string? state, string? stateDesc)
{
    return this.ExecuteMethodNonQuery(this.GetMethodInfo(), oldState, state, stateDesc).ReturnValue;
}

[MethodStoredProcedure("spStateCodeDelete")]
public int DeleteStateCode(string? state)
{
    return this.ExecuteMethodNonQuery(this.GetMethodInfo(), state).ReturnValue;
}

[MethodStoredProcedure("spProductSearch")]
public IEnumerable<ProductSearchResult> SearchForProducts(string? productName, string? categoryName,
    string? companyName)
{
    return this.ExecuteMethodQuery<ProductSearchResult>(this.GetMethodInfo(), productName,
        categoryName, companyName);
}
				
```


## Non-Query Stored Procedure Methods

A non-query stored procedure method is one that does not return a result set.  Instead it returns
the number of rows affected, a return value, and/or output parameter values.  The data context method calls the
[](@M:EWSoftware.EntityFramework.DatabaseExtensions.ExecuteMethodNonQuery(Microsoft.EntityFrameworkCore.DbContext,System.Reflection.MethodInfo,System.Object[])){prefer-overload="true"}
extension method.  The first parameter is always a `MethodInfo` instance representing the
calling method.  Subsequent parameters, if any, are those passed to the data context method.  The order of the
parameters must match the order for the calling data context method.  Any parameters for which output values are
needed should be passed to the data context method by reference.  However, they are not passed by reference to
the extension method.


The return value is a tuple containing the number of rows affected assuming the stored procedure
does not use `SET NOCOUNT ON`, the return value of the stored procedure if any, and a
dictionary containing any output parameters indexed by method parameter name with the value being the output
value from the stored procedure.  Typically, the data context method will return the rows affected or return
value or use the output parameters to update the reference method parameters and return nothing.  However, unlike
LINQ to SQL, the tuple itself or any combination of the above can be returned by the Entity Framework data
context method.


``` cs
// Execute a stored procedure and return its return value
public int spStockAdd(string symbol, string assetDescription, decimal currentBid,
  decimal currentAsk, decimal priceChangePercent)
{
    return this.ExecuteMethodNonQuery(this.GetMethodInfo(), symbol, assetDescription, currentBid,
        currentAsk, priceChangePercent).ReturnValue;
}

// Execute a stored procedure and return the number of rows affected
public int spStockDelete(string symbol)
{
    return this.ExecuteMethodNonQuery(this.GetMethodInfo(), symbol).RowsAffected;
}

// Execute a stored procedure and return the output parameters via the ref parameters on
// the method.  We can also return the stored procedure's return value or rows affected.
public int spCheckForEmployeeSchedule(string bidGroup, int entityKey,
  ref bool bidGroupScheduled, ref bool entityScheduled)
{
    var result = this.ExecuteMethodNonQuery(this.GetMethodInfo(), bidGroup, entityKey,
        bidGroupScheduled, entityScheduled);

    bidGroupScheduled = (bool)result.OutputValues[nameof(bidGroupScheduled);
    entityScheduled = (bool)result.OutputValues[nameof(entityScheduled);

    return result.ReturnValue;
}
				
```


## Query Stored Procedure Methods

A query stored procedure method is similar to the `LoadByKey` extension
method except the parameters do not have to match the key properties on the entity such as a search query that
allows for various criteria parameters.  If no parameters are specified it is equivalent to the
`LoadAll` extension method so it is not necessary to create a separate data context method
for those cases.


The data context method calls the
[](@M:EWSoftware.EntityFramework.DatabaseExtensions.ExecuteMethodQuery``1(Microsoft.EntityFrameworkCore.DbContext,System.Reflection.MethodInfo,System.Object[])){prefer-overload="true"}	extension method. As above, the first parameter is always a `MethodInfo` instance
representing the calling method.  Subsequent parameters, if any, are those passed to the data context method.
The order of the parameters must match the order for the calling data context method.  All parameters are input
only.  The return value of the data context method is an enumerable list of the given entity type created from
the query's result set.


> [!IMPORTANT]
> The query extension methods return an enumerable list of the results.  Actual execution of the
> query is deferred until the results are requested.  As such, if your query performs any inserts, updates, or
> deletes in addition to returning a result set, you must call something like `ToList()`,
> `Any()`, or `First()` to ensure that the query is executed or the
> changes will not be made.
> 
>

``` cs
// Execute a search stored procedure and return its result set
public IEnumerable<spTransactionListResult> spTransactionList(int accountKey,
  string? symbol, DateTime fromDate, DateTime toDate, string? txType)
{
    return this.ExecuteMethodQuery<spTransactionListResult>(this.GetMethodInfo(),
        accountKey, symbol, fromDate, toDate, txType);
}
				
```


## Differences for the Asynchronous Extension Methods

The asynchronous versions of the query and non-query extension methods require a few modifications
in order to use them correctly.


- If you follow convention and suffix the method name with "Async", you can still omit the
`MethodStoredProcedureAttribute` as the suffix will be removed automatically when the method
name ends with the [](@P:EWSoftware.EntityFramework.DatabaseExtensions.AsyncMethodSuffix)
property value.  If you use a suffix different from the property value, you will need to specify the attribute.
- The method information must be obtained using the stack trace to find the actual called method.
This is required because the code within the stored procedure method is rewritten by the compiler and is
placed inside a separate compiler generated method.  Always use the `GetMethodInfo`
extension method as shown in the examples as it will work whether the method is synchronous or asynchronous.
- The method parameters must be passed to the extension method as an array.  This is necessary
as the last parameter for the extension method is the optional cancellation token.
- A cancellation token cannot be passed explicitly to the stored procedure method as it will be
considered one of the stored procedure parameters and the method parameter count will not match the number of
parameters passed to the extension method.  The
[](@M:System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation``1(System.Collections.Generic.IAsyncEnumerable{``0},System.Threading.CancellationToken)){prefer-overload="true"}
extension method can be used instead to achieve the same result.


Some examples are shown below.


``` cs
// Execute a stored procedure and return its return value.  We could omit the
// attribute here since the name matches the method name with an "Async" suffix".
[MethodStoredProcedure("spStockAdd")]
public async int spStockAddAsync(string symbol, string assetDescription,
  decimal currentBid, decimal currentAsk, decimal priceChangePercent)
{
    var result = await this.ExecuteMethodNonQueryAsync(this.GetMethodInfo(), [symbol,
        assetDescription, currentBid, currentAsk, priceChangePercent]);

    return result.ReturnValue;
}

// Execute a stored procedure and return the number of rows affected
[MethodStoredProcedure("spStockDelete")]
public async int spStockDeleteAsync(string symbol)
{
    var result = await this.ExecuteMethodNonQueryAsync(this.GetMethodInfo(), [symbol]);

    return result.RowsAffected;
}

// Execute a stored procedure and return the output parameters via the ref parameters on
// the method.  We can also return the stored procedure's return value or rows affected.
[MethodStoredProcedure("spCheckForEmployeeSchedule")]
public async int spCheckForEmployeeScheduleAsync(string bidGroup, int entityKey,
  ref bool bidGroupScheduled, ref bool entityScheduled)
{
    var result = await this.ExecuteMethodNonQueryAsync(this.GetMethodInfo(), [bidGroup,
        entityKey, bidGroupScheduled, entityScheduled]);

    bidGroupScheduled = (bool)result.OutputValues[nameof(bidGroupScheduled);
    entityScheduled = (bool)result.OutputValues[nameof(entityScheduled);

    return result.ReturnValue;
}

// Execute a search stored procedure and return its result set
[MethodStoredProcedure("spTransactionList")]
public IAsyncEnumerable<spTransactionListResult> spTransactionListAsync(int accountKey,
  string? symbol, DateTime fromDate, DateTime toDate, string? txType)
{
    // Note that we can't pass a cancellation token as it would look like one of
    // the method parameters.  Use the WithCancellation() extension method on the
    // call to this method instead.
    return this.ExecuteMethodQueryAsync<spTransactionListResult>(this.GetMethodInfo(),
        [accountKey, symbol, fromDate, toDate, txType]);
}

// We can't pass the cancellation token to the query method as it will look like
// a parameter to the stored procedure.  We need to use the WithCancellation()
// extension method instead.
var cts = new CancellationTokenSource();

await foreach(var t in dc.spTransactionListAsync(1, "MSFT", fromDate,
    toDate, null).WithCancellation(cts.Token))
{
    ....
}

				
```


## See Also


**Other Resources**  
[](@48c2006f-d738-40a1-a486-d53fbdf7208c)  
