// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Throughput.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Throughput of Services
    /// </summary>
    [DataContract]
    [Serializable]
    public class Throughput
    {
        #region Properties
        /// <summary>
        /// Gets or sets Exceptions
        /// </summary>
        [DataMember]
        public int Exceptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Messages
        /// </summary>
        [DataMember]
        public int Messages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Performance
        /// </summary>
        [DataMember]
        public int Performance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Event Log
        /// </summary>
        [DataMember]
        public int EventLog
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Server Statistics Count
        /// </summary>
        [DataMember]
        public int ServerStatistics
        {
            get;
            set;
        }
        #endregion
    }
}