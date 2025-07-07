//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : DatabaseExtensions.cs
// Author  : Eric Woodruff
// Updated : 06/14/2025
//
// This file contains a class that contains extension methods for database objects
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/22/2024  EFW  Created the code
//===============================================================================================================

// Ignore Spelling: se cts

using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using EWSoftware.EntityFramework.DataAnnotations;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EWSoftware.EntityFramework
{
    /// <summary>
    /// This class contains extension methods for database objects
    /// </summary>
    public static class DatabaseExtensions
    {
        #region Helper methods
        //=====================================================================

        /// <summary>
        /// Create a stored procedure name
        /// </summary>
        /// <param name="dataContext">The data context that may have a <see cref="SchemaNameAttribute"/></param>
        /// <param name="storedProcedureName">The stored procedure name</param>
        /// <returns>The stored procedure name prefixed with the schema name if defined.  If the passed stored
        /// procedure name already contains a schema name, it is returned as is.  If the data context has a
        /// <see cref="SchemaNameAttribute"/>, the schema name from it is added to the stored procedure name.  If
        /// not, the stored procedure name is returned as is.</returns>
        private static string CreateStoredProcedureName(DbContext dataContext, string storedProcedureName)
        {
#if !NETSTANDARD2_0
            if(storedProcedureName.Contains('.', StringComparison.Ordinal))
                return storedProcedureName;
#else
            if(storedProcedureName.Contains('.'))
                return storedProcedureName;
#endif
            var schemaName = dataContext.GetType().GetCustomAttribute<SchemaNameAttribute>();

            if(schemaName == null)
                return storedProcedureName;

            return String.Concat(schemaName.SchemaName, ".", storedProcedureName);
        }

        /// <summary>
        /// Create a command instance to load entities of the given type using a stored procedure defined on the
        /// entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="dataContext">The data context to use</param>
        /// <param name="parameters">For loading all entities, this is null.  For loading by key, it is one or
        /// more parameter values that will be passed to the stored procedure.  The parameter order must match
        /// the declared key order on the entity but does not have to match the parameter order in the stored
        /// procedure.</param>
        /// <returns>A tuple containing the connection instance, the command instance, a dictionary containing
        /// the entity type properties, the never track flag, and the entity type.  The caller is responsible
        /// for disposing of the command instance.</returns>
        private static (DbConnection connection, DbCommand command, Dictionary<string, PropertyInfo> properties,
          bool neverTrack, Type entityType)
          CreateLoadCommand<TEntity>(DbContext dataContext, object?[]? parameters)
        {
            Type entityType = typeof(TEntity);
            string storedProcName;
            string? parameterNamePrefix = null;

            if((parameters?.Length ?? 0) == 0)
            {
                var spNameAttr = entityType.GetCustomAttributes<LoadAllStoredProcedureAttribute>().FirstOrDefault() ??
                    throw new NotSupportedException("The specified entity type is not decorated with the " +
                        nameof(LoadAllStoredProcedureAttribute));

                storedProcName = spNameAttr.StoredProcedureName;
            }
            else
            {
                var spNameAttr = entityType.GetCustomAttributes<LoadByKeyStoredProcedureAttribute>().FirstOrDefault() ??
                    throw new NotSupportedException("The specified entity type is not decorated with the " +
                        nameof(LoadByKeyStoredProcedureAttribute));

                storedProcName = spNameAttr.StoredProcedureName;

                var namePrefixAttr = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();

                parameterNamePrefix = spNameAttr.ParameterNamePrefix ?? namePrefixAttr?.Prefix;
            }

            bool neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().Any();
            var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);
            var connection = dataContext.Database.GetDbConnection();
            var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, storedProcName);

            if((parameters?.Length ?? 0) != 0)
            {
                // Get the primary key for the entity type
                var keys = DetermineKeyProperties(dataContext, entityType, properties.Values).ToList();

                if(keys.Count != parameters!.Length)
                {
                    throw new InvalidOperationException($"The number of keys specified on the entity ({keys.Count}) " +
                        $"does not match the number of parameters passed ({parameters.Length})");
                }

                int paramIdx = 0;

                // Add a parameter for each key field
                foreach(string key in keys)
                {
                    if(!properties.TryGetValue(key, out var p))
                        throw new InvalidOperationException($"The key property {key} was not found on the entity");

                    // If the property has a column attribute, use the name from it instead
                    var columnName = p.GetCustomAttribute<ColumnAttribute>();

                    object? value = parameters[paramIdx++];

                    if(value != null)
                    {
                        var valueType = value.GetType();

                        // For nullable types, compare the underlying type to the value type
                        var underlyingType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                        if(!underlyingType.Equals(valueType))
                        {
                            throw new InvalidOperationException($"Data type of property {p.Name} ({p.PropertyType.Name}) " +
                                $"does not match the data type of the parameter value \"{value}\" ({valueType.Name})");
                        }
                    }

                    // If the parameter value is null, use DBNull.Value to send a NULL to the database rather than
                    // using any default value assigned to the parameter.
                    var param = new SqlParameter($"@{parameterNamePrefix}{columnName?.Name ?? key}",
                        value ?? DBNull.Value);
                    param.SetParameterType(p.PropertyType);
                    command.Parameters.Add(param);
                }
            }

            return (connection, command, properties, neverTrack, entityType);
        }

        /// <summary>
        /// Create a command instance to insert or update an entity of the given type using a stored procedure
        /// defined on the entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="dataContext">The data context to use</param>
        /// <param name="entity">The entity to insert or update</param>
        /// <param name="forInsert">True for insert, false for update</param>
        /// <returns>A tuple containing the connection instance, the command instance, a list containing
        /// the input/output parameters, and the never track flag.  The caller is responsible for disposing of
        /// the command instance.</returns>
        private static (DbConnection connection, DbCommand command,
          List<(PropertyInfo Property, SqlParameter Parameter)> inOutParams, bool neverTrack)
          CreateInsertUpdateCommand<TEntity>(DbContext dataContext, TEntity entity, bool forInsert)
        {
            Type entityType = typeof(TEntity);
            string storedProcName;
            string? parameterNamePrefix;
            StoredProcedureAttribute spNameAttr;
            List<string>? keys = null;

            var properties = entityType.GetProperties();

            if(forInsert)
            {
                spNameAttr = entityType.GetCustomAttributes<InsertEntityStoredProcedureAttribute>().FirstOrDefault() ??
                    throw new NotSupportedException("The specified entity type is not decorated with the " +
                        nameof(InsertEntityStoredProcedureAttribute));

                keys = [.. DetermineKeyProperties(dataContext, entityType, properties)];
            }
            else
            {
                spNameAttr = entityType.GetCustomAttributes<UpdateEntityStoredProcedureAttribute>().FirstOrDefault() ??
                    throw new NotSupportedException("The specified entity type is not decorated with the " +
                        nameof(UpdateEntityStoredProcedureAttribute));
            }

            storedProcName = spNameAttr.StoredProcedureName;

            var namePrefixAttr = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();

            parameterNamePrefix = spNameAttr.ParameterNamePrefix ?? namePrefixAttr?.Prefix;

            bool neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().Any();
            var connection = dataContext.Database.GetDbConnection();
            var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, storedProcName);

            var inOutParams = new List<(PropertyInfo Property, SqlParameter Parameter)>();

            // Add a parameter for each public property unless it is ignored for inserts/updates
            foreach(var p in properties)
            {
                var ignored = p.GetCustomAttribute<IgnoreAttribute>();

                if((forInsert && !(ignored?.ForInsert ?? false)) || (!forInsert && !(ignored?.ForUpdate ?? false)))
                {
                    // If the property has a column attribute, use the name from it instead
                    var columnName = p.GetCustomAttribute<ColumnAttribute>();
                    var timestamp = p.GetCustomAttribute<TimestampAttribute>();

                    // If the parameter value is null, use DBNull.Value to send a NULL to the database rather than
                    // using any default value assigned to the parameter.
                    var param = new SqlParameter($"@{parameterNamePrefix}{columnName?.Name ?? p.Name}",
                        p.GetValue(entity) ?? DBNull.Value);
                    param.SetParameterType(p.PropertyType);

                    // If it's a time stamp or, for inserts only, a single key column that looks like an identity
                    // column, make it an input/output parameter.  Primary keys with multiple columns or
                    // non-integer keys are not assumed to be output parameters.
                    if(timestamp != null || (forInsert && param.SqlDbType == SqlDbType.Int && keys!.Count == 1 &&
                      keys[0] == p.Name))
                    {
                        param.Direction = ParameterDirection.InputOutput;
                        inOutParams.Add((p, param));
                    }

                    command.Parameters.Add(param);
                }
            }

            return (connection, command, inOutParams, neverTrack);
        }

        /// <summary>
        /// Create a command instance to delete an entity of the given type using a stored procedure defined on
        /// the entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="dataContext">The data context to use</param>
        /// <param name="entity">The entity to insert or update</param>
        /// <returns>A tuple containing the connection instance, the command instance, and the never track flag.
        /// The caller is responsible for disposing of the command instance.</returns>
        private static (DbConnection connection, DbCommand command, bool neverTrack)
          CreateDeleteCommand<TEntity>(DbContext dataContext, TEntity entity)
        {
            Type entityType = typeof(TEntity);
            string? parameterNamePrefix;

            var spNameAttr = entityType.GetCustomAttributes<DeleteEntityStoredProcedureAttribute>().FirstOrDefault() ??
                throw new NotSupportedException("The specified entity type is not decorated with the " +
                    nameof(DeleteEntityStoredProcedureAttribute));

            var namePrefixAttr = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();

            parameterNamePrefix = spNameAttr.ParameterNamePrefix ?? namePrefixAttr?.Prefix;

            bool neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().Any();
            var connection = dataContext.Database.GetDbConnection();

            using var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, spNameAttr.StoredProcedureName);

            // Get the primary key for the entity type and add a parameter for each key field
            var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);

            foreach(string key in DetermineKeyProperties(dataContext, entityType, properties.Values))
            {
                if(!properties.TryGetValue(key, out var p))
                    throw new InvalidOperationException($"The key property {key} was not found on the entity");

                // If the property has a column attribute, use the name from it instead
                var columnName = p.GetCustomAttribute<ColumnAttribute>();

                // If the property value is null, use DBNull.Value to send a NULL to the database rather than
                // using any default value assigned to the parameter.
                var param = new SqlParameter($"@{parameterNamePrefix}{columnName?.Name ?? key}",
                    p.GetValue(entity) ?? DBNull.Value);
                param.SetParameterType(p.PropertyType);
                command.Parameters.Add(param);
            }

            return (connection, command, neverTrack);
        }

        /// <summary>
        /// Executes an insert or update for the specified entity in the given data context
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to be inserted or updated.</typeparam>
        /// <param name="dataContext">The data context to use</param>
        /// <param name="entity">The entity to insert or update</param>
        /// <param name="forInsert">True for insert, false for update</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        private static int InsertUpdateEntityInternal<TEntity>(DbContext dataContext, TEntity entity, bool forInsert)
        {
            // Any changes made here should also be made to InsertUpdateEntityInternalAsync if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(entity);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
#endif
            var (connection, command, inOutParams, neverTrack) = CreateInsertUpdateCommand(dataContext, entity, forInsert);
            bool closeConnection = false;
            int rowsAffected;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                rowsAffected = command.ExecuteNonQuery();

                // Update output parameters with their values
                foreach(var inOut in inOutParams)
                    inOut.Property.SetValueFromDatabase(entity, inOut.Parameter.Value);
            }
            finally
            {
                command?.Dispose();

                if(closeConnection)
                    connection.Close();
            }

            if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
            {
                var changeEntry = dataContext.Entry(entity);

                // We must set the original values to the new values first before marking it unchanged
                // or it reverts the values to the original unmodified values.
                changeEntry.OriginalValues.SetValues(changeEntry.Entity);
                changeEntry.State = EntityState.Unchanged;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Executes an insert or update asynchronously for the specified entity in the given data context
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to be inserted or updated.</typeparam>
        /// <param name="dataContext">The data context to use</param>
        /// <param name="entity">The entity to insert or update</param>
        /// <param name="forInsert">True for insert, false for update</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        private static async Task<int> InsertUpdateEntityInternalAsync<TEntity>(DbContext dataContext,
          TEntity entity, bool forInsert, CancellationToken cancellationToken)
        {
            // Any changes made here should also be made to InsertUpdateEntityInternal if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(entity);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
#endif
            var (connection, command, inOutParams, neverTrack) = CreateInsertUpdateCommand(dataContext, entity, forInsert);
            bool closeConnection = false;
            int rowsAffected;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                // Update output parameters with their values
                foreach(var inOut in inOutParams)
                    inOut.Property.SetValueFromDatabase(entity, inOut.Parameter.Value);
            }
            finally
            {
                command?.Dispose();

#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }

            if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
            {
                var changeEntry = dataContext.Entry(entity);

                // We must set the original values to the new values first before marking it unchanged
                // or it reverts the values to the original unmodified values.
                changeEntry.OriginalValues.SetValues(changeEntry.Entity);
                changeEntry.State = EntityState.Unchanged;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Prepare the command instance for use by a method stored procedure execution method
        /// </summary>
        /// <param name="dataContext">The data context that will be used</param>
        /// <param name="command">The command instance to prepare</param>
        /// <param name="methodInfo">The method information for the user's calling method</param>
        /// <param name="parameters">The user's calling method parameters</param>
        /// <param name="memberName">The name of the member calling this one (used for error reporting)</param>
        /// <returns>A list containing tuples representing method parameter names and their corresponding index
        /// in the parameter set.  The caller can use this to return output values.</returns>
        private static List<(string ParameterName, int ParameterIndex)> PrepareMethodCommand(
          DbContext dataContext, DbCommand command, MethodInfo methodInfo,
          object?[] parameters, [CallerMemberName] string memberName = "")
        {
            var spName = methodInfo.GetCustomAttributes<MethodStoredProcedureAttribute>().FirstOrDefault();
            var inOutParams = new List<(string ParameterName, int ParameterIndex)>();
            var contextParamPrefix = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, spName?.StoredProcedureName ?? methodInfo.Name);

            if((parameters?.Length ?? 0) != 0)
            {
                var methodParams = methodInfo.GetParameters();

                if(methodParams.Length != parameters!.Length)
                {
                    throw new InvalidOperationException($"The number of parameters passed to {memberName} " +
                        $"does not match the number of parameters on the calling method {methodInfo.Name}");
                }

                for(int idx = 0; idx < parameters!.Length; idx++)
                {
                    var mp = methodParams[idx];

                    // If the parameter has a column attribute, use the name from it instead
                    var columnName = mp.GetCustomAttribute<ColumnAttribute>();

                    object? value = parameters[idx];
                    var paramType = mp.ParameterType.IsByRef ? mp.ParameterType.GetElementType()! : mp.ParameterType;

                    if(value != null)
                    {
                        var valueType = value.GetType();

                        // For nullable types, compare the underlying type to the value type
                        var underlyingType = Nullable.GetUnderlyingType(paramType) ?? paramType;

                        if(!underlyingType.Equals(valueType))
                        {
                            throw new InvalidOperationException($"Data type of method parameter {mp.Name} ({paramType.Name}) " +
                                $"does not match the data type of the parameter value \"{value}\" ({valueType.Name})");
                        }
                    }

                    // If the parameter value is null, use DBNull.Value to send a NULL to the database rather than
                    // using any default value assigned to the parameter.
                    var p = new SqlParameter($"@{spName?.ParameterNamePrefix ?? contextParamPrefix?.Prefix}{columnName?.Name ?? mp.Name}",
                        value ?? DBNull.Value);
                    p.SetParameterType(paramType);

                    // If the method parameter is by reference, make the SQL parameter input/output
                    if(mp.ParameterType.IsByRef)
                    {
                        p.Direction = ParameterDirection.InputOutput;

                        // For nulls, the size must be set for output parameters or it will fail.  We just pick
                        // an arbitrary large size.
                        if(p.Value == DBNull.Value && !paramType.IsValueType)
                            p.Size = Int32.MaxValue;

                        inOutParams.Add((mp.Name!, idx));
                    }

                    command.Parameters.Add(p);
                }
            }

            // Always add the return value parameter
            command.Parameters.Add(new SqlParameter("@___returnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue });

            return inOutParams;
        }

        /// <summary>
        /// Given an entity type and the properties from it, this will return an enumerable list of the key
        /// fields.
        /// </summary>
        /// <param name="dataContext">The data context</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="properties">An enumerable list of property information for the entity type</param>
        /// <returns>An enumerable list of the property names that are marked as the primary key or key fields
        /// on the entity.</returns>
        private static IEnumerable<string> DetermineKeyProperties(DbContext dataContext, Type entityType,
          IEnumerable<PropertyInfo> properties)
        {
            var keys = dataContext.Model.FindEntityType(entityType)?.FindPrimaryKey()?.Properties.Select(
                p => p.Name).ToList();

            if((keys?.Count ?? 0) != 0)
            {
                foreach(var key in keys!)
                    yield return key;

            }
            else
            {
#if !NETSTANDARD2_0
                var primaryKey = entityType.GetCustomAttributes<PrimaryKeyAttribute>().FirstOrDefault();

                if(primaryKey != null)
                {
                    foreach(var key in primaryKey.PropertyNames)
                        yield return key;
                }
                else
                {
#endif
                    bool hasKey = false;

                    // If there is no primary key attribute, get the names of all properties with a key attribute
                    foreach(var p in properties)
                    {
                        var keyAttr = p.GetCustomAttributes<KeyAttribute>().FirstOrDefault();

                        if(keyAttr != null)
                        {
                            hasKey = true;
                            yield return p.Name;
                        }
                    }

                    if(!hasKey)
                        throw new NotSupportedException("No primary key or key property is defined on the specified entity type");
#if !NETSTANDARD2_0
                }
#endif
            }
        }

        /// <summary>
        /// Set a property value from the database accounting for certain special conversion cases
        /// </summary>
        /// <param name="propInfo">The property information</param>
        /// <param name="entity">The entity on which to set the property</param>
        /// <param name="value">The new property value</param>
        /// <remarks><see cref="DBNull.Value"/> is converted to null.  If the property type is <see cref="Char"/>
        /// but the value is a non-null non-empty <see cref="String"/>, only the first character is used.</remarks>
        private static void SetValueFromDatabase(this PropertyInfo propInfo, object entity, object? value)
        {
            if(value == DBNull.Value)
                value = null;
            else
            {
                if(value != null)
                {
                    // Character values can be returned as single character strings.  If this looks like the case,
                    // get the first character of the string to avoid an exception.  If the string is empty, use a
                    // null character.
                    if((propInfo.PropertyType == typeof(char) || propInfo.PropertyType == typeof(char?)) &&
                      value.GetType() == typeof(string))
                    {
                        string stringValue = (string)value;

                        value = (stringValue.Length != 0) ? stringValue[0] : '\x0';
                    }
                    else
                    {
                        // XML values need to be converted from a string
                        if(propInfo.PropertyType == typeof(XElement) && value.GetType() == typeof(string))
                            value = XElement.Parse((string)value);
                    }
                }
            }

            try
            {
                propInfo.SetValue(entity, value);
            }
            catch(ArgumentException ex)
            {
                // If the types don't match, provide more information to help track down the cause.  For nullable
                // types, show the underlying type.
                var underlyingType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                throw new InvalidOperationException($"Possible type mismatch on {entity.GetType().Name} " +
                    $"property {propInfo.Name}.  Property type is {underlyingType.Name} but the " +
                    $"database value's type is {value?.GetType().Name ?? "(null)"}.", ex);
            }
        }

        /// <summary>
        /// This is used to infer the SQL database type from the value type
        /// </summary>
        /// <param name="parameter">The SQL parameter in which to set the SQL type</param>
        /// <param name="valueType">The property value type</param>
        private static void SetParameterType(this SqlParameter parameter, Type valueType)
        {
            // If it's a nullable type, get the type of the type parameter
            if(valueType.IsGenericType)
                valueType = valueType.GetGenericArguments()[0];

            switch(Type.GetTypeCode(valueType))
            {
                case TypeCode.Boolean:
                    parameter.SqlDbType = SqlDbType.Bit;
                    break;

                case TypeCode.Byte:
                    parameter.SqlDbType = SqlDbType.TinyInt;
                    break;

                case TypeCode.Char:
                    parameter.SqlDbType = SqlDbType.Char;
                    parameter.Size = 1;
                    break;

                case TypeCode.DateTime:
                    if(parameter.Value != null && parameter.Value != DBNull.Value)
                    {
                        if(((DateTime)parameter.Value).Year < 1753)
                            parameter.SqlDbType = SqlDbType.DateTime2;
                        else
                            parameter.SqlDbType = SqlDbType.DateTime;
                    }
                    break;

                case TypeCode.Decimal:
                    parameter.SqlDbType = SqlDbType.Money;
                    break;

                case TypeCode.Double:
                    parameter.SqlDbType = SqlDbType.Float;
                    break;

                case TypeCode.Int16:
                    parameter.SqlDbType = SqlDbType.SmallInt;
                    break;

                case TypeCode.Int32:
                    parameter.SqlDbType = SqlDbType.Int;
                    break;

                case TypeCode.Int64:
                    parameter.SqlDbType = SqlDbType.BigInt;
                    break;

                case TypeCode.Single:
                    parameter.SqlDbType = SqlDbType.Real;
                    break;

                default:
                    // XML and byte arrays requires special handling
                    if(valueType == typeof(XElement))
                    {
                        parameter.SqlDbType = SqlDbType.Xml;

                        if(parameter.Value != null && parameter.Value != DBNull.Value)
                            parameter.Value = new SqlXml(((XElement)parameter.Value).CreateReader());
                    }
                    else
                    {
                        if(valueType == typeof(byte[]))
                            parameter.SqlDbType = SqlDbType.VarBinary;
                    }

                    // Anything else we'll leave alone and let it infer
                    break;
            }
        }
        #endregion

        #region Data context extension methods
        //=====================================================================

        /// <summary>
        /// This extension method is used to turn off query tracking in a data context when constructed.
        /// </summary>
        /// <typeparam name="T">The data context type</typeparam>
        /// <param name="dataContext">The data context in which to turn off query tracking.</param>
        /// <returns>The passed data context object</returns>
        /// <remarks>This can improve performance if all you are doing is reading data without any updates.  This
        /// method can be used in conjunction with the <see cref="Open" /> extension method as shown in the
        /// example below.</remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext().NoTracking();
        /// 
        /// ... Execute commands ...
        /// 
        /// using var dataContext = new MyDbContext().NoTracking().Open();
        /// 
        /// ... Execute commands ...
        /// 
        /// </code>
        /// </example>
        public static T NoTracking<T>(this T dataContext) where T : DbContext
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            dataContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return dataContext;
        }

        /// <summary>
        /// This extension method is used to open a connection on a data context when constructed.
        /// </summary>
        /// <typeparam name="T">The data context type</typeparam>
        /// <param name="dataContext">The data context on which to open the connection.</param>
        /// <returns>The passed data context object</returns>
        /// <remarks>This method can be used in conjunction with the <see cref="NoTracking" /> extension method
        /// as shown in the example below.</remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext().Open();
        /// 
        /// ... Execute commands ...
        ///
        /// using var dataContext = new MyDbContext().NoTracking().Open();
        /// 
        /// ... Execute commands ...
        /// 
        /// </code>
        /// </example>
        public static T Open<T>(this T dataContext) where T : DbContext
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            dataContext.Database.OpenConnection();

            return dataContext;
        }

        /// <summary>
        /// This extension method is used to open a connection on a data context asynchronously when constructed.
        /// </summary>
        /// <typeparam name="T">The data context type</typeparam>
        /// <param name="dataContext">The data context on which to open the connection.</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>The passed data context object</returns>
        /// <remarks>This method can be used in conjunction with the <see cref="NoTracking" /> extension method
        /// as shown in the example below.</remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = await new MyDbContext().OpenAsync();
        /// 
        /// ... Execute commands ...
        ///
        /// using var dataContext = await new MyDbContext().NoTracking().OpenAsync();
        /// 
        /// ... Execute commands ...
        /// 
        /// </code>
        /// </example>
        public static async Task<T> OpenAsync<T>(this T dataContext,
          CancellationToken cancellationToken = default) where T : DbContext
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            await dataContext.Database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

            return dataContext;
        }

        /// <summary>
        /// This extension method is used to see if a data context has any unsaved changes.
        /// </summary>
        /// <param name="dataContext">The data context to check</param>
        /// <returns>True if the data context's change set has any inserted, updated, or deleted items waiting to
        /// be submitted, false if not.</returns>
        /// <example>
        /// <code language="cs">
        /// if(dataContext.HasChanges())
        ///     dataContext.SaveChanges();
        /// </code>
        /// </example>
        public static bool HasChanges(this DbContext dataContext)
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            return dataContext.ChangeTracker.HasChanges();
        }

        /// <summary>
        /// This extension method is used to get a command object in a state ready to execute the specified
        /// command text on the given data context.
        /// </summary>
        /// <param name="dataContext">The data context to use</param>
        /// <param name="commandText">The command text can be a stored procedure with or without parameters or a
        /// literal SQL statement.  It will be used when creating the command object.</param>
        /// <param name="parameters">Zero or more parameters for the command.</param>
        /// <returns>Returns a <see cref="SqlCommand"/> object.</returns>
        /// <remarks>The command will be associated with the connection from the data context instance on which
        /// this method is called with the command timeout set to the value from the data context if one has been
        /// set.  The command text can be a stored procedure with or without parameters or a literal SQL statement.
        /// The returned command is ready to execute or modify further with parameters.  This is useful for
        /// situations in which a <see cref="SqlDataReader"/> is needed, you need more control when executing a
        /// stored procedure, or the stored procedure returns multiple result sets.</remarks>
        ///
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext().NoTracking();
        ///
        /// // Set up the daily schedule command and data adapter
        /// var cmdDailySchedule = dc.GetCommand("spDailyScheduleInfo",
        ///     new SqlParameter("@paramBidGroup", SqlDbType.Char, 4),
        ///     new SqlParameter("@paramDate", SqlDbType.DateTime));
        ///
        /// var daDailySchedule = new SqlDataAdapter(cmdDailySchedule);
        /// </code>
        /// </example>
        public static SqlCommand GetCommand(this DbContext dataContext, string commandText, params SqlParameter[] parameters)
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(commandText);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(commandText == null)
                throw new ArgumentNullException(nameof(commandText));
