﻿//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : DatabaseExtensions.cs
// Author  : Eric Woodruff
// Updated : 12/26/2024
//
// This file contains a class that contains extension methods for database objects
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/22/2024  EFW  Created the code
//===============================================================================================================

using System.Runtime.CompilerServices;

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

            return String.Join(schemaName.SchemaName, ".", storedProcedureName);
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

                    if(value != null)
                    {
                        var valueType = value.GetType();

                        // For nullable types, compare the underlying type to the value type
                        var underlyingType = Nullable.GetUnderlyingType(mp.ParameterType) ?? mp.ParameterType;

                        if(!underlyingType.Equals(valueType))
                        {
                            throw new InvalidOperationException($"Data type of method parameter {mp.Name} ({mp.ParameterType.Name}) " +
                                $"does not match the data type of the parameter value \"{value}\" ({valueType.Name})");
                        }
                    }

                    // If the parameter value is null, use DBNull.Value to send a NULL to the database rather than
                    // using any default value assigned to the parameter.
                    var p = new SqlParameter($"@{spName?.ParameterNamePrefix ?? contextParamPrefix?.Prefix}{columnName?.Name ?? mp.Name}",
                        value ?? DBNull.Value);
                    p.SetParameterType();

                    // If the method parameter is by reference, make the SQL parameter input/output
                    if(mp.ParameterType.IsByRef)
                    {
                        p.Direction = ParameterDirection.InputOutput;
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
                // Character values can be returned as single character strings.  If this looks like the case,
                // get the first character of the string to avoid an exception.  If the string is empty, use a
                // null character.
                if(value != null && (propInfo.PropertyType == typeof(char) ||
                  propInfo.PropertyType == typeof(char?)) && value.GetType() == typeof(string))
                {
                    string stringValue = (string)value;

                    value = (stringValue.Length != 0) ? stringValue[0] : '\x0';
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
        private static void SetParameterType(this SqlParameter parameter)
        {
            if(parameter.Value == null || parameter.Value == DBNull.Value)
                return;

            Type valueType = parameter.Value.GetType();

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
                    if(((DateTime)parameter.Value).Year < 1753)
                        parameter.SqlDbType = SqlDbType.DateTime2;
                    else
                        parameter.SqlDbType = SqlDbType.DateTime;
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
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
#endif
            Type entityType = typeof(TEntity);

            var spName = entityType.GetCustomAttributes<LoadAllStoredProcedureAttribute>().FirstOrDefault() ??
                throw new NotSupportedException("The specified entity type is not decorated with the " +
                    nameof(LoadAllStoredProcedureAttribute));

            var neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().FirstOrDefault();
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, spName.StoredProcedureName);

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
                var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);

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

                    if(neverTrack == null &&
                      dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                    {
                        dataContext.Attach(entity);
                    }

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
        /// Load all entities of the given type using a stored procedure defined on the entity type using the
        /// given key value(s).
        /// </summary>
        /// <typeparam name="TEntity">The entity type to load</typeparam>
        /// <param name="dataContext">The data context to use when loading the entities</param>
        /// <param name="parameters">One or more parameter values that will be passed to the stored procedure.
        /// The parameter order typically matches the declared key order on the entity or the parameter order of
        /// the stored procedure but does not have to.</param>
        /// <returns>An enumerable list of the <typeparamref name="TEntity"/> entities matching the key values</returns>
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
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(parameters);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(parameters == null)
                throw new ArgumentNullException(nameof(parameters));
#endif
            Type entityType = typeof(TEntity);

            var spName = entityType.GetCustomAttributes<LoadByKeyStoredProcedureAttribute>().FirstOrDefault() ??
                throw new NotSupportedException("The specified entity type is not decorated with the " +
                    nameof(LoadByKeyStoredProcedureAttribute));

            var namePrefix = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();
            var neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().FirstOrDefault();
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, spName.StoredProcedureName);

            var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);

            var keys = dataContext.Model.FindEntityType(entityType)?.FindPrimaryKey()?.Properties.Select(
                p => p.Name).ToList();

            // Get the primary key for the entity type
            if((keys?.Count ?? 0) == 0)
                throw new NotSupportedException("No primary key or key property is defined on the specified entity type");

            if(keys!.Count != parameters.Length)
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
                var param = new SqlParameter($"@{spName.ParameterNamePrefix ?? namePrefix?.Prefix}{columnName?.Name ?? key}",
                    value ?? DBNull.Value);
                param.SetParameterType();
                command.Parameters.Add(param);
            }

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

                    if(neverTrack == null &&
                      dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                    {
                        dataContext.Attach(entity);
                    }

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
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(entity);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
#endif
            Type entityType = typeof(TEntity);

            var spName = entityType.GetCustomAttributes<InsertEntityStoredProcedureAttribute>().FirstOrDefault() ??
                throw new NotSupportedException("The specified entity type is not decorated with the " +
                    nameof(InsertEntityStoredProcedureAttribute));

            var namePrefix = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();
            var neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().FirstOrDefault();
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, spName.StoredProcedureName);

            var properties = entityType.GetProperties();
            var inOutParams = new List<(PropertyInfo Property, SqlParameter Parameter)>();

            var keys = dataContext.Model.FindEntityType(entityType)?.FindPrimaryKey()?.Properties.Select(
                p => p.Name).ToList();

            // Get the primary key for the entity type
            if((keys?.Count ?? 0) == 0)
                throw new NotSupportedException("No primary key or key property is defined on the specified entity type");

            // Add a parameter for each public property unless it is ignored for updates
            foreach(var p in properties)
            {
                var ignored = p.GetCustomAttribute<IgnoreAttribute>();

                if(!(ignored?.ForInsert ?? false))
                {
                    // If the property has a column attribute, use the name from it instead
                    var columnName = p.GetCustomAttribute<ColumnAttribute>();
                    var timestamp = p.GetCustomAttribute<TimestampAttribute>();

                    // If the parameter value is null, use DBNull.Value to send a NULL to the database rather than
                    // using any default value assigned to the parameter.
                    var param = new SqlParameter($"@{spName.ParameterNamePrefix ?? namePrefix?.Prefix}{columnName?.Name ?? p.Name}",
                        p.GetValue(entity) ?? DBNull.Value);
                    param.SetParameterType();

                    // If it's a time stamp or a single key column that looks like an identity column, make it an
                    // input/output parameter.  Primary keys with multiple columns or non-integer keys are not
                    // assumed to be output parameters.
                    if(timestamp != null || (keys!.Count == 1 && param.SqlDbType == SqlDbType.Int && keys[0] == p.Name))
                    {
                        param.Direction = ParameterDirection.InputOutput;
                        inOutParams.Add((p, param));
                    }

                    command.Parameters.Add(param);
                }
            }

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
                if(closeConnection)
                    connection.Close();
            }

            if(neverTrack == null &&
              dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
            {
                var changeEntry = dataContext.Entry(entity);

                if(changeEntry != null)
                {
                    // We must set the original values to the new values first before marking it unchanged
                    // or it reverts the values to the original unmodified values.
                    changeEntry.OriginalValues.SetValues(changeEntry.Entity);
                    changeEntry.State = EntityState.Unchanged;
                }
            }

            return rowsAffected;
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
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(entity);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
#endif
            Type entityType = typeof(TEntity);

            var spName = entityType.GetCustomAttributes<UpdateEntityStoredProcedureAttribute>().FirstOrDefault() ??
                throw new NotSupportedException("The specified entity type is not decorated with the " +
                    nameof(UpdateEntityStoredProcedureAttribute));

            var namePrefix = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();
            var neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().FirstOrDefault();
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, spName.StoredProcedureName);

            var properties = entityType.GetProperties();
            var inOutParams = new List<(PropertyInfo Property, SqlParameter Parameter)>();

            // Add a parameter for each public property unless it is ignored for updates
            foreach(var p in properties)
            {
                var ignored = p.GetCustomAttribute<IgnoreAttribute>();

                if(!(ignored?.ForUpdate ?? false))
                {
                    // If the property has a column attribute, use the name from it instead
                    var columnName = p.GetCustomAttribute<ColumnAttribute>();
                    var timestamp = p.GetCustomAttribute<TimestampAttribute>();

                    // If the parameter value is null, use DBNull.Value to send a NULL to the database rather than
                    // using any default value assigned to the parameter.
                    var param = new SqlParameter($"@{spName.ParameterNamePrefix ?? namePrefix?.Prefix}{columnName?.Name ?? p.Name}",
                        p.GetValue(entity) ?? DBNull.Value);
                    param.SetParameterType();

                    // If it's a time stamp make it an input/output parameter
                    if(timestamp != null)
                    {
                        param.Direction = ParameterDirection.InputOutput;
                        inOutParams.Add((p, param));
                    }

                    command.Parameters.Add(param);
                }
            }

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
                if(closeConnection)
                    connection.Close();
            }

            if(neverTrack == null &&
              dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
            {
                var changeEntry = dataContext.Entry(entity);

                if(changeEntry != null)
                {
                    // We must set the original values to the new values first before marking it unchanged
                    // or it reverts the values to the original unmodified values.
                    changeEntry.OriginalValues.SetValues(changeEntry.Entity);
                    changeEntry.State = EntityState.Unchanged;
                }
            }

            return rowsAffected;
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
#if !NETSTANDARD2_0
            ArgumentNullException.ThrowIfNull(dataContext);
            ArgumentNullException.ThrowIfNull(entity);
#else
            if(dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
#endif
            Type entityType = typeof(TEntity);

            var spName = entityType.GetCustomAttributes<DeleteEntityStoredProcedureAttribute>().FirstOrDefault() ??
                throw new NotSupportedException("The specified entity type is not decorated with the " +
                    nameof(DeleteEntityStoredProcedureAttribute));

            var namePrefix = dataContext.GetType().GetCustomAttribute<ParameterNamePrefixAttribute>();
            var neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().FirstOrDefault();
            var connection = dataContext.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = CreateStoredProcedureName(dataContext, spName.StoredProcedureName);

            var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);

            var keys = dataContext.Model.FindEntityType(entityType)?.FindPrimaryKey()?.Properties.Select(
                p => p.Name).ToList();

            // Get the primary key for the entity type and add a parameter for each key field
            if((keys?.Count ?? 0) == 0)
                throw new NotSupportedException("No primary key or key property is defined on the specified entity type");

            foreach(string key in keys!)
            {
                if(!properties.TryGetValue(key, out var p))
                    throw new InvalidOperationException($"The key property {key} was not found on the entity");

                // If the property has a column attribute, use the name from it instead
                var columnName = p.GetCustomAttribute<ColumnAttribute>();

                // If the property value is null, use DBNull.Value to send a NULL to the database rather than
                // using any default value assigned to the parameter.
                var param = new SqlParameter($"@{spName.ParameterNamePrefix ?? namePrefix?.Prefix}{columnName?.Name ?? key}",
                    p.GetValue(entity) ?? DBNull.Value);
                param.SetParameterType();
                command.Parameters.Add(param);
            }

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
                if(closeConnection)
                    connection.Close();
            }

            if(neverTrack == null &&
              dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
            {
                var changeEntry = dataContext.Entry(entity);

                // No need to update the original values here
                if(changeEntry != null)
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
        ///     dataContext.SubmitEntityChanges&lt;Account&gt;();
        /// </code>
        /// </example>
        public static void SubmitChanges<TEntity>(this DbContext dataContext) where TEntity : ChangeTrackingEntity
        {
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
        ///         e =&gt;
        ///         {
        ///             dataContext.spStateCodeAddUpdate(null, e.Entity.State, e.Entity.StateDesc);
        ///             return true;
        ///         },
        ///         e =&gt;
        ///         {
        ///             dataContext.spStateCodeAddUpdate((string?)e.OriginalValues[nameof(StateCode.State)],
        ///                 e.Entity.State, e.Entity.StateDesc);
        ///             return true;
        ///         },
        ///         e =&gt;
        ///         {
        ///             dataContext.spStateCodeDelete((string?)e.OriginalValues[nameof(StateCode.State)]);
        ///             return true;
        ///         });
        /// }
        /// </code>
        /// </example>
        public static void SubmitChanges<TEntity>(this DbContext dataContext,
          Func<EntityEntry<TEntity>, bool> insert, Func<EntityEntry<TEntity>, bool> update,
          Func<EntityEntry<TEntity>, bool> delete) where TEntity : ChangeTrackingEntity
        {
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
                var neverTrack = entityType.GetCustomAttributes<NeverTrackAttribute>().FirstOrDefault();

                using var reader = command.ExecuteReader();
                var properties = entityType.GetProperties().ToDictionary(k => k.Name, v => v);

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

                    if(neverTrack == null &&
                      dataContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll)
                    {
                        dataContext.Attach(entity);
                    }

                    yield return entity;
                }

            }
            finally
            {
                if(closeConnection)
                    connection.Close();
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
            return new TrackingBindingList<TEntity>(dataContext, entities.ToList());
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
        public static Nullable<T> ToNullable<T>(this T value) where T : struct
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
        public static Nullable<T> ToNullable<T>(this object? value) where T : struct
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