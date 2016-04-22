// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceMonitor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Logging
{
    using System;
    using System.Diagnostics;
    using Abc.Logging.Datum;
    using Abc.Underpinning;

    /// <summary>
    /// Performance Monitor, to log performance data to the ABC Datum web service
    /// </summary>
    public sealed class PerformanceMonitor : Abc.Diagnostics.PerformanceMonitor
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();
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
        /// Gets the Minimum Duration
        /// </summary>
        public override TimeSpan MinimumDuration
        {
            get
            {
                return ConfigurationSettings.MinimumDuration;
            }
        }

        /// <summary>
        /// Gets a value indicating whether To Log
        /// </summary>
        protected override bool ToLog
        {
            get
            {
                return ConfigurationSettings.LogPerformance;
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
            if (null != application.Token)
            {
                var occurrence = new Occurrence();
                occurrence.Load(duration, MethodName, ClassName, Content, base.SessionIdentifier);

                MessageHandler.Instance.Queue(occurrence);
            }
            else
            {
                Trace.Write("Application is not validated.");
            }
        }
        #endregion
    }
}