// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LoggingCleaner.cs'>
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
    /// Logging Cleaner
    /// </summary>
    public class LoggingCleaner : ApplicationScheduleManager
    {
        #region Members
        /// <summary>
        /// Expiration of Logged Items
        /// </summary>
        public static readonly TimeSpan Expiration = new TimeSpan(22, 0, 0, 0);
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LoggingCleaner()
            : base(24 * 60)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Messages
        /// </summary>
        public void Messages(Token token)
        {
            using (new PerformanceMonitor())
            {
                logCore.Log("Message processing begining.");

                try
                {
                    var item = new Message()
                    {
                        OccurredOn = DateTime.UtcNow.Subtract(Expiration),
                        Token = token,
                    };

                    logCore.Delete(item);
                }
                catch (Exception ex)
                {
                    logCore.Log(ex, EventTypes.Critical, 99999);
                }

                logCore.Log("Message processing completed.");
            }
        }

        /// <summary>
        /// Performance
        /// </summary>
        public void Performance(Token token)
        {
            using (new PerformanceMonitor())
            {
                logCore.Log("Performance processing begining.");

                try
                {
                    var item = new Occurrence()
                    {
                        OccurredOn = DateTime.UtcNow.Subtract(Expiration),
                        Token = token,
                    };

                    logCore.Delete(item);
                }
                catch (Exception ex)
                {
                    logCore.Log(ex, EventTypes.Critical, 99999);
                }

                logCore.Log("Performance processing completed.");
            }
        }

        /// <summary>
        /// Errors
        /// </summary>
        public void Errors(Token token)
        {
            using (new PerformanceMonitor())
            {
                logCore.Log("Error processing begining.");

                try
                {
                    var item = new ErrorItem()
                    {
                        OccurredOn = DateTime.UtcNow.Subtract(Expiration),
                        Token = token,
                    };

                    logCore.Delete(item);
                }
                catch (Exception ex)
                {
                    logCore.Log(ex, EventTypes.Critical, 99999);
                }

                logCore.Log("Error processing completed.");
            }
        }

        /// <summary>
        /// Execute Digest Tasks
        /// </summary>
        public override void Execute(Guid application)
        {
            using (new PerformanceMonitor())
            {
                var token = new Token()
                {
                    ApplicationId = application
                };

                this.Messages(token);
                this.Performance(token);
                this.Errors(token);
            }
        }
        #endregion
    }
}