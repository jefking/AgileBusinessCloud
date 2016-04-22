// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationScheduleManager.cs'>
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
    /// Application Schedule Manager
    /// </summary>
    public abstract class ApplicationScheduleManager : ScheduledManager
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
        protected ApplicationScheduleManager(int minutes)
            : base(30, (int)TimeSpan.FromMinutes(minutes).TotalSeconds)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            using (new PerformanceMonitor())
            {
                logCore.Log("Compressed Errors processing begining.");

                try
                {
                    foreach (var application in (from data in appCore.Applications()
                                                 where null != data
                                                     && Guid.Empty != data.Identifier
                                                 select data.Identifier).Distinct())
                    {
                        this.Execute(application);
                    }
                }
                catch (Exception ex)
                {
                    logCore.Log(ex, EventTypes.Critical, 99999);
                }

                logCore.Log("Compressed Errors processing completed.");
            }
        }

        public abstract void Execute(Guid application);
        #endregion
    }
}