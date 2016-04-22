// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogHistory.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// Log Item History
    /// </summary>
    [AzureDataStore("logging")]
    [Serializable]
    public class LogHistory<T>
        where T : LogItem
    {
        #region Properties
        /// <summary>
        ///  Gets and sets Generated On
        /// </summary>
        public DateTime? GeneratedOn
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets and sets Items Date
        /// </summary>
        public DateTime? MaximumDate
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets and sets Items Date
        /// </summary>
        public DateTime? MinimumDate
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets and sets Items Count
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets Items
        /// </summary>
        public T[] Items
        {
            get;
            set;
        }
        #endregion
    }
}