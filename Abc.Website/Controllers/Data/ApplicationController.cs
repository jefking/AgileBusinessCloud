// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers.Data
{
    using System;
    using System.Web.Mvc;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// Application Controller
    /// </summary>
    [Authorize]
    public class ApplicationController : Controller
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
        /// Get Applications
        /// </summary>
        /// <returns>Application Details Models</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Public web interface interface.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult GetApplications()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var applications = appCore.Get(this.User.Identity.Data(), ServerConfiguration.ApplicationIdentifier);

                    return this.Json(applications, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Save Application Information
        /// </summary>
        /// <param name="appInfo">Application Information</param>
        /// <returns>Updated Application Information</returns>
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult SaveApplicationInformation(ApplicationInformation appInfo)
        {
            using (new PerformanceMonitor())
            {
                if (null == appInfo)
                {
                    return this.Json(WebResponse.Bind((int)Fault.DataNotSpecified, "Application Information not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var userApplication = new ApplicationInformation()
                        {
                            Identifier = Settings.ApplicationIdentifier,
                        };
                        var updated = appCore.Save(appInfo, User.Identity.Editor(), userApplication);

                        return this.Json(updated, JsonRequestBehavior.AllowGet);
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
        /// Add User
        /// </summary>
        /// <param name="user">User Id</param>
        /// <param name="application">Application Id</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        [Authorize(Roles = "staff")]
        public ActionResult AddUser(Guid user, Guid application)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var appInfo = new Application()
                    {
                        Identifier = application,
                    };
                    var u = new User()
                    {
                        Identifier = user,
                    };
                    var userAppData = new UserApplication()
                    {
                        Application = appInfo,
                        Active = true,
                        Deleted = false,
                        User = u
                    };
                    var bound = appCore.Save(userAppData, User.Identity.Editor());

                    return this.Json(bound, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Application Count for current User
        /// </summary>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult ApplicationCount()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var user = User.Identity.Data();
                    if (User.Identity.IsManager())
                    {
                        return this.Json(100, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return this.Json(appCore.ApplicationCount(user), JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Can Create An Application
        /// </summary>
        /// <remarks>
        /// GET /Data/Application/CanCreateAnApplication
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult CanCreateAnApplication()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var user = User.Identity.Data();
                    
                    return this.Json(appCore.PermitApplicationCreation(Application.Current, user), JsonRequestBehavior.AllowGet);
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