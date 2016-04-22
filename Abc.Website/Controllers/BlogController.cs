// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BlogController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;

    /// <summary>
    /// Blog Controller
    /// </summary>
    public class BlogController : Controller
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        private static readonly LogCore log = new LogCore();

        /// <summary>
        /// Content Core
        /// </summary>
        private static readonly ContentCore core = new ContentCore();
        #endregion

        #region Methods
        /// <summary>
        /// Default Page
        /// </summary>
        /// <remarks>
        /// URL: /Blog/Index
        /// </remarks>
        /// <param name="identifier">Identifier</param>
        /// <returns>Action Result</returns>
        public ActionResult Index(Guid? identifier)
        {
            using (new PerformanceMonitor())
            {
                var model = new BlogModel();
                try
                {
                    var entry = new BlogEntry()
                    {
                        SectionIdentifier = BlogEntry.Company,
                        Identifier = identifier ?? Guid.Empty,
                    };

                    model.Posts = (from d in core.Get(entry)
                                   orderby d.PostedOn descending
                                   select d).ToList();
                    model.Post = (from item in model.Posts
                                  where !string.IsNullOrWhiteSpace(item.Content)
                                  select item).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Warning, (int)Fault.Unknown);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Default Page
        /// </summary>
        /// <remarks>
        /// URL: /Blog/JaimeBueza
        /// </remarks>
        /// <param name="identifier">Identifier</param>
        /// <returns>Action Result</returns>
        public ActionResult JaimeBueza(Guid? identifier)
        {
            using (new PerformanceMonitor())
            {
                var model = new BlogModel();
                try
                {
                    var entry = new BlogEntry()
                    {
                        SectionIdentifier = BlogEntry.JaimeBueza,
                        Identifier = identifier ?? Guid.Empty,
                    };

                    model.Posts = (from d in core.Get(entry)
                                   orderby d.PostedOn descending
                                   select d).ToList();
                    model.Post = (from item in model.Posts
                                  where !string.IsNullOrWhiteSpace(item.Content)
                                  select item).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Warning, (int)Fault.Unknown);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Default Page
        /// </summary>
        /// <remarks>
        /// URL: /Blog/JefKing
        /// </remarks>
        /// <param name="identifier">Identifier</param>
        /// <returns>Action Result</returns>
        public ActionResult JefKing(Guid? identifier)
        {
            using (new PerformanceMonitor())
            {
                var model = new BlogModel();
                try
                {
                    var entry = new BlogEntry()
                    {
                        SectionIdentifier = BlogEntry.JefKing,
                        Identifier = identifier ?? Guid.Empty,
                    };

                    model.Posts = (from d in core.Get(entry)
                                   orderby d.PostedOn descending
                                   select d).ToList();
                    model.Post = (from item in model.Posts
                                  where !string.IsNullOrWhiteSpace(item.Content)
                                  select item).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Warning, (int)Fault.Unknown);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Default Page
        /// </summary>
        /// <remarks>
        /// URL: /Blog/GeorgeDanes
        /// </remarks>
        /// <param name="identifier">Identifier</param>
        /// <returns>Action Result</returns>
        [Authorize(Roles = "staff")]
        public ActionResult GeorgeDanes(Guid? identifier)
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }
        #endregion
    }
}