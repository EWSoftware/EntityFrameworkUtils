//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : ChangeTrackingEntity.cs
// Author  : Eric Woodruff
// Updated : 08/15/2025
//
// This file contains the class used as a base class for entity types that require change tracking notifications
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/22/2024  EFW  Created the code
//===============================================================================================================

using System.Runtime.CompilerServices;

namespace EWSoftware.EntityFramework
{
    /// <summary>
    /// This serves as an abstract base class for entity types that require change tracking notifications
    /// </summary>
    /// <remarks>Properties in derived classes can call <see cref="SetWithNotify"/> to raise the
    /// <see cref="PropertyChanging"/> event before and <see cref="PropertyChanged"/> event after changing the
    /// property value.</remarks>
    public abstract class ChangeTrackingEntity : INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// This is used to raise the <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events
        /// if the new value does not equal the current property's value
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The new value</param>
        /// <param name="field">A reference to the field containing the current value that will receive the
        /// new value</param>
        /// <param name="propertyName">The property name that changed.  This defaults to the calling member's
        /// name if not specified</param>
        protected void SetWithNotify<T>(T value, ref T field, [CallerMemberName] string propertyName = "")
        {
            if(!EqualityComparer<T>.Default.Equals(field, value))
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
