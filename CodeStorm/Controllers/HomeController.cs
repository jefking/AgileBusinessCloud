// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='HomeController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Code.Controllers
{
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : Controller
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        protected static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            this.ViewBag.ClientId = Abc.Configuration.ServerConfiguration.GitHubClientId;

            try
            {
                var profiles = new UserCore().PublicProfilesFull(Application.Default, false);

                ViewBag.Profiles = (from profile in profiles.Select(p => p.Convert())
                                    where !string.IsNullOrWhiteSpace(profile.UserName)
                                    orderby profile.Points descending
                                    select profile).Take(12).ToList();
            }
            catch (Exception ex)
            {
                logger.Log(ex, EventTypes.Warning, 999);
            }

            return View();
        }

        /// <summary>
        /// Home/Tribes
        /// </summary>
        /// <returns></returns>
        public ActionResult Tribes()
        {
            try
            {
                var profiles = new UserCore().PublicProfilesFull(Application.Default, false);

                ViewBag.Profiles = (from profile in profiles.Select(p => p.Convert())
                                    where !string.IsNullOrWhiteSpace(profile.UserName)
                                    orderby profile.Points descending
                                    select profile).ToList();
            }
            catch (Exception ex)
            {
                logger.Log(ex, EventTypes.Warning, 999);
            }

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Storm()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }


        [Authorize]
        public ActionResult Welcome()
        {
            return View();
        }
        #endregion
    }
}