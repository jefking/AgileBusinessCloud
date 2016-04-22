// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethods.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Logging
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Abc.Azure;
    using Abc.Logging.Datum;
    using Abc.Underpinning;

    /// <summary>
    /// Extension Methods
    /// </summary>
    internal static class ExtensionMethods
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();
        #endregion

        #region Abc.Underpinning.Application
        /// <summary>
        /// Get Token
        /// </summary>
        /// <param name="app">Application</param>
        /// <returns>Token</returns>
        public static Token GetToken(this Application app)
        {
            return new Token()
            {
                ApplicationId = app.Token.ApplicationId,
                ValidationKey = app.Token.ValidationKey
            };
        }
        #endregion

        #region Abc.Logging.Datum.Occurrence
        /// <summary>
        /// Load Occurrence
        /// </summary>
        /// <param name="occurence">Occurrence</param>
        /// <param name="duration">Duration</param>
        /// <param name="method">Method</param>
        /// <param name="className">Class Name</param>
        /// <param name="message">Message</param>
        /// <param name="session">Session</param>
        public static void Load(this Occurrence occurence, TimeSpan duration, string method, string className, string message, Guid session)
        {
            occurence.MachineName = Environment.MachineName;
            occurence.ThreadId = Thread.CurrentThread.ManagedThreadId;
            occurence.DeploymentId = Abc.Azure.AzureEnvironment.DeploymentId;
            occurence.OccurredOn = DateTime.UtcNow;
            occurence.Token = application.GetToken();
            occurence.Duration = duration;
            occurence.Method = method;
            occurence.Class = className;
            occurence.Message = string.IsNullOrWhiteSpace(message) ? null : message;
            occurence.SessionIdentifier = session;
        }
        #endregion

        #region Abc.Logging.EventTypes
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="type">Event Types</param>
        /// <returns>Client Event Types</returns>
        internal static Datum.EventTypes Convert(this EventTypes type)
        {
            switch (type)
            {
                case EventTypes.Critical:
                    return Datum.EventTypes.Critical;
                case EventTypes.Error:
                    return Datum.EventTypes.Error;
                case EventTypes.Warning:
                    return Datum.EventTypes.Warning;
                case EventTypes.Information:
                    return Datum.EventTypes.Information;
                case EventTypes.Verbose:
                    return Datum.EventTypes.Verbose;
                case EventTypes.Start:
                    return Datum.EventTypes.Start;
                case EventTypes.Stop:
                    return Datum.EventTypes.Stop;
                case EventTypes.Suspend:
                    return Datum.EventTypes.Suspend;
                case EventTypes.Resume:
                    return Datum.EventTypes.Resume;
                case EventTypes.Transfer:
                    return Datum.EventTypes.Transfer;
                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion

        #region System.Diagnostics.EventLogEntry
        /// <summary>
        /// Convert Event Log Entry to Event Log Item
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <returns>Datum Entry</returns>
        internal static Datum.EventLogItem Convert(this EventLogEntry entry)
        {
            return new EventLogItem()
            {
                DeploymentId = AzureEnvironment.DeploymentId,
                EntryType = entry.EntryType.Convert(),
                InstanceId = entry.InstanceId,
                MachineName = entry.MachineName,
                Message = entry.Message,
                OccurredOn = entry.TimeGenerated.ToUniversalTime(),
                Source = entry.Source,
                Token = application.GetToken(),
                User = entry.UserName,
            };
        }
        #endregion

        #region System.Diagnostics.EventLogEntryType
        /// <summary>
        /// Convert Event Log Entry Type from Windows to Web Service
        /// </summary>
        /// <param name="entryType">Entry Type</param>
        /// <returns>Datum Event Log Entry Type</returns>
        internal static Abc.Logging.Datum.EventLogEntryType Convert(this System.Diagnostics.EventLogEntryType entryType)
        {
            return (Abc.Logging.Datum.EventLogEntryType)entryType;
        }
        #endregion
    }
}