// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EventArgs.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System;

    /// <summary>
    /// Event Arguments
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public class EventArgs<T> : EventArgs
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the EventArgs class
        /// </summary>
        /// <param name="argument">Argument</param>
        public EventArgs(T argument)
        {
            this.Argument = argument;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Argument
        /// </summary>
        public T Argument
        {
            get;
            private set;
        }
        #endregion
    }
}