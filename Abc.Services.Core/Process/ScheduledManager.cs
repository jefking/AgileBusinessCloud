// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ScheduledManager.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Process
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using System;
    using System.Linq;

    /// <summary>
    /// Scheduled Manager
    /// </summary>
    public abstract class ScheduledManager : TaskManager
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        protected readonly LogCore log = new LogCore();

        /// <summary>
        /// Period of Timer
        /// </summary>
        private readonly TimeSpan period;

        /// <summary>
        /// Maximum Duration before Retry
        /// </summary>
        private readonly TimeSpan retryInterval = TimeSpan.FromMinutes(10);
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        protected ScheduledManager(int dueInSeconds, int periodInSeconds)
            : base(dueInSeconds, periodInSeconds)
        {
            this.period = TimeSpan.FromSeconds(periodInSeconds);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute
        /// </summary>
        protected override void Execute(object state)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var item = new DataManagerLog(this.GetType());

                    var table = new AzureTable<DataManagerLog>(ServerConfiguration.Default);
                    var latest = (from data in table.QueryByPartition(item.PartitionKey).ToList()
                                  orderby data.StartTime
                                  select data).FirstOrDefault();

                    // Check if there's any task to execute
                    var performTask = null == latest || (latest.CompletionTime.HasValue ?
                        DateTime.UtcNow.Subtract(latest.CompletionTime.Value) >= period || !latest.Successful :
                        DateTime.UtcNow.Subtract(latest.StartTime) >= retryInterval);

                    if (performTask)
                    {
                        log.Log(string.Format("{0} Task Started.", DateTime.UtcNow));

                        item.StartTime = DateTime.UtcNow;

                        table.AddEntity(item);

                        // Start the backup
                        try
                        {
                            this.Execute();
                            item.Successful = true;
                        }
                        catch (Exception ex)
                        {
                            log.Log(ex, EventTypes.Critical, (int)ServiceFault.Unknown);
                            item.Successful = false;
                        }
                        finally
                        {
                            item.CompletionTime = DateTime.UtcNow;
                        }

                        // Update entry in table
                        table.AddOrUpdateEntity(item);
                        log.Log(string.Format("{0} Task Completed. Success: {1}", DateTime.UtcNow, item.Successful));
                    }
                    else
                    {
                        log.Log(string.Format("{0} No Action Required.", DateTime.UtcNow));
                    }
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Error, (int)ServiceFault.Unknown);
                }
            }
        }
        #endregion
    }
}