// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='HelpController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using System.Web.Mvc;
    using Abc.Services;
    using Abc.Services.Core;

    /// <summary>
    /// Help Controller
    /// </summary>
    public class HelpController : Controller
    {
        #region Members
        /// <summary>
        /// Application Core
        /// </summary>
        private static readonly ApplicationCore appCore = new ApplicationCore();
        #endregion

        #region Methods
        /// <summary>
        /// Default Page
        /// </summary>
        /// <remarks>
        /// GET: /
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Index()
        {
            using (new PerformanceMonitor())
            {
                ViewBag.CanCreateAnApplication = this.User.Identity.IsAuthenticated && this.CanCreateAnApplication();
                return View();
            }
        }

        /// <summary>
        /// Amazing Insights
        /// </summary>
        /// <remarks>
        /// GET: /AmazingInsights
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult AmazingInsights()
        {
            using (new PerformanceMonitor())
            {
                ViewBag.CanCreateAnApplication = this.User.Identity.IsAuthenticated && this.CanCreateAnApplication();
                return View();
            }
        }

        /// <summary>
        /// Amazing Insights Collector
        /// </summary>
        /// <remarks>
        /// GET: /AmazingInsightsCollector
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult AmazingInsightsCollector()
        {
            using (new PerformanceMonitor())
            {
                ViewBag.CanCreateAnApplication = this.User.Identity.IsAuthenticated && this.CanCreateAnApplication();
                return View();
            }
        }

        /// <summary>
        /// Amazing Insights Collector Notifications
        /// </summary>
        /// <remarks>
        /// GET: /Notifications
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Notifications()
        {
            using (new PerformanceMonitor())
            {
                ViewBag.CanCreateAnApplication = this.User.Identity.IsAuthenticated && this.CanCreateAnApplication();
                return View();
            }
        }

        /// <summary>
        /// Configuration
        /// </summary>
        /// <remarks>
        /// GET: /Configuration
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Configuration()
        {
            using (new PerformanceMonitor())
            {
                ViewBag.CanCreateAnApplication = this.User.Identity.IsAuthenticated && this.CanCreateAnApplication();
                return View();
            }
        }

        /// <summary>
        /// Check if the current user Can Create An Application
        /// </summary>
        /// <returns>Can Create An Application</returns>
        private bool CanCreateAnApplication()
        {
            using (new PerformanceMonitor())
            {
                return appCore.PermitApplicationCreation(Abc.Services.Contracts.Application.Current, User.Identity.Data());
            }
        }
        #endregion
    }
}