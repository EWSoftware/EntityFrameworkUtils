//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : TrackingObservableCollection.cs
// Author  : Eric Woodruff
// Updated : 11/26/2024
//
// This file contains an observable collection class used to contain trackable entities
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/23/2024  EFW  Created the code
//===============================================================================================================

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EWSoftware.EntityFramework
{
    /// <summary>
    /// This is used to contain a set of entities that have tracking enabled
    /// </summary>
    /// <typeparam name="T">The entity type in the collection</typeparam>
    /// <remarks>
    /// <note type="important">This collection class will not work if bound to Windows Forms controls.  Use
    /// <see cref="TrackingBindingList{TEntity}"/> instead.</note>
    /// </remarks>
    public class TrackingObservableCollection<T> : ObservableCollection<T>
    {
        #region Private data members
        //=====================================================================

        private readonly DbContext dataContext;

        #endregion

        #region Internal constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext">The related data context that is tracking the entities</param>
        /// <param name="entities">An enumerable list of the entities being tracked</param>
        internal TrackingObservableCollection(DbContext dataContext, IEnumerable<T> entities) : base(entities)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <summary>
        /// This is overridden to remove each individual entry rather than clearing them all out at once to
        /// allow the data context to know about the removal of all the tracked items.
        /// </summary>
        protected override void ClearItems()
        {
            while(this.Count != 0)
                this.RemoveItem(0);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// This is overridden to add or remove items from the related data context
        /// </summary>
        /// <inheritdoc />
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if(e != null)
            {
                switch(e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if(e.NewItems != null)
                        {
                            foreach(T item in e.NewItems)
                                dataContext.Add(item);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if(e.OldItems != null)
                        {
                            foreach(T item in e.OldItems)
                                dataContext.Remove(item);
                        }
                        break;

                    default:
                        break;
                }

                base.OnCollectionChanged(e);
            }
        }
        #endregion
    }
}
