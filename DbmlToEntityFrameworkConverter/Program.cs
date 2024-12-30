//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : Program.cs
// Author  : Eric Woodruff
// Updated : 12/19/2024
//
// This file contains a utility used to convert LINQ to SQL DBML file definitions to rough equivalents of their
// Entity Framework counterparts.
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/27/2024  EFW  Created the code
//===============================================================================================================

namespace DbmlToEntityFrameworkConverter
{
    /// <summary>
    /// This utility is used to convert LINQ to SQL DBML file definitions to rough equivalents of their Entity
    /// Framework counterparts.
    /// </summary>
    internal sealed class Program
    {
        #region Private data members
        //=====================================================================

        private static readonly XNamespace ns = "http://schemas.microsoft.com/linqtosql/dbml/2007";
        private static readonly Dictionary<string, XElement> tables = [];
        private static readonly Dictionary<string, TypeInfo> tableTypes = [];
        private static readonly List<StoredProcedureMethodInfo> functions = [];
        private static Dictionary<string, StoredProcedureMethodInfo> functionMap = null!;

        private static string outputFolder = null!, schemaName = "dbo", contextNamespace = null!,
            entityNamespace = null!, contextClassName = null!, accessModifier = null!, dbmlFilename = null!,
            nullForgiving = String.Empty;
        private static string? parameterPrefix, settingsObjectName, settingsPropertyName, connectionString;
        private static bool useFieldKeyword, nullableRefTypes, ef7orLater;

        #endregion

        #region Main program entry point
        //=====================================================================

        /// <summary>
        /// Main program entry point
        /// </summary>
        /// <param name="args">The command line arguments</param>
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("DbmlToEntityFrameworkConverter.exe PathToDbmlFile OutputFolder " +
                    "[/contextNamespace:Context.Namespace] [/entityNamespace:Entity.Namespace] " +
                    "[/schema:SchemaName] [/paramPrefix:ParameterPrefix] [/useFieldKeyword] [/nullableRefTypes] " +
                    "[/ef7orLater]");
                Console.WriteLine("PathToDbmlFile is required and specifies the DBML file to convert.");
                Console.WriteLine("OutputFolder is required and specifies where the generated files will be stored.");
                Console.WriteLine("/contextNamespace:Context.Namespace is optional.  If not specified, the " +
                    "context namespace from the DBML file is used if specified or 'Database' if not.");
                Console.WriteLine("/entityNamespace:Entity.Namespace is optional.  If not specified, the " +
                    "entity namespace from the DBML file is used if specified or the context namespace if not.");
                Console.WriteLine("/schema:SchemaName is optional.  If not specified, 'dbo' is used as the " +
                    "default schema.");
                Console.WriteLine("/paramPrefix:ParameterPrefix is optional.  If not specified, no common stored " +
                    "procedure parameter prefix is defined.");
                Console.WriteLine("/useFieldKeyword is optional (requires C# 13 preview or later).  If not " +
                    "specified, backing fields will be generated for entity properties that use change tracking.");
                Console.WriteLine("/nullableRefTypes is optional (requires C# 8 or later).  If not " +
                    "specified, defaults will not be added to non-nullable reference types and null forgiving " +
                    "operators will be omitted.");
                Console.WriteLine("/ef7orLater is optional (requires Entity Framework Core 7.0 or later).  This " +
                    "controls how certain elements such as key fields are rendered in the generated code.");
                Console.WriteLine("The DBML file to convert and the output folder must be the first and second " +
                    "parameters respectively.  Optional parameters can appear in any order.");
                return;
            }

