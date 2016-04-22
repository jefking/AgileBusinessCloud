// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers.Data
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// User Controller
    /// </summary>
    public class UserController : Controller
    {
        #region Members
        /// <summary>
        /// User Core
        /// </summary>
        private static readonly UserCore userCore = new UserCore();

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
        [Authorize]
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
        /// <param name="preference">User Preferences</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [Authorize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Save(UserPreference preference)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    if (null == preference)
                    {
                        return this.Json(WebResponse.Bind((int)Fault.DataNotSpecified, "Preference not specified"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var user = User.Identity.Data();
                        preference.User = new User()
                        {
                            Identifier = user.Identifier,
                        };
                        preference.Application = Application.Current;
                        preference.MaximumAllowedApplications = null;

                        var saved = userCore.Save(preference);

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

        /// <summary>
        /// Get Configuration of an Application
        /// </summary>
        /// <param name="preference">Application</param>
        /// <returns>Configuration Items</returns>
        [HttpPost]
        [Authorize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Get(UserPreference preference)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    if (null == preference)
                    {
                        return this.Json(WebResponse.Bind((int)Fault.DataNotSpecified, "Preference not specified"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var user = User.Identity.Data();
                        var app = Application.Current;
                        preference.User = user;
                        preference.Application = app;

                        var saved = userCore.Get(preference);
                        saved.CanCreateApplication = appCore.PermitApplicationCreation(app, user);
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

        /// <summary>
        /// User to Application
        /// </summary>
        /// <remarks>
        /// GET: /User/Users
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        [Authorize(Roles = "staff")]
        public ActionResult Users()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var appInfo = new ApplicationInformation()
                    {
                        Identifier = ServerConfiguration.ApplicationIdentifier,
                    };
                    var users = appCore.GetUsers(appInfo, true);

                    return this.Json(users, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// User to Application
        /// </summary>
        /// <remarks>
        /// GET: /User/UserProfiles
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult UserProfiles()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var appInfo = new ApplicationInformation()
                    {
                        Identifier = ServerConfiguration.ApplicationIdentifier,
                    };
                    var users = appCore.GetUsers(appInfo, true);

                    return this.Json(users.Select(u => u.Convert()), JsonRequestBehavior.AllowGet);
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