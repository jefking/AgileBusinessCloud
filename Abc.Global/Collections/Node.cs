﻿// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Node.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Collections
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Tree Node
    /// </summary>
    /// <typeparam name="T">Data type stored</typeparam>
    internal class Node<T>
    {
        #region Members
        /// <summary>
        /// Gets or sets the Left
        /// </summary>
        internal Node<T> Left;

        /// <summary>
        /// Gets or sets the Right
        /// </summary>
        internal Node<T> Right;

        /// <summary>
        /// Data
        /// </summary>
        private readonly T data;

        /// <summary>
        /// Key
        /// </summary>
        private readonly int key;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Node class.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="data">Data</param>
        internal Node(int key, T data)
        {
            Contract.Requires<ArgumentOutOfRangeException>(0 < key, "Invalid key, below zero.");

            this.key = key;
            this.data = data;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Key
        /// </summary>
        internal int Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets Data
        /// </summary>
        internal T Data
        {
            get
            {
                return this.data;
            }
        }
        #endregion
    }
}