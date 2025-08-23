//===============================================================================================================
// System  : Entity Framework Test Application
// File    : Converters.cs
// Author  : Eric Woodruff
// Updated : 08/21/2025
//
// This file contains the value converter classes
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/21/2025  EFW  Created the code
//===============================================================================================================

using System.Globalization;

using Avalonia.Data.Converters;

namespace AvaloniaTestApp.ViewModels
{
    /// <summary>
    /// This converter is used to maintain null values for empty and whitespace strings
    /// </summary>
    public class StringToNullConverter : IValueConverter
    {
        /// <inheritdoc />
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Nothing to do here
            return value;
        }

        /// <inheritdoc />
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value is string s && String.IsNullOrWhiteSpace(s)) ? null : value;
        }
    }
}
