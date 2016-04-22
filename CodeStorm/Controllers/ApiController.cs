// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApiController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Code.Controllers
{
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website;
    using System;
    using System.Web.Mvc;

    public class ApiController : Controller
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        protected static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        [Authorize]
        public ActionResult AddUserToTribe(string tribe)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tribe))
                {
                    return this.Json(WebResponse.Bind((int)Fault.Unknown, "Tribe must be specified."), JsonRequestBehavior.AllowGet);
                }

                var source = new DomainSource();
                var user = source.GetUserByEmail(Application.Default.Identifier, base.User.Identity.Name);
                var core = new UserCore();
                core.Save(user.Convert(), tribe);
            }
            catch (Exception ex)
            {
                logger.Log(ex, EventTypes.Warning, 999);
                return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        #endregion
    }
}