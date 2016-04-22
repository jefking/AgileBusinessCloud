// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='HomeController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// Home Controller
    /// </summary>
    [HandleError]
    public class HomeController : SocialController
    {
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
                this.ViewBag.Profiles = this.Profiles();

                return View();
            }
        }

        /// <summary>
        /// 404, Not Found Page
        /// </summary>
        /// <remarks>
        /// GET: /NotFound
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult NotFound()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Investors Page
        /// </summary>
        /// <remarks>
        /// GET: /Investors
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Investors()
        {
            using (new PerformanceMonitor())
            {
                return base.RedirectToActionPermanent("Investors", "Corporate");
            }
        }

        /// <summary>
        /// SiteMap
        /// </summary>
        /// <remarks>
        /// GET: /SiteMap
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult SiteMap()
        {
            using (new PerformanceMonitor())
            {
                this.ViewBag.Profiles = this.Profiles(int.MaxValue);

                return View();
            }
        }

        /// <summary>
        /// ABC Open Source Initiatives
        /// </summary>
        /// <remarks>
        /// GET: /Home/OpenSource
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult OpenSource()
        {
            using (new PerformanceMonitor())
            {
                this.ViewBag.Profiles = this.Profiles();

                return View();
            }
        }

        /// <summary>
        /// About
        /// </summary>
        /// <remarks>
        /// GET: /Home/About
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult About()
        {
            using (new PerformanceMonitor())
            {
                return base.RedirectToActionPermanent("About", "Corporate");
            }
        }

        /// <summary>
        /// Contact
        /// </summary>
        /// <remarks>
        /// GET: /Home/Contact
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Contact()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Partner
        /// </summary>
        /// <remarks>
        /// GET: /Home/Partner
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Partner()
        {
            using (new PerformanceMonitor())
            {
                return base.RedirectToActionPermanent("Partner", "Corporate");
            }
        }

        /// <summary>
        /// Amazing Insights
        /// </summary>
        /// <remarks>
        /// GET: /Home/AmazingInsights
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult AmazingInsights()
        {
            using (new PerformanceMonitor())
            {
                this.ViewBag.Profiles = this.Profiles();

                return View();
            }
        }

        /// <summary>
        /// Amazing Insights Collector
        /// </summary>
        /// <remarks>
        /// GET: /Home/AmazingInsightsCollector
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult AmazingInsightsCollector()
        {
            using (new PerformanceMonitor())
            {
                var query = new LogQuery()
                {
                    ApplicationIdentifier = new Guid("72b66fdd-0f4c-484c-88e1-b7c64bce545a"),
                };

                IEnumerable<ServerStatisticSetDisplay> servers = null;
                try
                {
                    var source = new LogCore();
                    servers = source.LatestServerStatistics(query);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Warning, (int)Fault.Unknown);
                }

                return View(servers);
            }
        }
        
        /// <summary>
        /// Error
        /// </summary>
        /// <remarks>
        /// GET: /Home/Error
        /// </remarks>
        /// <param name="aspxerrorpath">ASPX Error Path</param>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Error message, unable to control")]
        public ActionResult Error(string aspxerrorpath)
        {
            using (new PerformanceMonitor())
            {
                logger.Log(aspxerrorpath);

                return View();
            }
        }

        /// <summary>
        /// Privacy Policy
        /// </summary>
        /// <remarks>
        /// GET: /Home/PrivacyPolicy
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult PrivacyPolicy()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Terms Of Use
        /// </summary>
        /// <remarks>
        /// GET: /Home/TermsOfUse
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult TermsOfUse()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Training
        /// </summary>
        /// <remarks>
        /// GET: /Home/Training
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Training()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Handle Unknown Action
        /// </summary>
        /// <param name="actionName">Action Name</param>
        protected override void HandleUnknownAction(string actionName)
        {
            ViewData["actionName"] = actionName;
            View("NotFound").ExecuteResult(this.ControllerContext);
        }
        #endregion
    }
}