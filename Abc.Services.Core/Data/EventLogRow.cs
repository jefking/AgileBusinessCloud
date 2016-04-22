// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EventLogRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;

    /// <summary>
    /// Event Log Row
    /// </summary>
    [Serializable]
    [AzureDataStore("EventLog")]
    [CLSCompliant(false)]
    public class EventLogRow : LogData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the EventLogRow class
        /// </summary>
        public EventLogRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EventLogRow class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public EventLogRow(Guid applicationId)
            : base(applicationId)
        {
            Contract.Requires(Guid.Empty != applicationId);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Instance Id
        /// </summary>
        public long? InstanceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Source
        /// </summary>
        public string Source
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets User
        /// </summary>
        public string User
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Event Id
        /// </summary>
        public int? EventId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Event Log Entry Type Value
        /// </summary>
        public int EntryTypeValue
        {
            get;
            set;
        }
        #endregion
    }
}