            for(int i = 2; i < args.Length; i++)
            {
                if(args[i].StartsWith("/contextNamespace:", StringComparison.OrdinalIgnoreCase))
                    contextNamespace = args[i][18..].Trim();
                else if(args[i].StartsWith("/entityNamespace:", StringComparison.OrdinalIgnoreCase))
                    entityNamespace = args[i][17..].Trim();
                else if(args[i].StartsWith("/schema:", StringComparison.OrdinalIgnoreCase))
                    schemaName = args[i][8..].Trim();
                else if(args[i].StartsWith("/paramPrefix:", StringComparison.OrdinalIgnoreCase))
                    parameterPrefix = args[i][13..].Trim();
                else if(args[i].Equals("/useFieldKeyword", StringComparison.OrdinalIgnoreCase))
                    useFieldKeyword = true;
                else if(args[i].Equals("/nullableRefTypes", StringComparison.OrdinalIgnoreCase))
                {
                    nullableRefTypes = true;
                    nullForgiving = "!";
                }
                else if(args[i].Equals("/ef7orLater", StringComparison.OrdinalIgnoreCase))
                    ef7orLater = true;
                else
                {
                    Console.WriteLine("Unknown command line option: {0}", args[i]);
                    return;
                }
            }

            if(!File.Exists(args[0]))
            {
                Console.WriteLine("The specified DBML file does not exist");
                return;
            }

            outputFolder = args[1];

            if(!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            Console.WriteLine("Converting {0} from LINQ to SQL to Entity Framework Core", args[0]);
            Console.WriteLine("Generated source code files will be placed in {0}", outputFolder);
            Console.WriteLine("Default schema: {0}", schemaName);
            Console.WriteLine("Common parameter prefix: {0}", parameterPrefix ?? "(not used)");
            Console.WriteLine("Use field keyword: {0}", useFieldKeyword);
            Console.WriteLine("Nullable reference types: {0}", nullableRefTypes);
            Console.WriteLine("EF Core 7.0 or later: {0}", ef7orLater);

            dbmlFilename = args[0];

            var dbmlFile = XDocument.Load(dbmlFilename);
            var root = dbmlFile.Root!;
            var connection = root.Element(ns + "Connection");

            if(connection != null)
            {
                connectionString = connection.Attribute("ConnectionString")?.Value;
                settingsObjectName = connection.Attribute("SettingsObjectName")?.Value;
                settingsPropertyName = connection.Attribute("SettingsPropertyName")?.Value;
            }

            contextNamespace ??= root.Attribute("ContextNamespace")?.Value ?? "Database";
            entityNamespace ??= root.Attribute("EntityNamespace")?.Value ?? contextNamespace;
            contextClassName = root.Attribute("Class")!.Value;
            accessModifier = root.Attribute("AccessModifier")?.Value ?? "public";

            // Get all table entities
            foreach(var tableType in root.Descendants(ns + "Type"))
            {
                string? id = tableType.Attribute("Id")?.Value ?? tableType.Attribute("Name")!.Value;

                tables.Add(id, tableType);
            }

            // Add entities defined as the output of stored procedures that don't match an entity above
            foreach(var elementType in root.Descendants(ns + "ElementType"))
            {
                string? name = elementType.Attribute("Name")?.Value;

                if(name != null)
                    tables.Add(name, elementType);
            }

            Console.WriteLine("Obtained information for {0} table entities", tables.Count);

            // Generate the entity type information
            foreach(var tableInfo in tables)
            {
                var t = new TypeInfo(ns, tableInfo.Value);

                foreach(var p in t.Properties)
                {
                    if(!nullableRefTypes)
                    {
                        if(p.PropertyType == "string" || p.PropertyType == "byte[]" ||
                          p.PropertyType.StartsWith("System.", StringComparison.Ordinal))
                        {
                            p.IsNullable = false;
                        }
                    }
                }

                tableTypes.Add(tableInfo.Key, t);
            }

            // Generate functions that just return values or output parameters
            foreach(var f in root.Descendants(ns + "Function").Where(f => f.Element(ns + "ElementType") == null))
                functions.Add(CreateFunction(f));

            // Generate functions for result set methods and their corresponding result set type
            foreach(var t in tables.Values.Where(t => t.Name.LocalName == "ElementType"))
            {
                var function = CreateFunction(t.Parent!);
                    
                function.ResultSetType = tableTypes[t.Attribute("Name")!.Value];

                functions.Add(function);
            }

            // Generate functions for all methods that return a table entity
            foreach(var f in root.Descendants(ns + "Function").Where(f => f.Element(ns + "ElementType")?.Attribute("IdRef") != null))
            {
                var function = CreateFunction(f);

                function.ResultSetType = tableTypes[f.Element(ns + "ElementType")!.Attribute("IdRef")!.Value];

                functions.Add(function);
            }

            // Create a dictionary of the insert, update, and delete methods used by types
            functionMap = functions.Where(f => f.FunctionId != null).ToDictionary(k => k.FunctionId!, v => v);

            Console.WriteLine("Obtained information for {0} functions", functions.Count);
            Console.WriteLine("Generating entity type source code files");

            // Generate each of the table type source code files. We could probably do this with a code generator
            // or T4 templates or something but we'll keep it simple and just write out the text for C# only.
            // This isn't intended to be comprehensive or complex, just a quick and dirty way to get started.
            foreach(var tableType in tableTypes.Values)
                GenerateTypeSourceFile(tableType);

            // Generate the data context class
            Console.WriteLine("Generating data context source code file");

            GenerateDataContextSourceFile([.. tableTypes.Values], functions);
        }
        #endregion

