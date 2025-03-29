namespace LinqToSQLTestApp.Database
{
    using System;

    partial class DemoDatabaseDataContext
    {
        /// <summary>
        /// This is used to set the database location for the demo
        /// </summary>
        public static string DatabaseLocation { get; set; }

        partial void OnCreated()
        {
            if(String.IsNullOrWhiteSpace(DatabaseLocation))
                throw new InvalidOperationException("The database location has not been set");

            this.Connection.ConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFileName={DatabaseLocation}";
        }
    }
}