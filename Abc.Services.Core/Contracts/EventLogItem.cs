// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EventLogItem.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Abc.Services.Data;
    using Abc.Services.Validation;

    /// <summary>
    /// Event Log Item
    /// </summary>
    [Serializable]
    [DataContract]
    public class EventLogItem : LogItem, IConvert<EventLogRow>, IValidate<EventLogItem>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Source
        /// </summary>
        [DataMember]
        public string Source
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Instance Id
        /// </summary>
        [DataMember]
        public long InstanceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets User
        /// </summary>
        [DataMember]
        public string User
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Event Id
        /// </summary>
        [DataMember]
        public int EventId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Event Log Entry Type Value
        /// </summary>
        [DataMember]
        public EventLogEntryType EntryType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Validation Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Email>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not a localization issue.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<EventLogItem>> Rules
        {
            get
            {
                return new Rule<EventLogItem>[]
                {
                    new Rule<EventLogItem>(i => DataSource.RowIsValid(i.Message), "Message isn't valid."),
                    new Rule<EventLogItem>(i => DataSource.RowIsValid(i.MachineName), "Machine name is too long."),
                    new Rule<EventLogItem>(i => !string.IsNullOrWhiteSpace(i.MachineName), "Machine name isn't specified."),
                    new Rule<EventLogItem>(i => DataSource.RowIsValid(i.DeploymentId), "Deployment is too long."),
                    new Rule<EventLogItem>(i => DataSource.RowIsValid(i.User), "User is too long."),
                    new Rule<EventLogItem>(i => i.EventId >= 0, "Event Id isn't valid."),
                    new Rule<EventLogItem>(i => i.InstanceId >= 0, "Instance Id isn't valid."),
                    new Rule<EventLogItem>(i => i.EntryType != EventLogEntryType.Unknown && Enum.IsDefined(typeof(EventLogEntryType), i.EntryType), "Event Type isn't valid.")
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Event Log Row</returns>
        [CLSCompliant(false)]
        public EventLogRow Convert()
        {
            return new EventLogRow(this.Token.ApplicationId)
            {
                DeploymentId = this.DeploymentId,
                EventId = this.EventId,
                EntryTypeValue = (int)this.EntryType,
                MachineName = this.MachineName,
                Message = this.Message,
                OccurredOn = this.OccurredOn,
                Source = this.Source,
                User = this.User,
                InstanceId = this.InstanceId,
            };
        }
        #endregion
    }
}