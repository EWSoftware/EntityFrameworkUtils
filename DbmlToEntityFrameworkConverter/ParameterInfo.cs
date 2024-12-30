//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : ParameterInfo.cs
// Author  : Eric Woodruff
// Updated : 11/28/2024
//
// This file contains a class used to contain function parameter information from a DBML file
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/27/2024  EFW  Created the code
//===============================================================================================================

namespace DbmlToEntityFrameworkConverter
{
    /// <summary>
    /// This class is used to contain function parameter information from a DBML file
    /// </summary>
    internal sealed class ParameterInfo
    {
        /// <summary>
        /// The parameter name
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// The parameter type
        /// </summary>
        public string ParameterType { get; set; }

        /// <summary>
        /// True if the parameter is nullable, false if not
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// True if the parameter is an output (by reference) parameter
        /// </summary>
        public bool IsOutput { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterInfo">The element from which to get the parameter information</param>
        /// <param name="parameterPrefix">An optional parameter prefix to remove from the parameter name</param>
        public ParameterInfo(XElement parameterInfo, string? parameterPrefix)
        {
            this.ParameterName = parameterInfo.Attribute("Name")!.Value;
            this.ParameterType = PropertyInfo.PropertyTypes[parameterInfo.Attribute("Type")!.Value];
            this.IsNullable = (bool?)parameterInfo.Attribute("CanBeNull") ?? true;
            this.IsOutput = parameterInfo.Attribute("Direction")?.Value == "InOut";

            // Camel casing can be a bit tricky so we'll just give it our best shot
            if(!String.IsNullOrWhiteSpace(parameterPrefix) && this.ParameterName.StartsWith(parameterPrefix, StringComparison.Ordinal))
            {
                int i = parameterPrefix.Length;

                while(i < this.ParameterName.Length && Char.IsUpper(this.ParameterName[i]))
                    i++;

                if(i > parameterPrefix.Length + 1)
                    i--;

                this.ParameterName = this.ParameterName[(parameterPrefix.Length)..i].ToLowerInvariant() + this.ParameterName[i..];

                if(Char.IsUpper(this.ParameterName[^1]) && !Char.IsUpper(this.ParameterName[^2]))
                    this.ParameterName = this.ParameterName.ToLowerInvariant();
            }
        }
    }
}
