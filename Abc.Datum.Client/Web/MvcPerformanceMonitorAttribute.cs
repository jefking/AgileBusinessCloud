// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='MvcPerformanceMonitorAttribute.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Web
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Web.Mvc;
    using Abc.Logging;
    using Abc.Logging.Datum;
    using Abc.Underpinning;
    using Abc.Diagnostics;

    /// <summary>
    /// MVC Performance Monitor Attribute
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Justification = "Technology used.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Unlikely to have access to modify boolean during runtime.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1018:MarkAttributesWithAttributeUsage", Justification = "Allowing developers to decide.")]
    public sealed class MvcPerformanceMonitorAttribute : ActionFilterAttribute
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = new Logger();

        /// <summary>
        /// Log Unhandled Exceptions
        /// </summary>
        private readonly bool logExceptions = true;

        /// <summary>
        /// Started On
        /// </summary>
        private Stopwatch stopwatch = null;

        /// <summary>
        /// Session Identifier
        /// </summary>
        private Guid sessionIdentifier = Guid.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MvcPerformanceMonitorAttribute class
        /// </summary>
        public MvcPerformanceMonitorAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MvcPerformanceMonitorAttribute class
        /// </summary>
        /// <param name="logUnhandledExceptions">Log Unhandled Exceptions</param>
        public MvcPerformanceMonitorAttribute(bool logUnhandledExceptions)
        {
            this.logExceptions = logUnhandledExceptions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// On Action Executing
        /// </summary>
        /// <param name="filterContext">Fliter Context</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (ConfigurationSettings.LogPerformance)
            {
                this.stopwatch = Stopwatch.StartNew();
                sessionIdentifier = Session.GetSession();
            }
        }

        /// <summary>
        /// On Action Executed
        /// </summary>
        /// <param name="filterContext">Fliter Context</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (null != this.stopwatch)
            {
                this.stopwatch.Stop();
            }

            if (Guid.Empty != this.sessionIdentifier)
            {
                Session.ReleaseSession();
            }

            if (null != filterContext)
            {
                if (ConfigurationSettings.LogPerformance)
                {
                    var duration = this.stopwatch.Elapsed;
                    if (ConfigurationSettings.MinimumDuration < duration)
                    {
                        if (null != application.Token)
                        {
                            var parameters = new StringBuilder();
                            bool firstParameter = true;
                            parameters.Append('(');
                            foreach (var parameter in filterContext.ActionDescriptor.GetParameters())
                            {
                                if (!firstParameter)
                                {
                                    parameters.Append(',');
                                }

                                parameters.AppendFormat("{0} {1}", parameter.ParameterType, parameter.ParameterName);

                                firstParameter = false;
                            }

                            parameters.Append(')');

                            var method = "{0} {1}{2}".FormatWithCulture(typeof(ActionResult).ToString(), filterContext.ActionDescriptor.ActionName, parameters);

                            var occurrence = new Occurrence();
                            occurrence.Load(duration, method, filterContext.Controller.ToString(), null, sessionIdentifier);

                            MessageHandler.Instance.Queue(occurrence);
                        }
                        else
                        {
                            Trace.Write("Application is not validated.");
                        }
                    }
                }

                if (this.logExceptions && null != filterContext.Exception && !filterContext.ExceptionHandled)
                {
                    logger.Log(filterContext.Exception);
                }
            }

            base.OnActionExecuted(filterContext);
        }
        #endregion
    }
}