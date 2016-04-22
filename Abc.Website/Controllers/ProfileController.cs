// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ProfileController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Public Profile Controller
    /// </summary>
    public class ProfileController : Controller
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        public LogCore log = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Default Page
        /// </summary>
        /// <remarks>
        /// GET: /
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Index(string username)
        {
            using (new PerformanceMonitor())
            {
                if (!string.IsNullOrWhiteSpace(username))
                {
                    var page = new ProfilePage()
                    {
                        Handle = username,
                        ApplicationIdentifier = Application.Current.Identifier,
                    };

                    try
                    {
                        var core = new UserCore();
                        page = core.Get(page);
                        if (null != page)
                        {
                            var user = new User()
                            {
                                Identifier = page.OwnerIdentifier,
                            };

                            var userApp = new UserApplication()
                            {
                                Application = Application.Current,
                                User = user,
                            };

                            var userCore = new UserCore();
                            user = userCore.GetByIdentifier(userApp);
                            if (null != user)
                            {
                                var publicProfile = user.Convert().Convert();

                                var preference = new UserPreference()
                                {
                                    Application = Application.Current,
                                    User = user,
                                };

                                preference = userCore.Get(preference);

                                publicProfile.Set(preference);

                                if (!string.IsNullOrWhiteSpace(publicProfile.TwitterHandle))
                                {
                                    try
                                    {
                                        var twitter = new TwitterSource();
                                        publicProfile.Tweets = twitter.ByUser(publicProfile.TwitterHandle, 10).ToList();
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Log(ex, EventTypes.Warning, (int)Fault.TwitterFailure);
                                    }

                                }
                                return this.View(publicProfile);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log(ex, EventTypes.Warning, (int)ServiceFault.Empty);
                    }
                }

                return this.RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
