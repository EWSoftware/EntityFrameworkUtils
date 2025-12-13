---
uid: f818ea4c-f1c6-4bc0-b7f1-373bde857dd1
alt-uid: ResultSetAndParamExtMethods
title: Result Set and Parameter Extension Methods
---
The following extension methods are used with the result sets of the query extension methods or the parameters
passed to them.  See the extension method documentation for usage examples.

<autoOutline excludeRelatedTopics="true" lead="none" />

## Result Set Extension Methods
The following result set extension methods are available:

- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.ToTrackingBindingList``1(System.Collections.Generic.IEnumerable{``0},Microsoft.EntityFrameworkCore.DbContext)){prefer-overload="true"} - 
  This converts an enumerable list of change tracking entities to a binding list that will notify the related
  data context of additions, changes, and deletions.  The returned binding list is only suitable for binding to
  Windows Forms controls.  Use the method below for WPF controls.
- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.ToTrackingCollection``1(System.Collections.Generic.IEnumerable{``0},Microsoft.EntityFrameworkCore.DbContext)){prefer-overload="true"} - 
  This converts an enumerable list of change tracking entities to an observable collection that will notify the
  related data context of additions, changes, and deletions.  The returned observable collection is only suitable
  for use with WPF controls.  Use the method above for Windows Forms controls.

## Parameter Value Extension Methods
The following parameter value extension methods are available:

- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.NullIfEmpty(System.String)){prefer-overload="true"} -
  This is used to convert strings to null values if they are empty.  This is useful for passing string values to
  database context methods when the value needs to be passed or stored as a null value rather than an empty string.
- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.NullIfWhiteSpace(System.String)){prefer-overload="true"} - 
  This is used to convert strings to null values if they are empty or all whitespace.  This is useful for passing
  string values to database context methods when the value needs to be passed or stored as a null value rather than
  an empty or whitespace string.
- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.ToStringOrNull(System.Object)){prefer-overload="true"} - 
  This is used to convert an object to a string and return either the string value if not empty, or null if it is
  an empty string.  This is useful for passing object values to database context methods when the value needs to
  be passed or stored as a null value rather than an empty string.
- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.ToChar(System.String)){prefer-overload="true"} - 
  This is used to convert a string value to a nullable `Char` by returning null if the
  string is null or empty or the first character of the string if not.
- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.ToNullable``1(``0)){prefer-overload="true"} - 
  This is used to convert value types to null values if they are set to their default value for the type (i.e.
  zero for integers, `DateTime.MinValue` for date/times, etc).  This is useful for passing
  values to database context methods when the parameters needs to be passed or stored as a null value rather than a
  literal value if it is set to the default.
- [](@M:EWSoftware.EntityFramework.DatabaseExtensions.ToNullable``1(System.Object)){prefer-overload="true"} - 
  This is used to convert objects to null values if they are equal to `null`,
  `DBNull.Value`, or the default value for the given type.  This is useful for passing
  values to database context methods when the parameters needs to be passed or stored as a null rather than
  `DBNull.Value` or the type's default value.

## See Also
**Other Resources**  
[](@48c2006f-d738-40a1-a486-d53fbdf7208c)  
