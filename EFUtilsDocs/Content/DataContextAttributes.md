---
uid: 227b8bd9-c28a-4e13-b2e5-9bcfbc8fed25
alt-uid: DataContextAttributes
title: Data Context Attributes
---
This topic describes the data context attributes.

<autoOutline excludeRelatedTopics="true" lead="none" />

## SchemaNameAttribute
The optional [](@T:EWSoftware.EntityFramework.DataAnnotations.SchemaNameAttribute) is used to define a common
schema name that will be used for all stored procedures called by the data context extension methods.  It should
be applied to the data context class.  When a stored procedure name defined in an attribute already contains a
schema name, it is returned as is.  If it does not and the data context has this attribute applied to it, the
schema name from it is added to the stored procedure name.  If not, the stored procedure name is returned as is
and will use the default schema.

``` csharp
[SchemaName("Demo")]
public sealed class DemoDatabaseDataContext : DbContext
{
    ... Data context definition ...
}
```

## ParameterNamePrefixAttribute
The optional [](@T:EWSoftware.EntityFramework.DataAnnotations.ParameterNamePrefixAttribute) is used to define a
common stored procedure parameter name prefix that will be used for all stored procedures called by the data
context extension methods.  It should be applied to the data context class.  As an example, if set to "param" and
an entity property name is `AccountKey`, the stored procedure parameter name will be set to `@paramAccountKey`.
If not defined on a data context the parameter will be named after the property (`@AccountKey` in the preceding
example).

``` csharp
[ParameterNamePrefix("param")]
public sealed class DemoDatabaseDataContext : DbContext
{
    ... Data context definition ...
}
```

## See Also
**Other Resources**  
[](@48c2006f-d738-40a1-a486-d53fbdf7208c)  