#endif
            var command = (SqlCommand)dataContext.Database.GetDbConnection().CreateCommand();

            command.CommandTimeout = dataContext.Database.GetCommandTimeout() ?? command.CommandTimeout;

            // If there's a space in the SQL string, assume the command type is text.  Otherwise, set it to
            // stored procedure.
#if !NETSTANDARD2_0
            if(commandText.Contains(' ', StringComparison.Ordinal))
#else
            if(commandText.IndexOf(' ') != -1)
#endif
                command.CommandType = CommandType.Text;
            else
                command.CommandType = CommandType.StoredProcedure;

            foreach(SqlParameter p in parameters)
                command.Parameters.Add(p);

            return command;
        }

        /// <summary>
        /// This extension method is used to check for null keys or incomplete rows added by controls and
        /// either remove them or update them based on the passed in delegate methods.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="dataContext">The data context to use</param>
        /// <param name="nullKeyCheck">The function delegate to execute for null key/incomplete row checks.  If
        /// it returns true, the row is assumed to be an empty row and is detached.  If it returns false or is
        /// not specified, the <paramref name="updateKeys"/> action delegate is called.</param>
        /// <param name="updateKeys">The action delegate to execute for each row that should be retained.  This
        /// is used to update any keys in the row if necessary.  If not specified, no action is taken.</param>
        /// <remarks>In certain cases, some list controls like data grids add a temporary row for new additions.
        /// If the row is left without saving changes or the changes are cancelled, the row is not always removed
        /// and still exists when changes are saved.  Because it has null keys or missing values, it should not
        /// be kept.  This extension method can be used to find such rows and remove them.  It can also be used
        /// to update keys or other fields in new or existing rows if necessary.</remarks>
        /// <example>
        /// <code language="cs">
        /// // If a list control added a row but left it incomplete, delete it.  If not, update the parent key.
        /// dataContext.CheckAndUpdateKeys&lt;ChildTable&gt;(
        ///     (entity, state) => state == EntityState.Added &amp;&amp; entity.GroupId == null,
        ///     (entity, state) => entity.ParentId = parent.ParentId);
        /// </code>
        /// </example>
        public static void CheckAndUpdateKeys<TEntity>(this DbContext dataContext,
          Func<TEntity, EntityState, bool> nullKeyCheck, Action<TEntity, EntityState> updateKeys)
          where TEntity : ChangeTrackingEntity
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            // Get additions, updates, and deletions from the change set
            dataContext.ChangeTracker.Entries<TEntity>().Where(e => e.State != EntityState.Unchanged &&
              e.State != EntityState.Detached).ToList().ForEach(e =>
            {
                if(nullKeyCheck?.Invoke(e.Entity, e.State) ?? false)
                    e.State = EntityState.Detached;
                else
                    updateKeys?.Invoke(e.Entity, e.State);
            });
        }
        #endregion

        #region Query related data context extension methods
        //=====================================================================

        /// <summary>
        /// Load all entities of the given type using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to load</typeparam>
        /// <param name="dataContext">The data context to use when loading the entities</param>
        /// <returns>An enumerable list of the <typeparamref name="TEntity"/> entities</returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="LoadAllStoredProcedureAttribute"/> on the entity type.  The stored procedure should not
        /// have any parameters or only parameters with acceptable default values in order to return all rows.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while loading the entities.
        /// If change tracking is enabled on the data context, changes to the entities will be tracked.  If not
        /// or the entity is marked with the <see cref="NeverTrackAttribute"/>, they will not be tracked.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// var watchList = dataContext.LoadAll&lt;WatchList&gt;().ToList();
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> LoadAll<TEntity>(this DbContext dataContext) where TEntity : class, new()
        {
            // Any changes made here should also be made to LoadAllAsync if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            var (connection, command, properties, neverTrack, entityType) = CreateLoadCommand<TEntity>(
                dataContext, null);
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                using var reader = command.ExecuteReader();

                while(reader.Read())
                {
                    var entity = (TEntity)Activator.CreateInstance(entityType)!;

                    for(int column = 0; column < reader.FieldCount; column++)
                    {
                        object? value = reader.GetValue(column);

                        // Set the entity values based on what is returned by the stored procedure rather than
                        // what's in the entity.  This allows us to use different stored procedures with a common
                        // entity type even if the stored procedures don't return the same set of columns.
                        if(value != null && properties.TryGetValue(reader.GetName(column), out var p))
                            p.SetValueFromDatabase(entity, value);
                    }

                    if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                        dataContext.Attach(entity);

                    yield return entity;
                }
            }
            finally
            {
                command?.Dispose();

                if(closeConnection)
                    connection.Close();
            }
        }

        /// <summary>
        /// Load all entities of the given type asynchronously using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to load</typeparam>
        /// <param name="dataContext">The data context to use when loading the entities</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>An enumerable list of the <typeparamref name="TEntity"/> entities</returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="LoadAllStoredProcedureAttribute"/> on the entity type.  The stored procedure should not
        /// have any parameters or only parameters with acceptable default values in order to return all rows.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while loading the entities.
        /// If change tracking is enabled on the data context, changes to the entities will be tracked.  If not
        /// or the entity is marked with the <see cref="NeverTrackAttribute"/>, they will not be tracked.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// var watchList = await dataContext.LoadAllAsync&lt;WatchList&gt;().ToListAsync();
        /// </code>
        /// </example>
        public static async IAsyncEnumerable<TEntity> LoadAllAsync<TEntity>(this DbContext dataContext,
          [EnumeratorCancellation]CancellationToken cancellationToken = default) where TEntity : class, new()
        {
            // Any changes made here should also be made to LoadAll if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            var (connection, command, properties, neverTrack, entityType) = CreateLoadCommand<TEntity>(
                dataContext, null);
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                while(await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var entity = (TEntity)Activator.CreateInstance(entityType)!;

                    for(int column = 0; column < reader.FieldCount; column++)
                    {
                        object? value = reader.GetValue(column);

                        // Set the entity values based on what is returned by the stored procedure rather than
                        // what's in the entity.  This allows us to use different stored procedures with a common
                        // entity type even if the stored procedures don't return the same set of columns.
                        if(value != null && properties.TryGetValue(reader.GetName(column), out var p))
                            p.SetValueFromDatabase(entity, value);
                    }

                    if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                        dataContext.Attach(entity);

                    yield return entity;
                }
            }
            finally
            {
                command?.Dispose();

#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }
        }

        /// <summary>
        /// Load all entities of the given type using a stored procedure defined on the entity type using the
        /// given key value(s).
        /// </summary>
        /// <typeparam name="TEntity">The entity type to load</typeparam>
        /// <param name="dataContext">The data context to use when loading the entities</param>
        /// <param name="parameters">One or more parameter values that will be passed to the stored procedure.
        /// The parameter order must match the declared key order on the entity but does not have to match the
        /// parameter order in the stored procedure.</param>
        /// <returns>An enumerable list of the <typeparamref name="TEntity"/> entities matching the key values.
        /// Typically, there will only be one entity but there could be others if a non-primary key is used.</returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="LoadByKeyStoredProcedureAttribute"/> on the entity type.  The stored procedure
        /// must have a parameter for each of the passed key values in <paramref name="parameters"/>.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while loading the
        /// entities.  If change tracking is enabled on the data context, changes to the entities will be
        /// tracked.  If not, they will not be tracked.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// var assetInfo = dataContext.LoadByKey&lt;Asset&gt;(assetKey).Single();
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> LoadByKey<TEntity>(this DbContext dataContext,
          params object?[] parameters) where TEntity : class, new()
        {
            // Any changes made here should also be made to LoadByKeyAsync if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(parameters);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(parameters == null)
                throw new ArgumentNullException(nameof(parameters));
#endif
            var (connection, command, properties, neverTrack, entityType) = CreateLoadCommand<TEntity>(
                dataContext, parameters);
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                using var reader = command.ExecuteReader();

                while(reader.Read())
                {
                    var entity = (TEntity)Activator.CreateInstance(entityType)!;

                    for(int column = 0; column < reader.FieldCount; column++)
                    {
                        object? value = reader.GetValue(column);

                        // Set the entity values based on what is returned by the stored procedure rather than
                        // what's in the entity.  This allows us to use different stored procedures with a common
                        // entity type even if the stored procedures don't return the same set of columns.
                        if(value != null && properties.TryGetValue(reader.GetName(column), out var p))
                            p.SetValueFromDatabase(entity, value);
                    }

                    if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                        dataContext.Attach(entity);

                    yield return entity;
                }
            }
            finally
            {
                command?.Dispose();

                if(closeConnection)
                    connection.Close();
            }
        }

        /// <summary>
        /// Load all entities of the given type asynchronously using a stored procedure defined on the entity
        /// type using the given key value(s).
        /// </summary>
        /// <typeparam name="TEntity">The entity type to load</typeparam>
        /// <param name="dataContext">The data context to use when loading the entities</param>
        /// <param name="parameters">One or more parameter values that will be passed to the stored procedure.
        /// The parameter order must match the declared key order on the entity but does not have to match the
        /// parameter order in the stored procedure.</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>An enumerable list of the <typeparamref name="TEntity"/> entities matching the key values.
        /// Typically, there will only be one entity but there could be others if a non-primary key is used.</returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="LoadByKeyStoredProcedureAttribute"/> on the entity type.  The stored procedure
        /// must have a parameter for each of the passed key values in <paramref name="parameters"/>.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while loading the
        /// entities.  If change tracking is enabled on the data context, changes to the entities will be
        /// tracked.  If not, they will not be tracked.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// var assetInfo = await dataContext.LoadByKeyAsync&lt;Asset&gt;(assetKey).SingleAsync();
        /// </code>
        /// </example>
        public static async IAsyncEnumerable<TEntity> LoadByKeyAsync<TEntity>(this DbContext dataContext,
          object?[] parameters, [EnumeratorCancellation] CancellationToken cancellationToken = default)
          where TEntity : class, new()
        {
            // Any changes made here should also be made to LoadByKey if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(parameters);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(parameters == null)
                throw new ArgumentNullException(nameof(parameters));
#endif
            var (connection, command, properties, neverTrack, entityType) = CreateLoadCommand<TEntity>(
                dataContext, parameters);
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                while(await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var entity = (TEntity)Activator.CreateInstance(entityType)!;

                    for(int column = 0; column < reader.FieldCount; column++)
                    {
                        object? value = reader.GetValue(column);

                        // Set the entity values based on what is returned by the stored procedure rather than
                        // what's in the entity.  This allows us to use different stored procedures with a common
                        // entity type even if the stored procedures don't return the same set of columns.
                        if(value != null && properties.TryGetValue(reader.GetName(column), out var p))
                            p.SetValueFromDatabase(entity, value);
                    }

                    if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                        dataContext.Attach(entity);

                    yield return entity;
                }
            }
            finally
            {
                command?.Dispose();

#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }
        }

        /// <summary>
        /// Insert the given entity using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to insert</typeparam>
        /// <param name="dataContext">The data context to use when inserting the entity</param>
        /// <param name="entity">The entity to insert</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="InsertEntityStoredProcedureAttribute"/> on the entity type.  The stored procedure must
        /// have parameters for each of the entity properties except those marked with the
        /// <see cref="IgnoreAttribute"/> for inserts.  It should not return a value or a result set.  Parameters
        /// related to the primary key (single column, integer only) or marked with the <see cref="TimestampAttribute"/>
        /// are defined as input/out parameters.  All other parameters are input only.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while updating the entity.
        /// If change tracking is enabled on the data context and the entity its state will be set to
        /// unchanged.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// if(watchListItem.WatchID == 0)
        ///     dataContext.InsertEntity(watchListItem);
        /// else
        ///     dataContext.UpdateEntity(watchListItem);
        /// </code>
        /// </example>
        public static int InsertEntity<TEntity>(this DbContext dataContext, TEntity entity)
        {
            return InsertUpdateEntityInternal(dataContext, entity, true);
        }

        /// <summary>
        /// Insert the given entity asynchronously using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to insert</typeparam>
        /// <param name="dataContext">The data context to use when inserting the entity</param>
        /// <param name="entity">The entity to insert</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="InsertEntityStoredProcedureAttribute"/> on the entity type.  The stored procedure must
        /// have parameters for each of the entity properties except those marked with the
        /// <see cref="IgnoreAttribute"/> for inserts.  It should not return a value or a result set.  Parameters
        /// related to the primary key (single column, integer only) or marked with the <see cref="TimestampAttribute"/>
        /// are defined as input/out parameters.  All other parameters are input only.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while updating the entity.
        /// If change tracking is enabled on the data context and the entity its state will be set to
        /// unchanged.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// if(watchListItem.WatchID == 0)
        ///     await dataContext.InsertEntityAsync(watchListItem);
        /// else
        ///     await dataContext.UpdateEntityAsync(watchListItem);
        /// </code>
        /// </example>
        public static async Task<int> InsertEntityAsync<TEntity>(this DbContext dataContext, TEntity entity,
          CancellationToken cancellationToken = default)
        {
            return await InsertUpdateEntityInternalAsync(dataContext, entity, true, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Update the given entity using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to update</typeparam>
        /// <param name="dataContext">The data context to use when updating the entity</param>
        /// <param name="entity">The entity to update</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="UpdateEntityStoredProcedureAttribute"/> on the entity type.  The stored procedure must
        /// have parameters for each of the entity properties except those marked with the
        /// <see cref="IgnoreAttribute"/> for updates.  It should not return a value or a result set.  Parameters
        /// marked with the <see cref="TimestampAttribute"/> are defined as input/out parameters.  All other
        /// parameters are input only.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while updating the entity.
        /// If change tracking is enabled on the data context and the entity its state will be set to
        /// unchanged.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// if(watchListItem.WatchID == 0)
        ///     dataContext.InsertEntity(watchListItem);
        /// else
        ///     dataContext.UpdateEntity(watchListItem);
        /// </code>
        /// </example>
        public static int UpdateEntity<TEntity>(this DbContext dataContext, TEntity entity)
        {
            return InsertUpdateEntityInternal(dataContext, entity, false);
        }

        /// <summary>
        /// Update the given entity asynchronously using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to update</typeparam>
        /// <param name="dataContext">The data context to use when updating the entity</param>
        /// <param name="entity">The entity to update</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        /// <remarks>
        /// <para>The stored procedure name is determined by looking for the
        /// <see cref="UpdateEntityStoredProcedureAttribute"/> on the entity type.  The stored procedure must
        /// have parameters for each of the entity properties except those marked with the
        /// <see cref="IgnoreAttribute"/> for updates.  It should not return a value or a result set.  Parameters
        /// marked with the <see cref="TimestampAttribute"/> are defined as input/out parameters.  All other
        /// parameters are input only.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while updating the entity.
        /// If change tracking is enabled on the data context and the entity its state will be set to
        /// unchanged.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// if(watchListItem.WatchID == 0)
        ///     await dataContext.InsertEntityAsync(watchListItem);
        /// else
        ///     await dataContext.UpdateEntityAsync(watchListItem);
        /// </code>
        /// </example>
        public static async Task<int> UpdateEntityAsync<TEntity>(this DbContext dataContext, TEntity entity,
          CancellationToken cancellationToken = default)
        {
            return await InsertUpdateEntityInternalAsync(dataContext, entity, false, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the given entity using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to delete</typeparam>
        /// <param name="dataContext">The data context to use when deleting the entity</param>
        /// <param name="entity">The entity to delete</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        /// <remarks><para>The stored procedure name is determined by looking for the
        /// <see cref="DeleteEntityStoredProcedureAttribute"/> on the entity type.  The stored procedure must
        /// have one or more parameters representing the key columns on the entity type identified with a
        /// <c>PrimaryKeyAttribute</c> or one or more properties with a <c>KeyAttribute</c> or defined by the
        /// data context.  It should not return a value or a result set.  All parameters are input only.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while deleting the entity.
        /// If change tracking is enabled on the data context and the entity its state will be set to
        /// unattached.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// dataContext.DeleteEntity(watchListItem);
        /// </code>
        /// </example>
        public static int DeleteEntity<TEntity>(this DbContext dataContext, TEntity entity)
        {
            // Any changes made here should also be made to DeleteEntityAsync if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(entity);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
#endif
            var (connection, command, neverTrack) = CreateDeleteCommand(dataContext, entity);
            bool closeConnection = false;
            int rowsAffected;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                rowsAffected = command.ExecuteNonQuery();
            }
            finally
            {
                command?.Dispose();

                if(closeConnection)
                    connection.Close();
            }

            if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
            {
                var changeEntry = dataContext.Entry(entity);

                // No need to update the original values here
                changeEntry.State = EntityState.Detached;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Delete the given entity asynchronously using a stored procedure defined on the entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type to delete</typeparam>
        /// <param name="dataContext">The data context to use when deleting the entity</param>
        /// <param name="entity">The entity to delete</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>The number of rows affected assuming the stored procedure is not using <c>SET NOCOUNT ON</c></returns>
        /// <remarks><para>The stored procedure name is determined by looking for the
        /// <see cref="DeleteEntityStoredProcedureAttribute"/> on the entity type.  The stored procedure must
        /// have one or more parameters representing the key columns on the entity type identified with a
        /// <c>PrimaryKeyAttribute</c> or one or more properties with a <c>KeyAttribute</c> or defined by the
        /// data context.  It should not return a value or a result set.  All parameters are input only.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while deleting the entity.
        /// If change tracking is enabled on the data context and the entity its state will be set to
        /// unattached.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// await dataContext.DeleteEntityAsync(watchListItem);
        /// </code>
        /// </example>
        public static async Task<int> DeleteEntityAsync<TEntity>(this DbContext dataContext, TEntity entity,
          CancellationToken cancellationToken = default)
        {
            // Any changes made here should also be made to DeleteEntity if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(entity);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
#endif
            var (connection, command, neverTrack) = CreateDeleteCommand(dataContext, entity);
            bool closeConnection = false;
            int rowsAffected;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(true);
            }
            finally
            {
                command?.Dispose();

#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }

            if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
            {
                var changeEntry = dataContext.Entry(entity);

                // No need to update the original values here
                changeEntry.State = EntityState.Detached;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Submit all tracked add, update, and delete changes for the given entity type using the stored
        /// procedures defined on the entity type with the <see cref="InsertEntityStoredProcedureAttribute"/>,
        /// <see cref="UpdateEntityStoredProcedureAttribute"/>, and <see cref="DeleteEntityStoredProcedureAttribute"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type for which to submit changes</typeparam>
        /// <param name="dataContext">The data context to use for the operations</param>
        /// <remarks>This will get the changed entities from the data context's change tracker and submit them
        /// accordingly.  The state of the entities is also updated to reflect that they are in an unchanged
        /// state after being added or updated or detached if deleted.  If the connection is not in an open
        /// state, it is opened temporarily while performing the actions.</remarks>
        /// <example>
        /// <code language="cs">
        /// if(dataContext.HasChanges())
        ///     dataContext.SubmitChanges&lt;Account&gt;();
        /// </code>
        /// </example>
        public static void SubmitChanges<TEntity>(this DbContext dataContext) where TEntity : ChangeTrackingEntity
        {
            // Any changes made here should also be made to SubmitChangesAsync(dataContext) if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            var connection = dataContext.Database.GetDbConnection();
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                foreach(var change in dataContext.ChangeTracker.Entries<TEntity>().Where(
                  e => e.State != EntityState.Unchanged).ToList())
                {
                    // The insert, update, and delete methods, handle the state change so we don't have to do it here
                    switch(change.State)
                    {
                        case EntityState.Added:
                            dataContext.InsertEntity(change.Entity);
                            break;

                        case EntityState.Modified:
                            dataContext.UpdateEntity(change.Entity);
                            break;

                        case EntityState.Deleted:
                            dataContext.DeleteEntity(change.Entity);
                            break;

                        default:
                            break;
                    }
                }
            }
            finally
            {
                if(closeConnection)
                    connection.Close();
            }
        }

        /// <summary>
        /// Submit all tracked add, update, and delete changes asynchronously for the given entity type using the
        /// stored procedures defined on the entity type with the <see cref="InsertEntityStoredProcedureAttribute"/>,
        /// <see cref="UpdateEntityStoredProcedureAttribute"/>, and <see cref="DeleteEntityStoredProcedureAttribute"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type for which to submit changes</typeparam>
        /// <param name="dataContext">The data context to use for the operations</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>This will get the changed entities from the data context's change tracker and submit them
        /// accordingly.  The state of the entities is also updated to reflect that they are in an unchanged
        /// state after being added or updated or detached if deleted.  If the connection is not in an open
        /// state, it is opened temporarily while performing the actions.</remarks>
        /// <example>
        /// <code language="cs">
        /// if(dataContext.HasChanges())
        ///     await dataContext.SubmitChangesAsync&lt;Account&gt;();
        /// </code>
        /// </example>
        public static async Task SubmitChangesAsync<TEntity>(this DbContext dataContext,
          CancellationToken cancellationToken = default) where TEntity : ChangeTrackingEntity
        {
            // Any changes made here should also be made to SubmitChanges(dataContext) if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            var connection = dataContext.Database.GetDbConnection();
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                foreach(var change in dataContext.ChangeTracker.Entries<TEntity>().Where(
                  e => e.State != EntityState.Unchanged).ToList())
                {
                    // The insert, update, and delete methods, handle the state change so we don't have to do it here
                    switch(change.State)
                    {
                        case EntityState.Added:
                            await dataContext.InsertEntityAsync(change.Entity, cancellationToken).ConfigureAwait(false);
                            break;

                        case EntityState.Modified:
                            await dataContext.UpdateEntityAsync(change.Entity, cancellationToken).ConfigureAwait(false);
                            break;

                        case EntityState.Deleted:
                            await dataContext.DeleteEntityAsync(change.Entity, cancellationToken).ConfigureAwait(false);
                            break;

                        default:
                            break;
                    }
                }
            }
            finally
            {
#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }
        }

        /// <summary>
        /// Submit all tracked add, update, and delete changes for the given entity type using supplied functions
        /// that allow for custom handling of the operations.
        /// </summary>
        /// <typeparam name="TEntity">The entity type for which to submit changes</typeparam>
        /// <param name="dataContext">The data context to use for the operations</param>
        /// <param name="insert">The function to invoke to handle insertions.  It is passed the entity change
        /// entry and should return true if the insertion was made or false if not.  If null, the action is
        /// ignored.</param>
        /// <param name="update">The function to invoke to handle updates.  It is passed the entity change
        /// entry and should return true if the update was made or false if not.    If null, the action is
        /// ignored.</param>
        /// <param name="delete">The function to invoke to handle deletions.  It is passed the entity change
        /// entry and should return true if the delete was made or false if not.  If null, the action is
        /// ignored.</param>
        /// <remarks>This will get the changed entities from the data context's change tracker and submit them
        /// accordingly using the given functions.  If the corresponding function returns true, the state of the
        /// entity is updated to reflect that it is in an unchanged state after being added or updated or
        /// detached if deleted.  If the connection is not in an open state, it is opened temporarily while
        /// performing the actions.</remarks>
        /// <example>
        /// <code language="cs">
        /// if(dataContext.HasChanges())
        /// {
        ///     // Submit changes using stored procedure methods on the data context
        ///     dataContext.SubmitChanges&lt;StateCode&gt;(
        ///         se =&gt;
        ///         {
        ///             dataContext.spStateCodeAddUpdate(null, se.Entity.State, se.Entity.StateDesc);
        ///             return true;
        ///         },
        ///         se =&gt;
        ///         {
        ///             dataContext.spStateCodeAddUpdate((string?)se.OriginalValues[nameof(StateCode.State)],
        ///                 se.Entity.State, se.Entity.StateDesc);
        ///             return true;
        ///         },
        ///         se =&gt;
        ///         {
        ///             dataContext.spStateCodeDelete((string?)se.OriginalValues[nameof(StateCode.State)]);
        ///             return true;
        ///         });
        /// }
        /// </code>
        /// </example>
        public static void SubmitChanges<TEntity>(this DbContext dataContext,
          Func<EntityEntry<TEntity>, bool>? insert, Func<EntityEntry<TEntity>, bool>? update,
          Func<EntityEntry<TEntity>, bool>? delete) where TEntity : ChangeTrackingEntity
        {
            // Any changes made here should also be made to SubmitChangesAsync(dataContext, insert, ...) if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            var connection = dataContext.Database.GetDbConnection();
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                foreach(var change in dataContext.ChangeTracker.Entries<TEntity>().Where(
                  e => e.State != EntityState.Unchanged).ToList())
                {
                    switch(change.State)
                    {
                        case EntityState.Added:
                            if(insert?.Invoke(change) ?? false)
                            {
                                // We must set the original values to the new values first before marking it unchanged
                                // or it reverts the values to the original unmodified values.
                                change.OriginalValues.SetValues(change.Entity);
                                change.State = EntityState.Unchanged;
                            }
                            break;

                        case EntityState.Modified:
                            if(update?.Invoke(change) ?? false)
                            {
                                // We must set the original values to the new values first before marking it unchanged
                                // or it reverts the values to the original unmodified values.
                                change.OriginalValues.SetValues(change.Entity);
                                change.State = EntityState.Unchanged;
                            }
                            break;

                        case EntityState.Deleted:
                            if(delete?.Invoke(change) ?? false)
                            {
                                // No need to update the original values here
                                change.State = EntityState.Unchanged;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            finally
            {
                if(closeConnection)
                    connection.Close();
            }
        }

        /// <summary>
        /// Submit all tracked add, update, and delete changes asynchronously for the given entity type using
        /// supplied functions that allow for custom handling of the operations.
        /// </summary>
        /// <typeparam name="TEntity">The entity type for which to submit changes</typeparam>
        /// <param name="dataContext">The data context to use for the operations</param>
        /// <param name="insert">The asynchronous function to invoke to handle insertions.  It is passed the
        /// entity change entry and should return true if the insertion was made or false if not.  If null, the
        /// action is ignored.</param>
        /// <param name="update">The asynchronous function to invoke to handle updates.  It is passed the entity
        /// change entry and should return true if the update was made or false if not.    If null, the action is
        /// ignored.</param>
        /// <param name="delete">The asynchronous function to invoke to handle deletions.  It is passed the
        /// entity change entry and should return true if the delete was made or false if not.  If null, the
        /// action is ignored.</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>This will get the changed entities from the data context's change tracker and submit them
        /// accordingly using the given functions.  If the corresponding function returns true, the state of the
        /// entity is updated to reflect that it is in an unchanged state after being added or updated or
        /// detached if deleted.  If the connection is not in an open state, it is opened temporarily while
        /// performing the actions.</remarks>
        /// <example>
        /// <code language="cs">
        /// if(dataContext.HasChanges())
        /// {
        ///     // Submit changes using stored procedure methods on the data context
        ///     dataContext.SubmitChangesAsync&lt;StateCode&gt;(
        ///         async se =&gt;
        ///         {
        ///             await dataContext.spStateCodeAddUpdate(null, se.Entity.State, se.Entity.StateDesc);
        ///             return true;
        ///         },
        ///         async se =&gt;
        ///         {
        ///             await dataContext.spStateCodeAddUpdate((string?)se.OriginalValues[nameof(StateCode.State)],
        ///                 se.Entity.State, se.Entity.StateDesc);
        ///             return true;
        ///         },
        ///         async se =&gt;
        ///         {
        ///             await dataContext.spStateCodeDelete((string?)se.OriginalValues[nameof(StateCode.State)]);
        ///             return true;
        ///         });
        /// }
        /// </code>
        /// </example>
        public static async Task SubmitChangesAsync<TEntity>(this DbContext dataContext,
          Func<EntityEntry<TEntity>, Task<bool>>? insert,
          Func<EntityEntry<TEntity>, Task<bool>>? update,
          Func<EntityEntry<TEntity>, Task<bool>>? delete,
          CancellationToken cancellationToken = default) where TEntity : ChangeTrackingEntity
        {
            // Any changes made here should also be made to SubmitChanges(dataContext, insert, ...) if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            var connection = dataContext.Database.GetDbConnection();
            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                foreach(var change in dataContext.ChangeTracker.Entries<TEntity>().Where(
                  e => e.State != EntityState.Unchanged).ToList())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    switch(change.State)
                    {
                        case EntityState.Added:
                            if(insert != null && await insert(change).ConfigureAwait(false))
                            {
                                // We must set the original values to the new values first before marking it unchanged
                                // or it reverts the values to the original unmodified values.
                                change.OriginalValues.SetValues(change.Entity);
                                change.State = EntityState.Unchanged;
                            }
                            break;

                        case EntityState.Modified:
                            if(update != null && await update(change).ConfigureAwait(false))
                            {
                                // We must set the original values to the new values first before marking it unchanged
                                // or it reverts the values to the original unmodified values.
                                change.OriginalValues.SetValues(change.Entity);
                                change.State = EntityState.Unchanged;
                            }
                            break;

                        case EntityState.Deleted:
                            if(delete != null && await delete(change).ConfigureAwait(false))
                            {
                                // No need to update the original values here
                                change.State = EntityState.Unchanged;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            finally
            {
#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }
        }

        /// <summary>
        /// Execute a non-query stored procedure associated with a method on a data context and return the
        /// stored procedure's return value, number of rows affected, and optionally output parameter values.
        /// </summary>
        /// <param name="dataContext">The data context on which to execute the stored procedure</param>
        /// <param name="methodInfo">The method info for the calling method</param>
        /// <param name="parameters">Zero or more parameter values to be passed to the stored procedure.  These
        /// must match the parameter order of the calling data context method.</param>
        /// <returns>A tuple containing the number of rows affected assuming the stored procedure is not using
        /// <c>SET NOCOUNT ON</c>,  the return value of the stored procedure if any, and a dictionary containing
        /// any output parameters indexed by method parameter name with the value being the output value from the
        /// stored procedure.</returns>
        /// <remarks><para>The stored procedure name is determined by looking for the
        /// <see cref="MethodStoredProcedureAttribute"/> on the calling data context method.  If not specified,
        /// the stored procedure name is assumed to be the same as the calling method's name.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while executing the stored
        /// procedure.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// // Execute a stored procedure and return its return value
        /// public int spStockAdd(string symbol, string assetDescription, decimal currentBid,
        ///   decimal currentAsk, decimal priceChangePercent)
        /// {
        ///     return this.ExecuteMethodNonQuery((MethodInfo)MethodInfo.GetCurrentMethod()!,
        ///         symbol, assetDescription, currentBid, currentAsk, priceChangePercent).ReturnValue;
        /// }
        /// 
        /// // Execute a stored procedure and return the number of rows affected
        /// public int spStockDelete(string symbol)
        /// {
        ///     return this.ExecuteMethodNonQuery((MethodInfo)MethodInfo.GetCurrentMethod()!,
        ///         symbol).RowsAffected;
        /// }
        /// 
        /// // Execute a stored procedure and return the output parameters via the ref parameters on
        /// // the method.  We can also return the stored procedure's return value or rows affected.
        /// public int spCheckForEmployeeSchedule(string bidGroup, int entityKey,
        ///   ref bool bidGroupScheduled, ref bool entityScheduled)
        /// {
        ///     var result = this.ExecuteMethodNonQuery((MethodInfo)MethodInfo.GetCurrentMethod()!,
        ///         bidGroup, entityKey, bidGroupScheduled, entityScheduled);
        ///
        ///     bidGroupScheduled = (bool)result.OutputValues[nameof(bidGroupScheduled);
        ///     entityScheduled = (bool)result.OutputValues[nameof(entityScheduled);
        ///
        ///     return result.ReturnValue;
        /// }
        /// </code>
        /// </example>
        public static (int RowsAffected, int ReturnValue, IReadOnlyDictionary<string, object?> OutputValues)ExecuteMethodNonQuery(
          this DbContext dataContext, MethodInfo methodInfo, params object?[] parameters)
        {
            // Any changes made here should also be made to ExecuteNonMethodQueryAsync if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(methodInfo);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
#endif
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            var inOutParams = PrepareMethodCommand(dataContext, command, methodInfo, parameters);

            bool closeConnection = false;
            int rowsAffected = 0;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                rowsAffected = command.ExecuteNonQuery();
            }
            finally
            {
                if(closeConnection)
                    connection.Close();
            }

            var outputValues = inOutParams.ToDictionary(
                k => k.ParameterName,
                v =>
                {
                    var value = command.Parameters[v.ParameterIndex].Value;

                    if(value == DBNull.Value)
                        value = null;

                    return value;
                });

            return (rowsAffected, (int)command.Parameters["@___returnValue"].Value!, outputValues);
        }

        /// <summary>
        /// Execute a non-query stored procedure associated with a method on a data context asynchronously and
        /// return the stored procedure's return value, number of rows affected, and optionally output parameter
        /// values.
        /// </summary>
        /// <param name="dataContext">The data context on which to execute the stored procedure</param>
        /// <param name="methodInfo">The method info for the calling method</param>
        /// <param name="parameters">Zero or more parameter values to be passed to the stored procedure.  These
        /// must match the parameter order of the calling data context method.</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>A tuple containing the number of rows affected assuming the stored procedure is not using
        /// <c>SET NOCOUNT ON</c>,  the return value of the stored procedure if any, and a dictionary containing
        /// any output parameters indexed by method parameter name with the value being the output value from the
        /// stored procedure.</returns>
        /// <remarks><para>The stored procedure name is determined by looking for the
        /// <see cref="MethodStoredProcedureAttribute"/> on the calling data context method.  If not specified,
        /// the stored procedure name is assumed to be the same as the calling method's name.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while executing the stored
        /// procedure.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// // Execute a stored procedure and return its return value
        /// [MethodStoredProcedure("spStockAdd")]
        /// public async int spStockAddAsync(string symbol, string assetDescription,
        ///   decimal currentBid, decimal currentAsk, decimal priceChangePercent)
        /// {
        ///     // When called asynchronously, the parameters must be passed as an array
        ///     // and we must get the method info from the stack trace as we're inside the
        ///     // compiler generated state machine at this point.  We must also specify the
        ///     // stored procedure name in the method attribute if the method name does not
        ///     // match the stored procedure name.
        ///     var methodInfo = (MethodInfo)(new StackTrace().GetFrames().Select(
        ///         frame => frame.GetMethod()).FirstOrDefault(
        ///             item => item!.DeclaringType == GetType()) ??
        ///                throw new InvalidOperationException("Unable to get async method info"));
        ///
        ///     var result = await this.ExecuteMethodNonQueryAsync(methodInfo, [symbol, assetDescription,
        ///         currentBid, currentAsk, priceChangePercent]);
        ///         
        ///     return result.ReturnValue;
        /// }
        /// 
        /// // Execute a stored procedure and return the number of rows affected
        /// [MethodStoredProcedure("spStockDelete")]
        /// public async int spStockDeleteAsync(string symbol)
        /// {
        ///     // When called asynchronously, the parameters must be passed as an array
        ///     // and we must get the method info from the stack trace as we're inside the
        ///     // compiler generated state machine at this point.  We must also specify the
        ///     // stored procedure name in the method attribute if the method name does not
        ///     // match the stored procedure name.
        ///     var methodInfo = (MethodInfo)(new StackTrace().GetFrames().Select(
        ///         f => f.GetMethod()).FirstOrDefault(m => m!.DeclaringType == GetType()) ??
        ///             throw new InvalidOperationException("Unable to get async method info"));
        ///
        ///     var result = await this.ExecuteMethodNonQueryAsync(methodInfo, [symbol]);
        ///     
        ///     return result.RowsAffected;
        /// }
        /// 
        /// // Execute a stored procedure and return the output parameters via the ref parameters on
        /// // the method.  We can also return the stored procedure's return value or rows affected.
        /// [MethodStoredProcedure("spCheckForEmployeeSchedule")]
        /// public async int spCheckForEmployeeScheduleAsync(string bidGroup, int entityKey,
        ///   ref bool bidGroupScheduled, ref bool entityScheduled)
        /// {
        ///     // When called asynchronously, the parameters must be passed as an array
        ///     // and we must get the method info from the stack trace as we're inside the
        ///     // compiler generated state machine at this point.  We must also specify the
        ///     // stored procedure name in the method attribute if the method name does not
        ///     // match the stored procedure name.
        ///     var methodInfo = (MethodInfo)(new StackTrace().GetFrames().Select(
        ///         f => f.GetMethod()).FirstOrDefault(m => m!.DeclaringType == GetType()) ??
        ///             throw new InvalidOperationException("Unable to get async method info"));
        ///
        ///     var result = await this.ExecuteMethodNonQueryAsync(methodInfo, [bidGroup, entityKey,
        ///         bidGroupScheduled, entityScheduled]);
        ///
        ///     bidGroupScheduled = (bool)result.OutputValues[nameof(bidGroupScheduled);
        ///     entityScheduled = (bool)result.OutputValues[nameof(entityScheduled);
        ///
        ///     return result.ReturnValue;
        /// }
        /// </code>
        /// </example>
        public static async Task<(int RowsAffected, int ReturnValue, IReadOnlyDictionary<string, object?> OutputValues)>
            ExecuteMethodNonQueryAsync(this DbContext dataContext, MethodInfo methodInfo,
              object?[] parameters, CancellationToken cancellationToken = default)
        {
            // Any changes made here should also be made to ExecuteNonMethodQuery if necessary
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(methodInfo);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
#endif
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            var inOutParams = PrepareMethodCommand(dataContext, command, methodInfo, parameters);

            bool closeConnection = false;
            int rowsAffected = 0;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }

            var outputValues = inOutParams.ToDictionary(
                k => k.ParameterName,
                v =>
                {
                    var value = command.Parameters[v.ParameterIndex].Value;

                    if(value == DBNull.Value)
                        value = null;

                    return value;
                });

            return (rowsAffected, (int)command.Parameters["@___returnValue"].Value!, outputValues);
        }

        /// <summary>   
        /// Execute a query stored procedure associated with a method on a data context and return the
        /// stored procedure's result set as an enumerable list of the given entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type returned by the query</typeparam>
        /// <param name="dataContext">The data context on which to execute the stored procedure</param>
        /// <param name="methodInfo">The method info for the calling method</param>
        /// <param name="parameters">Zero or more parameter values to be passed to the stored procedure.  These
        /// must match the parameter order of the calling data context method.</param>
        /// <returns>An enumerable list of the given entity type.</returns>
        /// <remarks><para>The stored procedure name is determined by looking for the
        /// <see cref="MethodStoredProcedureAttribute"/> on the calling data context method.  If not specified,
        /// the stored procedure name is assumed to be the same as the calling method's name.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while loading the entities.
        /// If change tracking is enabled on the data context, changes to the entities will be tracked.  If not
        /// or the entity is marked with the <see cref="NeverTrackAttribute"/>, they will not be tracked.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// // Execute a search stored procedure and return its result set
        /// public IEnumerable&lt;spTransactionListResult&gt; spTransactionList(int accountKey,
        ///   string? symbol, DateTime fromDate, DateTime toDate, string? txType)
        /// {
        ///     return this.ExecuteMethodQuery&lt;spTransactionListResult&gt;(
        ///         (MethodInfo)MethodInfo.GetCurrentMethod()!, accountKey, symbol,
        ///         fromDate, toDate, txType);
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> ExecuteMethodQuery<TEntity>(
          this DbContext dataContext, MethodInfo methodInfo, params object?[] parameters) where TEntity : class, new()
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(methodInfo);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
#endif
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            // I suppose it's possible to have output parameters and return a result set but it's probably a
            // rare case so we're not going to handle it here for now.
            PrepareMethodCommand(dataContext, command, methodInfo, parameters);

            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                Type entityType = typeof(TEntity);
                bool neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().Any();
                var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);

                using var reader = command.ExecuteReader();

                while(reader.Read())
                {
                    var entity = (TEntity)Activator.CreateInstance(entityType)!;

                    for(int column = 0; column < reader.FieldCount; column++)
                    {
                        object? value = reader.GetValue(column);

                        // Set the entity values based on what is returned by the stored procedure rather than
                        // what's in the entity.  This allows us to use different stored procedures with a common
                        // entity type even if the stored procedures don't return the same set of columns.
                        if(value != null && properties.TryGetValue(reader.GetName(column), out var p))
                            p.SetValueFromDatabase(entity, value);
                    }

                    if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                        dataContext.Attach(entity);

                    yield return entity;
                }
            }
            finally
            {
                if(closeConnection)
                    connection.Close();
            }
        }

        /// <summary>   
        /// Execute a query stored procedure associated with a method on a data context and return the
        /// stored procedure's result set asynchronously as an enumerable list of the given entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type returned by the query</typeparam>
        /// <param name="dataContext">The data context on which to execute the stored procedure</param>
        /// <param name="methodInfo">The method info for the calling method</param>
        /// <param name="parameters">Zero or more parameter values to be passed to the stored procedure.  These
        /// must match the parameter order of the calling data context method.</param>
        /// <param name="cancellationToken">An optional cancellation token</param>
        /// <returns>An enumerable list of the given entity type.</returns>
        /// <remarks><para>The stored procedure name is determined by looking for the
        /// <see cref="MethodStoredProcedureAttribute"/> on the calling data context method.  If not specified,
        /// the stored procedure name is assumed to be the same as the calling method's name.</para>
        /// 
        /// <para>If the connection is not in an open state, it is opened temporarily while loading the entities.
        /// If change tracking is enabled on the data context, changes to the entities will be tracked.  If not
        /// or the entity is marked with the <see cref="NeverTrackAttribute"/>, they will not be tracked.</para></remarks>
        /// <example>
        /// <code language="cs">
        /// // Execute a search stored procedure and return its result set
        /// [MethodStoredProcedure("spTransactionList")]
        /// public IAsyncEnumerable&lt;spTransactionListResult&gt; spTransactionListAsync(int accountKey,
        ///   string? symbol, DateTime fromDate, DateTime toDate, string? txType)
        /// {
        ///     // When called asynchronously, the parameters must be passed as an array
        ///     // and we must get the method info from the stack trace as we're inside the
        ///     // compiler generated state machine at this point.  We must also specify the
        ///     // stored procedure name in the method attribute if the method name does not
        ///     // match the stored procedure name.
        ///     var methodInfo = (MethodInfo)(new StackTrace().GetFrames().Select(
        ///         f => f.GetMethod()).FirstOrDefault(m => m!.DeclaringType == GetType()) ??
        ///             throw new InvalidOperationException("Unable to get async method info"));
        ///
        ///     // Note that we can't pass a cancellation token as it would look like one of
        ///     // the method parameters.  Use the WithCancellation() extension method on the
        ///     // call to this method instead.
        ///     return this.ExecuteMethodQueryAsync&lt;spTransactionListResult&gt;(methodInfo,
        ///         [accountKey, symbol, fromDate, toDate, txType]);
        /// }
        /// 
        /// // We can't pass the cancellation token to the query method as it will look like
        /// // a parameter to the stored procedure.  We need to use the WithCancellation()
        /// // extension method instead.
        /// var cts = new CancellationTokenSource();
        /// 
        /// await foreach(var t in dc.spTransactionListAsync(1, "MSFT", fromDate,
        ///     toDate, null).WithCancellation(cts.Token))
        /// {
        ///     ....
        /// }
        /// </code>
        /// </example>
        public static async IAsyncEnumerable<TEntity> ExecuteMethodQueryAsync<TEntity>(
          this DbContext dataContext, MethodInfo methodInfo, object?[] parameters,
          [EnumeratorCancellation] CancellationToken cancellationToken = default) where TEntity : class, new()
        {
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(methodInfo);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
#endif
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            // I suppose it's possible to have output parameters and return a result set but it's probably a
            // rare case so we're not going to handle it here for now.
            PrepareMethodCommand(dataContext, command, methodInfo, parameters);

            bool closeConnection = false;

            try
            {
                // If the connection is already open, we won't close it when done
                if(connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    closeConnection = true;
                }

                Type entityType = typeof(TEntity);
                bool neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().Any();
                var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);

                using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                while(await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var entity = (TEntity)Activator.CreateInstance(entityType)!;

                    for(int column = 0; column < reader.FieldCount; column++)
                    {
                        object? value = reader.GetValue(column);

                        // Set the entity values based on what is returned by the stored procedure rather than
                        // what's in the entity.  This allows us to use different stored procedures with a common
                        // entity type even if the stored procedures don't return the same set of columns.
                        if(value != null && properties.TryGetValue(reader.GetName(column), out var p))
                            p.SetValueFromDatabase(entity, value);
                    }

                    if(!neverTrack && dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                        dataContext.Attach(entity);

                    yield return entity;
                }
            }
            finally
            {
#if !NETSTANDARD2_0
                if(closeConnection)
                    await connection.CloseAsync().ConfigureAwait(false);
#else
                if(closeConnection)
                    connection.Close();
#endif
            }
        }
        #endregion

        #region Result set extension methods
        //=====================================================================

        /// <summary>
        /// This converts an enumerable list of change tracking entities to a binding list that will notify the
        /// related data context of additions, changes, and deletions.  The returned list is suitable for binding
        /// to Windows Forms controls.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="entities">An enumerable list of the entities</param>
        /// <param name="dataContext">The data context that is tracking the items</param>
        /// <returns>A <see cref="TrackingBindingList{TEntity}"/> instance</returns>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        ///
        /// bsAccounts.DataSource = dc.LoadAll&lt;Account&gt;().ToTrackingBindingList(dataContext);
        /// </code>
        /// </example>
        public static TrackingBindingList<TEntity> ToTrackingBindingList<TEntity>(this IEnumerable<TEntity> entities,
          DbContext dataContext) where TEntity : ChangeTrackingEntity
        {
            return new TrackingBindingList<TEntity>(dataContext, [.. entities]);
        }

        /// <summary>
        /// This converts an enumerable list of change tracking entities to an observable collection that will
        /// notify the related data context of additions, changes, and deletions.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="entities">An enumerable list of the entities</param>
        /// <param name="dataContext">The data context that is tracking the items</param>
        /// <returns>A <see cref="TrackingObservableCollection{TEntity}"/> instance</returns>
        /// <remarks>
        /// <note type="important">The returned <see cref="TrackingObservableCollection{T}"/> will not work if
        /// bound to Windows Forms controls.  Use <see cref="ToTrackingBindingList{TEntity}"/> instead.</note>
        /// </remarks>
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// bsAccounts.DataSource = dc.LoadAll&lt;Account&gt;().ToTrackingCollection(dataContext);
        /// </code>
        /// </example>
        public static TrackingObservableCollection<TEntity> ToTrackingCollection<TEntity>(this IEnumerable<TEntity> entities,
          DbContext dataContext) where TEntity : ChangeTrackingEntity
        {
            return new TrackingObservableCollection<TEntity>(dataContext, entities);
        }
        #endregion

        #region Parameter value extension methods
        //=====================================================================

        /// <summary>
        /// This is used to convert strings to null values if they are empty
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>If the value is null or an empty string this returns <c>null</c>.  If it is not null or an
        /// empty string, it returns the passed value.</returns>
        /// <remarks>This is useful for passing string values to database context methods when the value needs to
        /// be stored as a null value rather than an empty string.</remarks>
        ///
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// dataContext.spOTWantedAddEdit(overtimeKey, entityKey,
        ///     dtpBeginDate.Value, dtpEndDate.Value, (byte)days,
        ///     (byte)shifts, txtWorksiteNote.Text.NullIfEmpty(),
        ///     txtRequestNote.Text.NullIfEmpty());
        /// </code>
        /// </example>
        public static string? NullIfEmpty(this string? value)
        {
            return String.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// This is used to convert strings to null values if they are empty or all whitespace
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>If the value is null, empty, or contains nothing but whitespace this returns <c>null</c>.
        /// If it is not null, empty, or all whitespace it returns the passed value.</returns>
        /// <remarks>This is useful for passing string values to database context methods when the value needs to
        /// be stored as a null value rather than an empty or whitespace string.</remarks>
        ///
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// dataContext.spOTWantedAddEdit(overtimeKey, entityKey,
        ///     dtpBeginDate.Value, dtpEndDate.Value, (byte)days,
        ///     (byte)shifts, txtWorksiteNote.Text.NullIfWhiteSpace(),
        ///     txtRequestNote.Text.NullIfWhiteSpace());
        /// </code>
        /// </example>
        public static string? NullIfWhiteSpace(this string? value)
        {
            return String.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>
        /// This is used to convert an object to a string and return either the string value if not empty, or
        /// null if it is an empty string.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>If the value is null, <c>DBNull.Value</c> or an empty string this returns <c>null</c>.  If
        /// not, it returns the string representation of the specified object.</returns>
        /// <remarks>This is useful for passing object values to database context methods when the value needs to
        /// be stored as a null value rather than an empty string.</remarks>
        ///
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// var resultSet = dc.spDrasticActions(dtpFromDate.Value, dtpToDate.Value,
        ///     cboActionType.SelectedValue.ToStringOrNull());
        /// </code>
        /// </example>
        public static string? ToStringOrNull(this object? value)
        {
            if(value == null || value == DBNull.Value)
                return null;

            string s = value.ToString()!;

            return (s.Length == 0) ? null : s;
        }

        /// <summary>
        /// This is used to convert a string value to a nullable <c>Char</c> by returning null if the string is
        /// null or empty or the first character of the string if not.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>If the value is null or an empty string, this returns <c>null</c>.  If not, it returns the
        /// first character of the string.</returns>
        ///
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// dataContext.spAddName(txtLastName.Text, txtFirstName.Text.NullIfWhiteSpace(),
        ///     txtRace.Text.ToChar(), txtSex.Text.ToChar(),
        ///     dtpDOB.BindableValue.ToNullable&lt;DateTime&gt;());
        /// </code>
        /// </example>
        public static char? ToChar(this string? value)
        {
            return !String.IsNullOrEmpty(value) ? value![0] : null;
        }

        /// <summary>
        /// This is used to convert value types to null values if they are set to their default value for the
        /// type (i.e. zero for integers, <c>DateTime.MinValue</c> for date/times, etc).
        /// </summary>
        /// <typeparam name="T">The data type to use</typeparam>
        /// <param name="value">The value to check</param>
        /// <returns>If the value is set to the default value for its type, this returns <c>null</c>.  If not, it
        /// returns the passed value.</returns>
        /// <remarks>This is useful for passing values to database context methods when the parameters needs to
        /// be stored as a null value rather than a literal value if it is set to the default.</remarks>
        ///
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext();
        /// 
        /// dataContext.spUpdateStartDate(caseKey, startDate.ToNullable());
        /// </code>
        /// </example>
        /// <seealso cref="ToNullable{T}(Object)" />
        public static T? ToNullable<T>(this T value) where T : struct
        {
            T defValue = default;

            if(value.Equals(defValue))
                return null;

            return value;
        }

        /// <summary>
        /// This is used to convert objects to null values if they are equal to <c>null</c>,
        /// <see cref="DBNull.Value">DBNull.Value</see>, or the default value for the given type.
        /// </summary>
        /// <typeparam name="T">The data type to use</typeparam>
        /// <param name="value">The value to check</param>
        /// <returns>If the value is set to <c>null</c>, <c>DBNull.Value</c>, or the default value for the type,
        /// this returns <c>null</c>.  If not, it returns the passed value.</returns>
        /// <remarks>This is useful for passing values to database context methods when the parameters needs to
        /// be passed as a null rather than <c>DBNull.Value</c> or the type's default value.</remarks>
        ///
        /// <example>
        /// <code language="cs">
        /// using var dataContext = new MyDbContext())
        /// 
        /// // The BindableValue property returns an object so we specify the type
        /// // for conversion.
        /// dataContext.spUpdateStartDate(caseKey,
        ///     dtpStartDate.BindableValue.ToNullable&lt;DateTime&gt;());
        /// </code>
        /// </example>
        /// <seealso cref="ToNullable{T}(T)" />
        public static T? ToNullable<T>(this object? value) where T : struct
        {
            if(value == null || value == DBNull.Value)
                return null;

            T defValue = default;

            if(value.Equals(defValue))
                return null;

            return (T)value;
        }
        #endregion
    }
}
