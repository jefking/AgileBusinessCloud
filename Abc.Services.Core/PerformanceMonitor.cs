// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PerformanceMonitor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Threading;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// Performance Monitor
    /// </summary>
    public sealed class PerformanceMonitor : Abc.Diagnostics.PerformanceMonitor
    {
        #region Members
        /// <summary>
        /// Logging Core
        /// </summary>
        private static readonly LogCore log = new LogCore();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PerformanceMonitor class
        /// </summary>
        public PerformanceMonitor()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether To Log
        /// </summary>
        protected override bool ToLog
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Log Occurence
        /// </summary>
        /// <param name="duration">Duration</param>
        protected override void LogOccurrence(TimeSpan duration)
        {
            var token = new Token()
            {
                ApplicationId = ServerConfiguration.ApplicationIdentifier
            };
            if (Guid.Empty != token.ApplicationId)
            {
                var occurance = new Occurrence()
                {
                    OccurredOn = DateTime.UtcNow,
                    Duration = duration,
                    MachineName = Environment.MachineName,
                    ThreadId = Thread.CurrentThread.ManagedThreadId,
                    Message = Content,
                    Token = token,
                    Method = MethodName,
                    Class = ClassName,
                    SessionIdentifier = this.SessionIdentifier,
                };

                log.Log(occurance);
            }
        }
        #endregion
    }
}