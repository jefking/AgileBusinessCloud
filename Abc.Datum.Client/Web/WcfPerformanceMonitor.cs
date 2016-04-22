// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='WcfPerformanceMonitor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Web
{
    using Abc.Diagnostics;
    using Abc.Logging;
    using Abc.Logging.Datum;
    using Abc.Underpinning;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Dispatcher;
    using System.Text;

    /// <summary>
    /// WCF Performance Monitor
    /// </summary>
    public class WcfPerformanceMonitor : IParameterInspector
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();

        /// <summary>
        /// Class Name
        /// </summary>
        private readonly Type callingClass;

        /// <summary>
        /// Started On
        /// </summary>
        private Stopwatch stopwatch = null;

        /// <summary>
        /// Input Types
        /// </summary>
        private string inputTypes = null;

        /// <summary>
        /// Session Identifier
        /// </summary>
        private Guid sessionIdentifier;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WcfPerformanceMonitor class
        /// </summary>
        /// <param name="type">Class Name</param>
        public WcfPerformanceMonitor(Type type)
        {
            this.callingClass = type;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Before Call
        /// </summary>
        /// <param name="operationName">Operation Name</param>
        /// <param name="inputs">Inputs</param>
        /// <returns>null</returns>
        public object BeforeCall(string operationName, object[] inputs)
        {
            if (ConfigurationSettings.LogPerformance)
            {
                var build = new StringBuilder();
                if (null != inputs)
                {
                    bool firstParameter = true;
                    build.Append('(');
                    foreach (var obj in inputs)
                    {
                        if (!firstParameter)
                        {
                            build.Append(',');
                        }

                        build.Append(null == obj ? "unknown" : obj.GetType().ToString());

                        firstParameter = false;
                    }

                    build.Append(')');
                }

                this.inputTypes = build.ToString();

                this.stopwatch = Stopwatch.StartNew();
                this.sessionIdentifier = Session.GetSession();
            }

            return null;
        }

        /// <summary>
        /// After Call
        /// </summary>
        /// <param name="operationName">Operation Name</param>
        /// <param name="outputs">Outputs</param>
        /// <param name="returnValue">Return Value</param>
        /// <param name="correlationState">Correlation State</param>
        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            if (null != this.stopwatch)
            {
                this.stopwatch.Stop();
            }

            if (Guid.Empty != this.sessionIdentifier)
            {
                Session.ReleaseSession();
            }

            if (ConfigurationSettings.LogPerformance)
            {
                var duration = this.stopwatch.Elapsed;
                if (ConfigurationSettings.MinimumDuration < duration)
                {
                    if (null != application.Token)
                    {
                        var method = "{0} {1}{2}".FormatWithCulture(null == returnValue ? string.Empty : returnValue.GetType().ToString(), operationName ?? "unknown", this.inputTypes);

                        var occurrence = new Occurrence();
                        occurrence.Load(duration, method, this.callingClass.ToString(), null, this.sessionIdentifier);

                        MessageHandler.Instance.Queue(occurrence);
                    }
                    else
                    {
                        Trace.Write("Application is not validated.");
                    }
                }
            }
        }
        #endregion
    }
}