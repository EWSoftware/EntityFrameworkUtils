//===============================================================================================================
// System  : LINQ to SQL Test App
// File    : StateCode.cs
// Author  : Eric Woodruff
// Updated : 12/16/2024
//
// This file contains a class used to contain partial methods for the state code entity
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/16/2024  EFW  Created the code
//===============================================================================================================

using System;
using System.Data.Linq;

namespace LinqToSQLTestApp.Database
{
    public partial class StateCode
    {

        /// <summary>
        /// Validate the row prior to submitting the changes
        /// </summary>
        /// <param name="value">The new value to validate</param>
        partial void OnValidate(ChangeAction action)
        {
            // On insert, assign a unique time stamp as it is our "primary key".  This works around LINQ to SQL's
            // unwillingness to let us modify the actual primary key (State).
            if(action == ChangeAction.Insert)
                this.LastModified = new Binary(BitConverter.GetBytes((long)Guid.NewGuid().GetHashCode()));
        }
    }
}
