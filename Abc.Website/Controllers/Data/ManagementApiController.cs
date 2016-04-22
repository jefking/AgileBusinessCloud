// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ManagementApiController.cs'>
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
    using Abc.Website.Models;
    using Abc.Website.Security;

    /// <summary>
    /// Management Api Controller
    /// </summary>
    [Authorize(Roles = "staff")]
    public class ManagementApiController : Controller
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Get Roles
        /// </summary>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Public web interface interface.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult GetRoles()
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var roles = new AzureRoleProvider();
                    return this.Json(roles.GetAllRoles(), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Save User Role
        /// </summary>
        /// <param name="userRole">User Role</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult SaveUserRole(UserRole userRole)
        {
            using (new PerformanceMonitor())
            {
                if (null == userRole)
                {
                    return this.Json(WebResponse.Bind((int)Fault.DataNotSpecified, "User Role not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var userCore = new UserCore();
                        var userId = new User()
                        {
                            Identifier = userRole.UserIdentifier,
                        };
                        var userApp = new UserApplication()
                        {
                            User = userId,
                            Application = Application.Current,
                        };
                        var user = userCore.Get(userApp);

                        var roles = new AzureRoleProvider();
                        roles.AddUserToRole(user.Email, userRole.RoleName);
                        return this.Json(new WebResponse(), JsonRequestBehavior.AllowGet);
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
        /// Delete User Role
        /// </summary>
        /// <param name="userRole">User Role</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult DeleteUserRole(UserRole userRole)
        {
            using (new PerformanceMonitor())
            {
                if (null == userRole)
                {
                    return this.Json(WebResponse.Bind((int)Fault.DataNotSpecified, "User Role not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var userCore = new UserCore();
                        var userId = new User()
                        {
                            Identifier = userRole.UserIdentifier,
                        };
                        var userApp = new UserApplication()
                        {
                            User = userId,
                            Application = Application.Current,
                        };
                        var user = userCore.Get(userApp);

                        var roles = new AzureRoleProvider();
                        roles.RemoveUserFromRole(user.UserName, userRole.RoleName);

                        return this.Json(new WebResponse(), JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        #endregion
    }
}