        #region Helper methods
        //=====================================================================

        /// <summary>
        /// Create a data context function
        /// </summary>
        /// <param name="functionElement">The function element from which to create the function</param>
        /// <returns>The function information used to generate the code</returns>
        private static StoredProcedureMethodInfo CreateFunction(XElement functionElement)
        {
            string storedProcedureName = functionElement.Attribute("Name")!.Value,
                methodName = functionElement.Attribute("Method")!.Value;

            var functionInfo = new StoredProcedureMethodInfo
            {
                FunctionId = functionElement.Attribute("Id")?.Value,
                MethodName = methodName,
                HasReturnValue = functionElement.Element(ns + "Return") != null
            };

            if(!String.IsNullOrWhiteSpace(schemaName))
                methodName = schemaName + "." + methodName;

            // The stored procedure name only needs to be set if it differs from the method name
            if(methodName != storedProcedureName)
                functionInfo.StoredProcedureName = storedProcedureName;

            foreach(var p in functionElement.Elements(ns + "Parameter"))
            {
                var fp = new ParameterInfo(p, parameterPrefix);

                if(!nullableRefTypes)
                {
                    if(fp.ParameterType == "string" || fp.ParameterType == "byte[]" ||
                      fp.ParameterType.StartsWith("System.", StringComparison.Ordinal))
                    {
                        fp.IsNullable = false;
                    }
                }

                functionInfo.Parameters.Add(fp);
            }

            return functionInfo;
        }

