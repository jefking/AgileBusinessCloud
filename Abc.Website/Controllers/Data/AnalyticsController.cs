// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AnalyticsController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers.Data
{
    using System;
    using System.Web.Mvc;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// Analytics Controller
    /// </summary>
    public class AnalyticsController : Controller
    {
        #region Members
        /// <summary>
        /// Analytics Core
        /// </summary>
        private static readonly AnalyticsCore core = new AnalyticsCore();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Current Throughput
        /// </summary>
        /// <remarks>
        /// GET:
        /// /Analytics/CurrentThroughput
        /// </remarks>
        /// <returns>Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety")]
        public ActionResult CurrentThroughput()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    return this.Json(core.Current, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion
    }
}