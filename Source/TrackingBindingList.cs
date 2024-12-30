//===============================================================================================================
// System  : EWSoftware Entity Framework Utilities
// File    : TrackingBindingList.cs
// Author  : Eric Woodruff
// Updated : 11/26/2024
//
// This file contains a binding list class used to contain trackable entities
//
//    Date     Who  Comments
// ==============================================================================================================
// 11/23/2024  EFW  Created the code
//===============================================================================================================

namespace EWSoftware.EntityFramework
{
    /// <summary>
    /// This is used to contain a set of entities that have tracking enabled
    /// </summary>
    /// <typeparam name="TEntity">The entity type in the collection</typeparam>
    public class TrackingBindingList<TEntity> : BindingList<TEntity> where TEntity : class
    {
        #region Private data members
        //=====================================================================

        private readonly DbContext dataContext;
        private TEntity? newInstance, cancelNewInstance;
        private bool addingNewInstance;

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext">The related data context that is tracking the entities</param>
        /// <param name="entities">A list of the entities being tracked</param>
        internal TrackingBindingList(DbContext dataContext, IList<TEntity> entities) : base(entities)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <inheritdoc />
        protected override object AddNewCore()
        {
            addingNewInstance = true;
            newInstance = (TEntity)base.AddNewCore()!;

            return newInstance;
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, TEntity item)
        {
            base.InsertItem(index, item);

            if(!addingNewInstance && index >= 0 && index <= this.Count)
                dataContext.Add(item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            if(index >= 0 && index < Count && this[index] == cancelNewInstance)
                cancelNewInstance = null;
            else
                dataContext.Remove(this[index]);

            base.RemoveItem(index);
        }

        /// <inheritdoc />
        protected override void SetItem(int index, TEntity item)
        {
            TEntity removedItem = this[index];

            base.SetItem(index, item);

            if(index >= 0 && index < this.Count)
            {
                // If the user is trying to set an item that is currently being added by AddNew
                // cancel the AddNew and add the item that was passed in.
                if(removedItem == newInstance)
                {
                    newInstance = null;
                    addingNewInstance = false;
                }
                else
                    dataContext.Remove(removedItem);

                dataContext.Add(item);
            }
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            foreach(var item in this)
                dataContext.Remove(item);

            base.ClearItems();
        }

        /// <inheritdoc />
        public override void EndNew(int itemIndex)
        {
            if(itemIndex >= 0 && itemIndex < this.Count && this[itemIndex] == newInstance)
            {
                dataContext.Add(newInstance);

                newInstance = null;
                addingNewInstance = false;
            }

            base.EndNew(itemIndex);
        }

        /// <inheritdoc />
        public override void CancelNew(int itemIndex)
        {
            if(itemIndex >= 0 && itemIndex < this.Count && this[itemIndex] == newInstance)
            {
                cancelNewInstance = newInstance;
                newInstance = null;
                addingNewInstance = false;
            }

            base.CancelNew(itemIndex);
        }
        #endregion
    }
}
