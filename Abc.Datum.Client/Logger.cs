// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Logger.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Logging
{
    using System;
    using System.Diagnostics;
    using Abc.Logging.Datum;
    using Abc.Underpinning;
    using Abc.Diagnostics;

    /// <summary>
    /// Logger, to log Exceptions to the ABC Datum web service
    /// </summary>
    public class Logger
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Logger class
        /// </summary>
        public Logger()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Log Event Log Entry
        /// </summary>
        /// <param name="entry">Entry</param>
        public static void Log(EventLogEntry entry)
        {
            if (null != entry)
            {
                var item = entry.Convert();
                MessageHandler.Instance.Queue(item);
            }
            else
            {
                Trace.WriteLine("Event log entry attempting to be stored was null.");
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="ex">Exception</param>
        public void Log(Exception ex)
        {
            this.Log(ex, EventTypes.Error, 0);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="type">Event Type</param>
        public void Log(Exception ex, EventTypes type)
        {
            this.Log(ex, type, 0);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="type">Event Type</param>
        /// <param name="errorCode">Error Code</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "API for developers.")]
        public void Log(Exception ex, EventTypes type, int errorCode)
        {
            if (ConfigurationSettings.LogExceptions)
            {
                if (null != ex)
                {
                    if (null != application.Token)
                    {
                        var token = application.GetToken();
                        var sessionIdentifier = Session.InstantSession();

                        ErrorItem root = null;
                        ErrorItem error = null;

                        while (ex != null)
                        {
                            if (null == root)
                            {
                                root = Convert(token, ex, type, errorCode, sessionIdentifier);
                                error = root;
                            }
                            else
                            {
                                error.Parent = Convert(token, ex, type, errorCode, sessionIdentifier);
                                error = error.Parent;
                            }

                            ex = ex.InnerException;
                        }

                        MessageHandler.Instance.Queue(root);
                    }
                }
                else
                {
                    // Do not log exception, recusive stack overflow could occur
                }
            }
            else
            {
                Trace.Write("Exception Logging is turned off.");
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="errorCode">Error Code</param>
        public void Log(Exception ex, int errorCode)
        {
            this.Log(ex, EventTypes.Error, errorCode);
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="ex">Exception</param>
        /// <param name="type">Type</param>
        /// <param name="errorCode">Error Code</param>
        /// <param name="sessionIdentifier">Session Identifier</param>
        /// <returns>Error</returns>
        private static ErrorItem Convert(Token token, Exception ex, EventTypes type, int errorCode, Guid sessionIdentifier)
        {
            return new ErrorItem()
            {
                ErrorCode = errorCode,
                EventType = type.Convert(),
                MachineName = Environment.MachineName,
                DeploymentId = Abc.Azure.AzureEnvironment.DeploymentId,
                OccurredOn = DateTime.UtcNow,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                Token = token,
                Message = ex.Message,
                ClassName = ex.GetType().ToString(),
                SessionIdentifier = sessionIdentifier,
            };
        }
        #endregion
    }
}