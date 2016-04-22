// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Program.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Client.Collector
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Abc.Instrumentation;
    using Abc.Logging;

    /// <summary>
    /// Collector Program
    /// </summary>
    public static class Program
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = new Logger();

        /// <summary>
        /// Console Title
        /// </summary>
        private const string ConsoleTitle = "Agile Business Cloud Solutions Ltd. - Amazing Insights Collector";
        #endregion

        #region Methods
        /// <summary>
        /// Main Entry Point for the application
        /// </summary>
        /// <param name="args">Arguments</param>
        public static void Main(string[] args)
        {
            Console.Title = ConsoleTitle;

            var uniqueString = "Collector_{0}".FormatWithCulture(ConfigurationSettings.ApplicationIdentifier);
            using (var mutex = new Mutex(false, uniqueString))
            {
                if (!mutex.WaitOne(0, false))
                {
                    Trace.Write("Collector is already started, exiting.");
                }
                else
                {
                    Trace.WriteLine("Collector Starting.");

                    Sampler sampler = null;
                    try
                    {
                        if (ConfigurationSettings.InstrumentServer)
                        {
                            Trace.WriteLine("Server Instrumentation starting.");

                            sampler = new Sampler();
                            sampler.StoreSamples(null);

                            Trace.WriteLine("Server Instrumentation started.");
                        }
                        else
                        {
                            Trace.WriteLine("Server Instrumentation not configured, you may turn this on in App.config.");
                        }

                        if (ConfigurationSettings.LogWindowsEvents)
                        {
                            Trace.WriteLine("Windows Event logging starting.");

                            var items = EventLog.GetEventLogs();
                            foreach (var log in items)
                            {
                                log.EnableRaisingEvents = true;
                                log.EntryWritten += (s, e) => Logger.Log(e.Entry);

                                Trace.WriteLine("Watching Event Log: '{0}' for new entries.".FormatWithCulture(log.LogDisplayName));
                            }

                            Trace.WriteLine("Windows Event logging started.");
                        }
                        else
                        {
                            Trace.WriteLine("Windows Event logging not configured, turn on in App.config.");
                        }

                        short i = 0;

                        while (true)
                        {
                            if (null != sampler)
                            {
                                if (i == 300)
                                {
                                    sampler.StoreSamples(null);
                                    i = 0;
                                }
                                else
                                {
                                    i++;
                                }
                            }

                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex, EventTypes.Critical);
                    }
                    finally
                    {
                        if (null != sampler)
                        {
                            sampler.Dispose();
                            sampler = null;
                        }
                    }

                    Trace.WriteLine("Collector Stopping.");
                }
            }

            GC.Collect();
        }
        #endregion
    }
}