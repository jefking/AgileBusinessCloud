// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationController.cs'>
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
    /// Configuration Controller
    /// </summary>
    [Authorize(Roles = "staff")]
    public class ConfigurationController : Controller
    {
        #region Members
        /// <summary>
        /// Application Core
        /// </summary>
        private static readonly ApplicationCore appCore = new ApplicationCore();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>Action Result</returns>
        public ActionResult Index()
        {
            using (new PerformanceMonitor())
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Save Configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Save(Configuration configuration)
        {
            using (new PerformanceMonitor())
            {
                if (null == configuration)
                {
                    return this.Json(WebResponse.Bind((int)Fault.DataNotSpecified, "Configuration not specified"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var saved = appCore.Save(configuration, User.Identity.Editor());

                        return this.Json(saved, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        /// <summary>
        /// Get Configuration of an Application
        /// </summary>
        /// <param name="application">Application</param>
        /// <returns>Configuration Items</returns>
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Get(Application application)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    if (null == application)
                    {
                        return this.Json(WebResponse.Bind((int)Fault.DataNotSpecified, "Application is null."), JsonRequestBehavior.AllowGet);
                    }
                    else if (Guid.Empty == application.Identifier)
                    {
                        return this.Json(WebResponse.Bind((int)Fault.InvalidApplicationIdentifier, "Application Identifier not specified."), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var token = new Token()
                        {
                            ApplicationId = application.Identifier,
                        };

                        var configuration = new Configuration()
                        {
                            Token = token,
                        };

                        var saved = appCore.Get(configuration);

                        return this.Json(saved, JsonRequestBehavior.AllowGet);
                    }
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