// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LoggingDigest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Process
{
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using System;
    using System.Linq;

    /// <summary>
    /// Logging Digest
    /// </summary>
    public abstract class LoggingDigest : ScheduledManager
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        protected readonly LogCore logCore = new LogCore();

        /// <summary>
        /// Application Core
        /// </summary>
        protected readonly ApplicationCore appCore = new ApplicationCore();
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LoggingDigest()
            : base(10, (int)TimeSpan.FromHours(12).TotalSeconds)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute
        /// </summary>
        public override void Execute()
        {
            using (new PerformanceMonitor())
            {
                logCore.Log("Processing begining.");

                try
                {
                    foreach (var application in from data in appCore.Applications()
                                                where null != data
                                                && Guid.Empty != data.Identifier
                                                select data.Identifier)
                    {
                        this.Digest(application);
                    }
                }
                catch (Exception ex)
                {
                    logCore.Log(ex, EventTypes.Critical, 99999);
                }

                logCore.Log("Processing completed.");
            }
        }

        /// <summary>
        /// Digest Information for an application
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        protected abstract void Digest(Guid applicationIdentifier);
        #endregion
    }
}