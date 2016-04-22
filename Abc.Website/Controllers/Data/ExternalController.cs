// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExternalController.cs'>
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
    using Abc.Services.Data;

    /// <summary>
    /// External Controller
    /// </summary>
    public class ExternalController : Controller
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Company Tweets
        /// </summary>
        /// <remarks>
        /// /External/CompanyTweets
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult CompanyTweets()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var source = new TwitterSource();
                    return this.Json(source.Employees(10), JsonRequestBehavior.AllowGet);
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