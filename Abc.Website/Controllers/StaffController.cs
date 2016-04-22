// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='StaffController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;

    /// <summary>
    /// Staff Controller
    /// </summary>
    [Authorize(Roles = "staff")]
    public class StaffController : SocialController
    {
        #region Members
        /// <summary>
        /// Application Core
        /// </summary>
        private static readonly ApplicationCore appCore = new ApplicationCore();

        /// <summary>
        /// CMS Core
        /// </summary>
        private static readonly ContentCore cmsCore = new ContentCore();
        #endregion

        #region Methods
        /// <summary>
        /// Default
        /// </summary>
        /// <remarks>
        /// GET: /Staff
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Index()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Blog
        /// </summary>
        /// <remarks>
        /// GET: /Staff/Blog
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Blog()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Blog
        /// </summary>
        /// <remarks>
        /// POST: /Staff/Blog
        /// </remarks>
        /// <returns>Action Result</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Blog(FormCollection form)
        {
            using (new PerformanceMonitor())
            {
                var entry = new BlogEntry()
                {
                    Content = form["html"],
                    PostedOn = DateTime.UtcNow,
                    Title = form["title"],
                };
                int index;
                if (int.TryParse(form["blogType"], out index))
                {
                    switch (index)
                    {
                        case 0:
                            entry.SectionIdentifier = BlogEntry.Company;
                            break;
                        case 1:
                            entry.SectionIdentifier = BlogEntry.JefKing;
                            break;
                        case 2:
                            entry.SectionIdentifier = BlogEntry.MarkWoodward;
                            break;
                        case 3:
                            entry.SectionIdentifier = BlogEntry.JaimeBueza;
                            break;
                    }
                }

                cmsCore.Store(entry);

                return this.Blog();
            }
        }

        /// <summary>
        /// Save Content
        /// </summary>
        /// <typeparam name="T">Text Content Type</typeparam>
        /// <param name="content">Content</param>
        private void Save<T>(string content)
            where T : TextContent
        {
            var container = new XmlContainer<T>(ServerConfiguration.Default);
            var blogPost = Activator.CreateInstance<T>();
            blogPost.Content = content;
            blogPost.CreatedOn = DateTime.UtcNow;
            blogPost.Active = true;
            blogPost.Deleted = false;
            blogPost.Id = Guid.NewGuid();
            container.Save(blogPost.Id.ToString(), blogPost);
        }

        /// <summary>
        /// User Gallery
        /// </summary>
        /// <remarks>
        /// GET: /Staff/userGallery
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult UserGallery()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Users
        /// </summary>
        /// <remarks>
        /// GET: /Staff/Users
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Users()
        {
            using (new PerformanceMonitor())
            {
                this.ViewBag.Profiles = this.Profiles(int.MaxValue);

                return View();
            }
        }

        /// <summary>
        /// ApplicationSettings
        /// </summary>
        /// <remarks>
        /// GET: /Management/ApplicationSettings
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult ApplicationSettings()
        {
            using (new PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// User to Application
        /// </summary>
        /// <remarks>
        /// GET: /Management/UserToApplication
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult UserToApplication()
        {
            using (new PerformanceMonitor())
            {
                if (User.Identity.IsManager())
                {
                    var appInfo = new ApplicationInformation()
                    {
                        Identifier = ServerConfiguration.ApplicationIdentifier,
                    };
                    var model = new UserApplicationModel()
                    {
                        Applications = this.GetApplications(),
                        Users = appCore.GetUsers(appInfo),
                    };

                    return this.View(model);
                }
                else
                {
                    return this.RedirectToAction("Index", "Home");
                }
            }
        }

        /// <summary>
        /// User to Application
        /// </summary>
        /// <remarks>
        /// POST: /Management/UserToApplication
        /// </remarks>
        /// <param name="user">User Id</param>
        /// <param name="application">Application Id</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult UserToApplication(Guid user, Guid application)
        {
            using (new PerformanceMonitor())
            {
                if (User.Identity.IsManager())
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
                        User = u,
                    };
                    appCore.Save(userAppData, User.Identity.Editor());

                    return this.RedirectToAction("UserToApplication");
                }
                else
                {
                    return this.RedirectToAction("Index", "Home");
                }
            }
        }

        /// <summary>
        /// Get Applications
        /// </summary>
        /// <returns>Application Details Models</returns>
        private IEnumerable<ApplicationDetailsModel> GetApplications()
        {
            var applications = appCore.Get(User.Identity.Data(), ServerConfiguration.ApplicationIdentifier);
            return applications.Convert(User.Identity.Data());
        }

        /// <summary>
        /// Initialize Model
        /// </summary>
        /// <returns>Management Model</returns>
        public ManagementModel InitializeModel()
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();
                model.Applications = this.GetApplications();
                model.Preference = GetPreference(ServerConfiguration.ApplicationIdentifier, User.Identity.Data().Identifier);
                if (null == model.Preference || null == model.Preference.CurrentApplication || Guid.Empty == model.Preference.CurrentApplication.Identifier)
                {
                    model.Application = model.Applications.FirstOrDefault();
                }
                else
                {
                    model.Application = (from data in model.Applications
                                         where data.ApplicationId == model.Preference.CurrentApplication.Identifier
                                         select data).FirstOrDefault();
                }

                return model;
            }
        }

        /// <summary>
        /// Get Preference
        /// </summary>
        /// <param name="appId">Application Identifier</param>
        /// <param name="userId">User Identifier</param>
        /// <returns>User Preference</returns>
        private static UserPreference GetPreference(Guid appId, Guid userId)
        {
            using (new PerformanceMonitor())
            {
                var app = new Application()
                {
                    Identifier = appId,
                };
                var user = new User()
                {
                    Identifier = userId,
                };
                var pref = new UserPreference()
                {
                    Application = app,
                    User = user,
                };

                var core = new UserCore();
                return core.Get(pref);
            }
        }
        #endregion
    }
}