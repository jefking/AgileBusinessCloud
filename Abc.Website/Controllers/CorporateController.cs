// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='CorporateController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using System.Web.Mvc;
    using Abc.Services;
    using Abc.Services.Core;

    /// <summary>
    /// Corporate Controller
    /// </summary>
    public class CorporateController : Controller
    {
        #region Members
        /// <summary>
        /// Logging
        /// </summary>
        private static readonly LogCore log = new LogCore();
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
                return base.RedirectToAction("Index", "Home");
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
                return View();
            }
        }
        #endregion
    }
}