// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Rule.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Validation
{
    using System;

    /// <summary>
    /// Rule
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class Rule<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Rule class
        /// </summary>
        /// <param name="test">Test</param>
        /// <param name="message">Message</param>
        public Rule(Func<T, bool> test, string message)
        {
            if (null == test)
            {
                throw new ArgumentNullException("test");
            }
            else if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentOutOfRangeException("message");
            }
            else
            {
                this.Test = test;
                this.Message = message;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Test
        /// </summary>
        public Func<T, bool> Test
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets Message
        /// </summary>
        public string Message
        {
            get;
            private set;
        }
        #endregion
    }
}