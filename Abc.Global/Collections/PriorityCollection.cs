﻿// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PriorityCollection.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Priority Collection
    /// </summary>
    /// <typeparam name="TStored">Type Stored</typeparam>
    public class PriorityCollection<TStored> : IEnumerable<TStored>,
        ICollection<TStored>,
        IEnumerable
    {
        #region Members
        /// <summary>
        /// Internal Data Type
        /// </summary>
        private readonly List<TStored> data;

        /// <summary>
        /// Locking, for synchronization
        /// </summary>
        private readonly object locker = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PriorityCollection class.
        /// </summary>
        public PriorityCollection()
            : this(new List<TStored>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityCollection class.
        /// </summary>
        /// <param name="count">Count</param>
        public PriorityCollection(int count)
            : this(new List<TStored>(count))
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityCollection class.
        /// </summary>
        /// <param name="list">Original List</param>
        private PriorityCollection(List<TStored> list)
            : base()
        {
            this.data = list;
            if (null != this.data
                && 0 < this.data.Count)
            {
                this.data.Sort();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Peek at Min Item
        /// </summary>
        public TStored Min
        {
            get
            {
                lock (this.locker)
                {
                    return (0 < this.data.Count) ? this.data[0] : default(TStored);
                }
            }
        }

        /// <summary>
        /// Gets Peek at Max Item
        /// </summary>
        public TStored Max
        {
            get
            {
                lock (this.locker)
                {
                    return (0 < this.data.Count) ? this.data[this.data.Count - 1] : default(TStored);
                }
            }
        }

        /// <summary>
        /// Gets the Count
        /// </summary>
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == this.data.Count);

                lock (this.locker)
                {
                    return this.data.Count;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read only
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Push
        /// </summary>
        /// <remarks>
        /// Sort overhead is implemented on Add
        /// </remarks>
        /// <param name="item">Item</param>
        public void Push(TStored item)
        {
            lock (this.locker)
            {
                this.data.Add(item);
                if (1 < this.data.Count)
                {
                    this.data.Sort();
                }
            }
        }

        /// <summary>
        /// Pop
        /// </summary>
        /// <returns>Item</returns>
        public TStored Pop()
        {
            lock (this.locker)
            {
                if (0 < this.data.Count)
                {
                    TStored item = this.data[this.data.Count - 1];
                    this.data.RemoveAt(this.data.Count - 1);
                    return item;
                }
                else
                {
                    return default(TStored);
                }
            }
        }

        #region IEnumerable<Type> Members
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<TStored> GetEnumerator()
        {
            lock (this.locker)
            {
                return this.data.GetEnumerator();
            }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.locker)
            {
                return this.data.GetEnumerator();
            }
        }
        #endregion

        #region ICollection<Type> Members
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="item">Item</param>
        public void Add(TStored item)
        {
            Contract.Ensures(this.Count >= Contract.OldValue(this.Count));

            this.Push(item);
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            Contract.Ensures(this.Count == 0);

            lock (this.locker)
            {
                if (this.data.Count > 0)
                {
                    this.data.Clear();
                }
            }
        }

        /// <summary>
        /// Contains Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Contains</returns>
        public bool Contains(TStored item)
        {
            lock (this.locker)
            {
                bool result = this.data.Count > 0 && this.data.Contains(item);

                Contract.Assume(!result || this.Count > 0);

                return result;
            }
        }

        /// <summary>
        /// Copy To Array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="arrayIndex">Array Index</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Unable to as it implements interface.")]
        public void CopyTo(TStored[] array, int arrayIndex)
        {
            Contract.Assume(null != array);
            Contract.Assume(arrayIndex + this.Count <= array.Length);
            Contract.Assume(arrayIndex < array.Length);

            lock (this.locker)
            {
                this.data.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Remove Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Removed</returns>
        public bool Remove(TStored item)
        {
            lock (this.locker)
            {
                return this.data.Remove(item);
            }
        }
        #endregion
        #endregion
    }
}