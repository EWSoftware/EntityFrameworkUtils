//===============================================================================================================
// System  : LINQ to SQL Test App
// File    : Utility.cs
// Author  : Eric Woodruff
// Updated : 12/17/2024
//
// This file contains some utility methods for the demo
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/17/2024  EFW  Created the code
//===============================================================================================================

using System;
using System.Data.Linq;

namespace LinqToSQLTestApp
{
    /// <summary>
    /// This contains some utility and extension methods for the demo
    /// </summary>
    internal static class Utility
    {
        #region Context extension methods
        //=====================================================================

        /// <summary>
        /// This extension method is used to see if a data context has any unsaved changes.
        /// </summary>
        /// <param name="dc">The data context to check</param>
        /// <returns>True if the data context's change set has any inserted, updated, or deleted items waiting to
        /// be submitted, False if not.</returns>
        ///
        /// <example>
        /// <code language="cs">
        /// if(dc.HasChanges())
        ///     dc.SubmitChanges();
        /// </code>
        /// </example>
        public static bool HasChanges(this DataContext dc)
        {
            if(dc == null)
                throw new ArgumentNullException(nameof(dc));

            ChangeSet changes = dc.GetChangeSet();

            return changes.Inserts.Count != 0 || changes.Updates.Count != 0 || changes.Deletes.Count != 0;
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
        public static string NullIfEmpty(this string value)
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
        public static string NullIfWhiteSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>
        /// This is used to convert an object to a string and return either the string value if not empty or null
        /// if it is an empty string.
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
        /// result = dc.spDrasticActions(dtpFromDate.Value, dtpToDate.Value,
        ///     cboActionType.SelectedValue.ToStringOrNull());
        /// </code>
        /// </example>
        public static string ToStringOrNull(this object value)
        {
            if(value == null || value == DBNull.Value)
                return null;

            string s = value.ToString();

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
        /// dataContext.spAddName(txtLastName.Text, txtFirstName.Text,
        ///     txtRace.Text.ToChar(), txtSex.Text.ToChar());
        /// </code>
        /// </example>
        public static char? ToChar(this string value)
        {
            return !String.IsNullOrEmpty(value) ? value[0] : (char?)null;
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
        /// dataContext.spUpdateStartDate(caseKey,
        ///     dtpStartDate.BindableValue.ToNullable&lt;DateTime&gt;());
        /// </code>
        /// </example>
        /// <seealso cref="ToNullable{T}(T)" />
        public static T? ToNullable<T>(this object value) where T : struct
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
