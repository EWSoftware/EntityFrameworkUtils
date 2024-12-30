//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : StoredProcedureMethodInfo.cs
// Author  : Eric Woodruff
// Updated : 11/28/2024
//
// This file contains a class used to contain stored procedure function method information from a DBML file
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/27/2024  EFW  Created the code
//===============================================================================================================

namespace DbmlToEntityFrameworkConverter
{
    /// <summary>
    /// This is used to contain information used to generate a data context stored procedure call method
    /// base on function information from a DBML file.
    /// </summary>
    internal sealed class StoredProcedureMethodInfo
    {
        /// <summary>
        /// The function ID used to map the stored procedure method to a type's insert, update, or delete
        /// operation.
        /// </summary>
        public string? FunctionId { get; set; }

        /// <summary>
        /// The method name
        /// </summary>
        public string MethodName { get; set; } = String.Empty;

        /// <summary>
        /// The stored procedure name if it differs from the method name
        /// </summary>
        public string? StoredProcedureName { get; set; }

        /// <summary>
        /// True if the method returns the stored procedure return value, false if not
        /// </summary>
        public bool HasReturnValue { get; set; }

        /// <summary>
        /// If not null, this is the result set type.  If null, the method returns the stored procedure's return value
        /// </summary>
        public TypeInfo? ResultSetType { get; set; }

        /// <summary>
        /// True if this is used as an entity's insert method, false if not
        /// </summary>
        public bool IsInsertMethod { get; set; }

        /// <summary>
        /// True if this is used as an entity's update method, false if not
        /// </summary>
        public bool IsUpdateMethod { get; set; }

        /// <summary>
        /// True if this is used as an entity's delete method, false if not
        /// </summary>
        public bool IsDeleteMethod { get; set; }

        /// <summary>
        /// Parameter information for the method, if any
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; } = [];
    }
}
