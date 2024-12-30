# EWSoftware.EntityFramework Library
The main goal of this library is to provide better support for stored procedures in Entity Framework and to
provide a way to use them in a way similar to LINQ to SQL. It consists of a set of extension methods and some
data annotation attributes that can be applied to the entity types to define which stored procedure to use for
operations such as loading all entities, loading by key value, inserts, updates, and deletes. It also provides a
way to add stored procedure methods to the Entity Framework data context similar to LINQ to SQL.

To aid in the transition from LINQ to SQL to Entity Framework, a conversion tool is
[also provided](https://github.com/EWSoftware/EntityFrameworkUtils/releases) that allows converting a DBML file to
a fairly equivalent set of entity classes and a data context. Some review and rework of the converted types and
the code that uses them is still needed but it is a quick way to get up and running with Entity Framework
without having to completely rewrite the LINQ to SQL data access code especially if you made significant use of
stored procedures with it.

A NuGet package is available: [EWSoftware.EntityFramework](http://www.nuget.org/packages/EWSoftware.EntityFramework)

See the [online help content](http://EWSoftware.github.io/EntityFrameworkUtils/index.html) for usage and API information.
