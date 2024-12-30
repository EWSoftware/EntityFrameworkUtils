//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : NeverTrackAttribute.cs
// Author  : Eric Woodruff
// Updated : 11/24/2024
//
// This file contains an attribute used to mark an entity type as never tracked so that the stored procedure
// extension methods never add, update, or remove it from the database context's change tracker.
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/24/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework.DataAnnotations
{
    /// <summary>
    /// This attribute is used to mark an entity type as never tracked so that the stored procedure extension
    /// methods never add, update, or remove it from the database context's change tracker.
    /// </summary>
    /// <remarks>This is useful for entities that are not modified and/or do not implement property change
    /// notification and thus do not require change tracking.</remarks>
    /// <example>
    /// <code language="cs">
    /// // Used to display state codes in a drop-down list.  This is never modified and does
    /// // not need change tracking.
    /// [NeverTrack]
    /// public sealed class StateCode
    /// {
    ///     // The state code
    ///     public string StateCode { get; set; }
    ///     
    ///     // The state description
    ///     public string StateDesc { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NeverTrackAttribute : Attribute
    {
    }
}