        /// <summary>
        /// This is used to generate a source code file for the given table type
        /// </summary>
        /// <param name="tableType">The table type for which to generate a source code file</param>
        private static void GenerateTypeSourceFile(TypeInfo tableType)
        {
            StoredProcedureMethodInfo? insertMethod = null, updateMethod = null, deleteMethod = null;
            string sourceFile = Path.Combine(outputFolder, tableType.TypeName + ".cs");

            using var sw = new StreamWriter(sourceFile);

            sw.WriteLine($@"// Code generated by DBML To Entity Framework Converter tool
// https://github.com/EWSoftware/EntityFrameworkUtils
//
// This code will need to be reviewed and tested to fix up any issues.

// TODO: Add or remove using statements as needed
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using EWSoftware.EntityFramework;
using EWSoftware.EntityFramework.DataAnnotations;

namespace {entityNamespace}
{{");
            // If a source code file exists named after the entity.  Add a To Do comment to check it
            string tableSourceFile = Path.Combine(Path.GetDirectoryName(dbmlFilename)!, tableType.TypeName + ".cs");

            if(File.Exists(tableSourceFile))
                sw.WriteLine("    // TODO: Check {0} for additional code for the entity", tableSourceFile);

            // LoadAllStoredProcedure and LoadByKeyStoredProcedure attributes will have to be added manually as
            // we don't have enough info to add them.  Things will work though as they are likely already handled
            // by a stored procedure method.  Reviewing those will identify case were the method can be removed
            // and replaced by one of the above attributes.

            // If it has insert, update, or delete functions or a key field, assume it needs change tracking.
            // Otherwise, keep it a simple type that is never tracked.
            bool tracksChanges = tableType.Properties.Any(p => p.IsPrimaryKey);

            int keyFieldCount = tableType.Properties.Count(p => p.IsPrimaryKey);

            if(tableType.InsertFunction != null || tableType.UpdateFunction != null ||
              tableType.DeleteFunction != null || (ef7orLater && keyFieldCount > 1))
            {
                tracksChanges = true;

                sw.Write("    [");

                bool hasAttributes = false;

                // The primary key attribute is only available in EF Core 7 and later
                if(ef7orLater && keyFieldCount > 1)
                {
                    sw.Write("PrimaryKey({0})", String.Join(", ", tableType.Properties.Where(
                        p => p.IsPrimaryKey).Select(p => $"nameof({p.PropertyName})")));
                    hasAttributes = true;
                }

                if(tableType.InsertFunction != null)
                {
                    if(hasAttributes)
                        sw.Write(", ");

                    insertMethod = functionMap[tableType.InsertFunction.Attribute("FunctionId")!.Value];
                    insertMethod.IsInsertMethod = insertMethod.Parameters.All(
                        p => tableType.Properties.Exists(tp => tp.PropertyName.Equals(p.ParameterName,
                            StringComparison.OrdinalIgnoreCase)));

                    sw.Write($@"InsertEntityStoredProcedure(""{insertMethod.MethodName}"")");
                    hasAttributes = true;
                }

                if(tableType.UpdateFunction != null)
                {
                    if(hasAttributes)
                        sw.Write(", ");

                    updateMethod = functionMap[tableType.UpdateFunction.Attribute("FunctionId")!.Value];
                    updateMethod.IsUpdateMethod = updateMethod.Parameters.All(
                        p => tableType.Properties.Exists(tp => tp.PropertyName.Equals(p.ParameterName,
                            StringComparison.OrdinalIgnoreCase)));

                    sw.Write($@"UpdateEntityStoredProcedure(""{updateMethod.MethodName}"")");
                    hasAttributes = true;
                }

                if(tableType.DeleteFunction != null)
                {
                    if(hasAttributes)
                        sw.Write(", ");

                    deleteMethod = functionMap[tableType.DeleteFunction.Attribute("FunctionId")!.Value];
                    deleteMethod.IsDeleteMethod = deleteMethod.Parameters.All(
                        p => tableType.Properties.Exists(tp => tp.PropertyName.Equals(p.ParameterName,
                            StringComparison.OrdinalIgnoreCase)));

                    sw.Write($@"DeleteEntityStoredProcedure(""{deleteMethod.MethodName}"")");
                }

                sw.WriteLine("]");
            }
            else
            {
                if(!tracksChanges)
                    sw.WriteLine("    [NeverTrack]");
            }

            sw.Write($"    {accessModifier} sealed class {tableType.TypeName}");

            if(tracksChanges)
                sw.Write(" : ChangeTrackingEntity");

            sw.WriteLine("\r\n    {");

            // Write out each of the properties
            foreach(var p in tableType.Properties)
            {
                bool ignoreForInsert = false, ignoreForUpdate = false;

                if(insertMethod != null && !insertMethod.Parameters.Any(mp => mp.ParameterName.Equals(
                  p.PropertyName, StringComparison.OrdinalIgnoreCase)))
                {
                    ignoreForInsert = true;
                }

                if(updateMethod != null && !updateMethod.Parameters.Any(mp => mp.ParameterName.Equals(
                  p.PropertyName, StringComparison.OrdinalIgnoreCase)))
                {
                    ignoreForUpdate = true;
                }

                if(!useFieldKeyword && tracksChanges)
                {
                    sw.Write($"        private {p.PropertyType}{(p.IsNullable ? "?" : String.Empty)} {p.BackingFieldName}");

                    if(nullableRefTypes && !String.IsNullOrWhiteSpace(p.ReferenceTypeDefault))
                        sw.Write(" = {0}", p.ReferenceTypeDefault);

                    sw.WriteLine(";\r\n");
                }

                if((keyFieldCount == 1 && p.IsPrimaryKey) || ignoreForInsert || ignoreForUpdate)
                {
                    sw.Write("        [");

                    if(keyFieldCount == 1 && p.IsPrimaryKey)
                        sw.Write("Key");

                    if(ignoreForInsert || ignoreForUpdate)
                    {
                        if(p.IsPrimaryKey)
                            sw.Write(", ");

                        sw.Write($"Ignore({ignoreForInsert.ToString().ToLowerInvariant()}, {ignoreForUpdate.ToString().ToLowerInvariant()})");
                    }

                    sw.WriteLine("]");
                }

                sw.Write($"        public {p.PropertyType}{(p.IsNullable ? "?" : String.Empty)} {p.PropertyName}");

                if(!tracksChanges)
                    sw.Write(" { get; set; }");
                else
                {
                    if(useFieldKeyword)
                    {
                        sw.Write(@"
        {
            get;
            set => this.SetWithNotify(value, ref field);
        }");
                    }
                    else
                    {
                        sw.Write($@"
        {{
            get => {p.BackingFieldName};
            set => this.SetWithNotify(value, ref {p.BackingFieldName});
        }}");
                    }
                }

                if(nullableRefTypes && !String.IsNullOrWhiteSpace(p.ReferenceTypeDefault))
                    sw.Write(" = {0};", p.ReferenceTypeDefault);

                sw.WriteLine("\r\n");
            }

            // Write out each of the relationships
            foreach(var r in tableType.Relationships)
            {
                sw.WriteLine("        [Ignore(true, true)]");

                if(r.IsForeignKey || r.Cardinality == "One")
                {
                    sw.Write($"        public {r.Type} {r.Member} {{ get; set; }}");

                    if(nullableRefTypes)
                        sw.Write(" = null!;");

                    sw.WriteLine("\r\n");
                }
                else
                {
                    sw.Write($"        public ICollection<{r.Type}> {r.Member} {{ get; }}");

                    if(nullableRefTypes)
                        sw.WriteLine(" = [];");
                    else
                        sw.WriteLine($" = new List<{r.Type}>();");

                    sw.WriteLine();
                }
            }

            sw.WriteLine("    }\r\n}");
        }

        /// <summary>
        /// This is used to generate the data context source file
        /// </summary>
        /// <param name="tableTypes"></param>
        /// <param name="functions"></param>
        private static void GenerateDataContextSourceFile(List<TypeInfo> tableTypes, List<StoredProcedureMethodInfo> functions)
        {
            string sourceFile = Path.Combine(outputFolder, contextClassName + ".cs");

            using var sw = new StreamWriter(sourceFile);

            sw.WriteLine($@"// Code generated by DBML To Entity Framework Converter tool
// https://github.com/EWSoftware/EntityFrameworkUtils
//
// This code will need to be reviewed and tested to fix up any issues.

// TODO: Add or remove using statements as needed
using System;
using System.Collections.Generic;
using System.Reflection;

using EWSoftware.EntityFramework;
using EWSoftware.EntityFramework.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace {contextNamespace}
{{");
            // If a source code file exists named after the data context.  Add a To Do comment to check it for
            // additional code.
            string dbmlSourceFile = Path.ChangeExtension(dbmlFilename, ".cs");

            if(File.Exists(dbmlSourceFile))
                sw.WriteLine("    // TODO: Check {0} for additional code for the data context", dbmlSourceFile);

            if(parameterPrefix != null)
                sw.WriteLine($@"    [ParameterNamePrefix(""{parameterPrefix}"")]");

            sw.WriteLine($@"    {accessModifier} sealed class {contextClassName} : DbContext
    {{
        #region Tracked entities
        //=====================================================================

        // These entities utilize change tracking so a property is required here for them");

            // Add DbSet<TEntity> properties for each table type with an insert, update, or delete function.
            // Pluralization is rather involved and many don't use it so we'll let the user figure it out and
            // change the property names if wanted.
            foreach(var table in tableTypes.Where(t => t.InsertFunction != null || t.UpdateFunction != null || t.DeleteFunction != null))
                sw.WriteLine($"        public DbSet<{table.TypeName}> {table.TypeName} {{ get; set; }}" +
                    $"{(nullableRefTypes ? " = null!;" : String.Empty)}\r\n");

            // Add the OnConfiguring overload
            string databaseConnection;

            if(!String.IsNullOrWhiteSpace(settingsObjectName) && !String.IsNullOrWhiteSpace(settingsPropertyName))
                databaseConnection = $"{settingsObjectName}.Default.{settingsPropertyName}";
            else
            {
                if(!String.IsNullOrWhiteSpace(connectionString))
                    databaseConnection = $@"""{connectionString}""";
                else
                    databaseConnection = @"""Undetermined""";
            }

            sw.WriteLine($@"        #endregion

        #region Method overrides
        //=====================================================================

        /// <inheritdoc />
        /// <remarks>This is overridden to set the connection string to the one stored in the settings</remarks>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {{
            optionsBuilder.UseSqlServer({databaseConnection});

            base.OnConfiguring(optionsBuilder);
        }}

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {{");

            // For entity framework versions that don't support the primary key attribute, write out compound
            // key info here.
            if(!ef7orLater)
            {
                foreach(var t in tableTypes.Where(t => t.Properties.Count(p => p.IsPrimaryKey) > 1))
                {
                    sw.WriteLine("            modelBuilder.Entity<{0}>().HasKey(t => new {{ {1} }});",
                        t.TypeName, String.Join(", ", t.Properties.Where(p => p.IsPrimaryKey).Select(
                            p => $"t.{p.PropertyName}")));
                }

                sw.WriteLine();
            }

            sw.Write(@"            /* TODO: If you pluralize or otherwise change the entity property names above but want to
               keep the table names as they are in the database, uncomment this.
            // Make the table names match the entity types not the DbSet<T> property names
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if(entityType.ClrType != null && entityType.GetTableName() != entityType.ClrType.Name)
                    entityType.SetTableName(entityType.ClrType.Name);
            }*/

            // TODO: If necessary define entity relationships here

            base.OnModelCreating(modelBuilder);
        }
        #endregion

        #region Stored procedure methods
        //=====================================================================

");
            // Add methods for each function
            foreach(var f in functions)
            {
                // Add To Do notes for a couple of common cases that can be handled without the method
                if(f.Parameters.Count == 0)
                {
                    sw.WriteLine("        // TODO: This method has no parameters.  You may be able to remove it");
                    sw.WriteLine("        // and add a LoadAllStoredProcedureAttribute to the result set type");
                    sw.WriteLine("        // and use the LoadAll<TEntity>() extension method instead.");
                }
                else
                {
                    if(f.ResultSetType != null)
                    {
                        var keys = new HashSet<string>(f.ResultSetType.Properties.Where(
                            p => p.IsPrimaryKey).Select(p => p.PropertyName), StringComparer.OrdinalIgnoreCase);

                        if(keys.Count != 0)
                        {
                            var parameters = new HashSet<string>(f.Parameters.Select(p => p.ParameterName),
                                StringComparer.OrdinalIgnoreCase);

                            if(keys.Count == parameters.Count && !keys.Except(parameters, StringComparer.OrdinalIgnoreCase).Any())
                            {
                                sw.WriteLine("        // TODO: This method's parameters match the key on the result set type.  You");
                                sw.WriteLine("        // may be able to remove it and add a LoadByKeyStoredProcedureAttribute to the ");
                                sw.WriteLine("        // result set type and use the LoadByKey<TEntity>() extension method instead.");
                            }
                        }
                    }
                    else
                    {
                        if(f.IsInsertMethod)
                        {
                            sw.WriteLine("        // TODO: This method is used to insert entities.  You may be able to ");
                            sw.WriteLine("        // remove it and add an InsertEntityStoredProcedureAttribute to the ");
                            sw.WriteLine("        // entity type and use the InsertEntity<TEntity>() or ");
                            sw.WriteLine("        // SubmitChanges<TEntity>() extension method instead.");
                        }

                        if(f.IsUpdateMethod)
                        {
                            sw.WriteLine("        // TODO: This method is used to update entities.  You may be able to ");
                            sw.WriteLine("        // remove it and add an UpdateEntityStoredProcedureAttribute to the ");
                            sw.WriteLine("        // entity type and use the UpdateEntity<TEntity>() or ");
                            sw.WriteLine("        // SubmitChanges<TEntity>() extension method instead.");
                        }

                        if(f.IsDeleteMethod)
                        {
                            sw.WriteLine("        // TODO: This method is used to delete entities.  You may be able to ");
                            sw.WriteLine("        // remove it and add a DeleteEntityStoredProcedureAttribute to the ");
                            sw.WriteLine("        // entity type and use the DeleteEntity<TEntity>() or ");
                            sw.WriteLine("        // SubmitChanges<TEntity>() extension method instead.");
                        }
                    }
                }

                if(!String.IsNullOrWhiteSpace(f.StoredProcedureName) && f.MethodName != f.StoredProcedureName)
                    sw.WriteLine($@"        [MethodStoredProcedure(""{f.StoredProcedureName}"")]");

                if(f.ResultSetType != null)
                    sw.Write($"        public IEnumerable<{f.ResultSetType.TypeName}> {f.MethodName}(");
                else
                {
                    if(f.HasReturnValue)
                        sw.Write($"        public int {f.MethodName}(");
                    else
                        sw.Write($"        public void {f.MethodName}(");
                }

                if(f.Parameters.Count == 0)
                    sw.WriteLine(")");
                else
                {
                    bool isFirst = true;

                    foreach(var p in f.Parameters)
                    {
                        if(!isFirst)
                            sw.Write(", ");

                        if(p.IsOutput)
                            sw.Write("ref ");

                        sw.Write($"{p.ParameterType}{(p.IsNullable ? "?" : String.Empty)} {p.ParameterName}");
                        isFirst = false;
                    }

                    sw.WriteLine(")");
                }

                sw.WriteLine("        {");

                string returnValue = "return";
                bool hasOutputValues = false;

                if(f.Parameters.Any(p => p.IsOutput))
                {
                    returnValue = "var result =";
                    hasOutputValues = true;
                }

                if(f.ResultSetType != null)
                    sw.Write($"            return this.ExecuteMethodQuery<{f.ResultSetType.TypeName}>((MethodInfo)MethodInfo.GetCurrentMethod(){nullForgiving}");
                else
                {
                    if(f.HasReturnValue || hasOutputValues)
                        sw.Write($"            {returnValue} this.ExecuteMethodNonQuery((MethodInfo)MethodInfo.GetCurrentMethod(){nullForgiving}");
                    else
                        sw.Write($"            this.ExecuteMethodNonQuery((MethodInfo)MethodInfo.GetCurrentMethod(){nullForgiving}");
                }

                if(f.Parameters.Count == 0)
                    sw.Write(")");
                else
                {
                    foreach(var p in f.Parameters)
                        sw.Write($", {p.ParameterName}");

                    sw.Write(")");
                }

                if(f.HasReturnValue && !hasOutputValues)
                    sw.WriteLine(".ReturnValue;");
                else
                    sw.WriteLine(";");

                if(hasOutputValues)
                {
                    sw.WriteLine();

                    foreach(var p in f.Parameters.Where(p => p.IsOutput))
                    {
                        sw.WriteLine($"            {p.ParameterName} = ({p.ParameterType}{(p.IsNullable ? "?" : String.Empty)})" +
                            $"result.OutputValues[nameof({p.ParameterName})];");
                    }

                    sw.WriteLine();

                    if(f.HasReturnValue)
                        sw.WriteLine("            return result.ReturnValue;");
                }

                sw.WriteLine("        }\r\n");
            }

            sw.WriteLine(@"        #endregion
    }
}");
        }
        #endregion
    }
}
