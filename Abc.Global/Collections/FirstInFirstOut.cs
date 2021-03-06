﻿// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='FirstInFirstOut.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// Safety First In First Out Queue
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class FirstInFirstOut<T>
    {
        #region Members
        /// <summary>
        /// Queue
        /// </summary>
        private readonly Queue<T> queue = null;

        /// <summary>
        /// Lock
        /// </summary>
        private readonly object safetyLock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the FirstInFirstOut class
        /// </summary>
        public FirstInFirstOut()
        {
            this.queue = new Queue<T>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets current Count
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.safetyLock)
                {
                    return this.queue.Count;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Dequeue
        /// </summary>
        /// <returns>Item</returns>
        public T Dequeue()
        {
            lock (this.safetyLock)
            {
                return this.queue.Count > 0 ? this.queue.Dequeue() : default(T);
            }
        }

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="item">Item</param>
        public void Enqueue(T item)
        {
            lock (this.safetyLock)
            {
                this.queue.Enqueue(item);
            }
        }
        #endregion
    }
}