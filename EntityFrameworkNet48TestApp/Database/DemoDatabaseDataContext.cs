// Code generated by DBML To Entity Framework Converter tool
// https://github.com/EWSoftware/EntityFrameworkUtils

using System;
using System.Collections.Generic;
using System.Reflection;

using EWSoftware.EntityFramework;
using EWSoftware.EntityFramework.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkNet48TestApp.Database
{
    [ParameterNamePrefix("param")]
    public sealed class DemoDatabaseDataContext : DbContext
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This is used to set the database location for the demo
        /// </summary>
        public static string DatabaseLocation { get; set; }

        #endregion

        #region Tracked entities
        //=====================================================================

        // These entities utilize change tracking so a property is required here for them
        public DbSet<StateCode> StateCodes { get; set; }

        public DbSet<DemoTable> DemoTable { get; set; }

        public DbSet<ProductInfo> ProductInfo { get; set; }

        #endregion

        #region Method overrides
        //=====================================================================

        /// <inheritdoc />
        /// <remarks>This is overridden to set the connection string to the one stored in the settings</remarks>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(String.IsNullOrWhiteSpace(DatabaseLocation))
                throw new InvalidOperationException("The database location has not been set");

            optionsBuilder.UseSqlServer($@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog={DatabaseLocation};Integrated Security=True");

            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make the table names match the entity types not the DbSet<T> property names
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if(entityType.ClrType != null && entityType.GetTableName() != entityType.ClrType.Name)
                    entityType.SetTableName(entityType.ClrType.Name);
            }

            base.OnModelCreating(modelBuilder);
        }
        #endregion

        #region Stored procedure methods
        //=====================================================================

        public int spStateCodeAddUpdate(string oldState, string state, string stateDesc)
        {
            return this.ExecuteMethodNonQuery((MethodInfo)MethodInfo.GetCurrentMethod(), oldState, state, stateDesc).ReturnValue;
        }

        public int spStateCodeDelete(string state)
        {
            return this.ExecuteMethodNonQuery((MethodInfo)MethodInfo.GetCurrentMethod(), state).ReturnValue;
        }

        public IEnumerable<spProductSearchResult> spProductSearch(string productName, string categoryName, string companyName)
        {
            return this.ExecuteMethodQuery<spProductSearchResult>((MethodInfo)MethodInfo.GetCurrentMethod(), productName, categoryName, companyName);
        }
        #endregion
    }
}