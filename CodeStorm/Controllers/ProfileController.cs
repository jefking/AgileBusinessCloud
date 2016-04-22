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
    using Abc.Website.Models;
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
        /// Logger
        /// </summary>
        protected static readonly LogCore logger = new LogCore();
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
            if (!string.IsNullOrWhiteSpace(username))
            {
                var page = new ProfilePage()
                {
                    Handle = username,
                    ApplicationIdentifier = Application.Default.Identifier,
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
                            Application = Application.Default,
                            User = user,
                        };

                        var userCore = new UserCore();
                        user = userCore.GetByIdentifier(userApp);
                        if (null != user)
                        {
                            var temp = user.Convert();
                            temp.Points = page.Points;
                            var publicProfile = temp.Convert();

                            var preference = new UserPreference()
                            {
                                Application = Application.Default,
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
                                    logger.Log(ex, EventTypes.Warning, 999);
                                }
                            }

                            this.ViewBag.Gravatar = publicProfile.Gravatar;
                            this.ViewBag.AbcHandle = publicProfile.AbcHandle.ToLower();
                            this.ViewBag.GithubHandle = publicProfile.GitHubHandle;
                            this.ViewBag.TwitterHandle = publicProfile.TwitterHandle;
                            return this.View(publicProfile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Warning, 999);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        /// <summary>
        /// Edit Profile
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit()
        {
            try
            {
                var source = new DomainSource();
                var user = source.GetUserByEmail(Application.Default.Identifier, base.User.Identity.Name);
                if (null != user)
                {
                    var preference = new UserPreference()
                    {
                        Application = Application.Default,
                        User = user.Convert(),
                    };

                    var userCore = new UserCore();
                    preference = userCore.Get(preference);
                    var page = new ProfilePage()
                    {
                        ApplicationIdentifier = Application.Default.Identifier,
                        Handle = preference.AbcHandle,
                    };
                    page = userCore.Get(page);
                    var profile = new UserProfile()
                    {
                        CreatedOn = user.CreatedOn,
                        Gravatar = string.IsNullOrWhiteSpace(user.Email) ? null : user.Email.GetHexMD5(),
                        UserName = user.UserName,
                        Email = user.Email,
                        TimeZone = preference.TimeZone,
                        MaximumAllowedApplications = preference.MaximumAllowedApplications,
                        CurrentApplicationIdentifier = preference.CurrentApplication == null ? Guid.Empty : preference.CurrentApplication.Identifier,
                        TwitterHandle = preference.TwitterHandle,
                        AbcHandle = preference.AbcHandle,
                        City = preference.City,
                        Country = preference.Country,
                        GitHubHandle = preference.GitHubHandle,
                        GitId = page.GitId,
                        GitAvatarUrl = page.GitAvatarUrl,
                        GitGravatarId = page.GitGravatarId,
                        GitUrl = page.GitUrl,
                        GitBlog = page.GitBlog,
                        GitHireable = page.GitHireable,
                        GitBiography = page.GitBiography,
                        GitPublicGists = page.GitPublicGists,
                        GitPublicRepos = page.GitPublicRepos,
                        GitFollowers = page.GitFollowers,
                        GitFollowing = page.GitFollowing,
                        GitHtmlUrl = page.GitHtmlUrl,
                        GitCreatedAt = page.GitCreatedAt,
                        GitType = page.GitType,
                        GitAccessToken = page.GitAccessToken,
                        GitCode = page.GitCode,
                        Word = page.Word,
                    };

                    profile.TimeZones = TimeZoneInfo.GetSystemTimeZones().Select(tz => new SelectListItem()
                    {
                        Text = tz.DisplayName,
                        Value = tz.Id,
                        Selected = tz.Id == profile.TimeZone.Id,
                    });

                    return View(profile);
                }
                else
                {
                    return base.RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex, EventTypes.Warning, 999);
            }

            return base.RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            var twitterHandle = collection["TwitterHandle"];
            if (string.IsNullOrWhiteSpace(twitterHandle))
            {
                return base.RedirectToAction("Index", "Home");
            }
            else
            {
                var source = new DomainSource();
                var user = source.GetUserByEmail(Application.Default.Identifier, base.User.Identity.Name);
                var preference = new UserPreference()
                {
                    TwitterHandle = twitterHandle,
                    User = user.Convert(),
                    Application = Application.Default,
                };

                var core = new UserCore();
                core.Save(preference);

                return this.Edit();
            }
        }
        #endregion
    }
}