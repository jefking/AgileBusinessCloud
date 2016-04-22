// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AccountController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Abc.Website.Models;
    using Abc.Website.Security;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Protocols.WSFederation;
    using Microsoft.IdentityModel.Web;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Principal;
    using System.Web.Mvc;
    using System.Web.Security;
    using Service = Abc.Services;

    /// <summary>
    /// Account Controller
    /// </summary>
    public class AccountController : Controller
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Abc.Services.Core.LogCore log = new Abc.Services.Core.LogCore();

        /// <summary>
        /// User Core
        /// </summary>
        private static readonly UserCore userCore = new UserCore();
        #endregion

        #region Methods
        /// <summary>
        /// Log On
        /// </summary>
        /// <remarks>
        /// URL: /Account/LogOn
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult LogOn()
        {
            using (new Service.PerformanceMonitor())
            {
                return View();
            }
        }

        /// <summary>
        /// Log On
        /// </summary>
        /// <remarks>
        /// URL: /Account/
        /// param: rm=0
        /// param: id=passive
        /// param: ru=%2fStaff%2fDataUsage
        /// </remarks>
        /// <param name="rm">rm = Not sure what this is for</param>
        /// <param name="id">id = Not sure what this is for</param>
        /// <param name="ru">Redirect Url</param>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "ACS parameter, cannot control")]
        public ActionResult Index(string rm, string id, string ru)
        {
            using (new Service.PerformanceMonitor())
            {
                log.Log("/Account/rm={0}&id={1}&ru={2}".FormatWithCulture(rm, id, ru));

                if (string.IsNullOrWhiteSpace(ru))
                {
                    return this.RedirectToAction("Index", "Home");
                }
                else
                {
                    return this.Redirect(ru);
                }
            }
        }

        /// <summary>
        /// Log On
        /// </summary>
        /// <remarks>
        /// URL: /Account/LogOn
        /// POST
        /// </remarks>
        /// <param name="forms">Form Collection</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LogOn(FormCollection forms)
        {
            using (new Service.PerformanceMonitor())
            {
                var newUser = this.Register(User.Identity);

                var message = WSFederationMessage.CreateFromNameValueCollection(new Uri("http://www.notused.com"), forms);
                var returnUrl = message != null ? message.Context : null;
                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    if (newUser)
                    {
                        //return RedirectToAction("Details", "Apps", new { appId = Guid.Empty });
                        return RedirectToAction("Profile", "Account");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
        }

        /// <summary>
        /// Log On
        /// </summary>
        /// <remarks>
        /// URL: /Account/Profile
        /// </remarks>
        /// <returns>Action Result</returns>
        [Authorize]
        public ActionResult Profile()
        {
            using (new Service.PerformanceMonitor())
            {
                var user = User.Identity.Data();
                if (null != user)
                {
                    var preference = new UserPreference()
                    {
                        Application = Application.Current,
                        User = user,
                    };

                    preference = userCore.Get(preference);
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
        }

        /// <summary>
        /// Log On
        /// </summary>
        /// <remarks>
        /// URL: /Account/GitHub
        /// </remarks>
        /// <returns>Action Result</returns>
        [Authorize]
        public ActionResult GitHub()
        {
            using (new Service.PerformanceMonitor())
            {
                var user = User.Identity.Data();
                var preference = new UserPreference()
                {
                    Application = Application.Current,
                    User = user,
                };

                preference = userCore.Get(preference);
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
                };

                this.ViewBag.Profiles = this.Profiles();

                return View(profile);
            }
        }

        /// <summary>
        /// Log On
        /// </summary>
        /// <remarks>
        /// URL: /Account/Twitter
        /// </remarks>
        /// <returns>Action Result</returns>
        [Authorize]
        public ActionResult Twitter()
        {
            using (new Service.PerformanceMonitor())
            {
                var user = User.Identity.Data();
                var preference = new UserPreference()
                {
                    Application = Application.Current,
                    User = user,
                };

                preference = userCore.Get(preference);
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
                };

                if (!string.IsNullOrWhiteSpace(profile.TwitterHandle))
                {
                    try
                    {
                        var twitter = new TwitterSource();
                        profile.Tweets = twitter.ByUser(preference.TwitterHandle, 10).ToList();
                    }
                    catch (Exception ex)
                    {
                        log.Log(ex, EventTypes.Warning, (int)Fault.TwitterFailure);
                    }
                }

                this.ViewBag.Profiles = this.Profiles();

                return View(profile);
            }
        }

        /// <summary>
        /// Profile
        /// </summary>
        /// <param name="collection">Form Collection</param>
        /// <returns>Profile</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Profile(FormCollection collection)
        {
            using (new Service.PerformanceMonitor())
            {
                try
                {
                    var preference = new UserPreference()
                    {
                        Application = Application.Current,
                        User = User.Identity.Data(),
                    };

                    var timeZoneId = collection["TimeZone.Id"];
                    preference = userCore.Get(preference);
                    preference.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    preference.TwitterHandle = collection["TwitterHandle"];
                    preference.GitHubHandle = collection["GitHubHandle"];
                    preference.AbcHandle = collection["AbcHandle"];
                    preference.Country = collection["Country"];
                    preference.City = collection["City"];
                    userCore.Save(preference);

                    var source = new DomainSource();
                    
                    var user = source.GetUserByNameIdentifier(Application.Current.Identifier, User.Identity.NameIdentifier());
                    user.Email = collection["Email"];
                    user.UserName = collection["UserName"];
                    user.LastActivityOn = DateTime.UtcNow;

                    source.Update(user);
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Warning, (int)Fault.Unknown);
                }

                return this.Profile();
            }
        }

        /// <summary>
        /// Log Off
        /// </summary>
        /// <remarks>
        /// URL: /Account/LogOff
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult LogOff()
        {
            using (new Service.PerformanceMonitor())
            {
                try
                {
                    FormsAuthentication.SignOut();
                }
                finally
                {
                    var fam = FederatedAuthentication.WSFederationAuthenticationModule;
                    fam.SignOut(true);
                }

                // Return to home after LogOff
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Register user on site
        /// </summary>
        /// <param name="userIdentity">user Identity</param>
        /// <returns>New User</returns>
        private bool Register(IIdentity userIdentity)
        {
            using (new Service.PerformanceMonitor())
            {
                if (userIdentity.IsAuthenticated)
                {
                    var identity = (IClaimsIdentity)userIdentity;

                    var register = new RegisterModel()
                    {
                        Email = User.Identity.EmailAddress(),
                        NameIdentifier = User.Identity.NameIdentifier(),
                        UserName = identity.Name,
                    };

                    var source = new DomainSource();
                    UserData user = null;
                    if (!string.IsNullOrWhiteSpace(register.NameIdentifier) && null != (user = source.GetUserByNameIdentifier(ServerConfiguration.ApplicationIdentifier, register.NameIdentifier)))
                    {
                        user.LastLoggedInOn = DateTime.UtcNow;
                        user.LastActivityOn = DateTime.UtcNow;

                        source.Update(user);
                    }
                    else if (!string.IsNullOrWhiteSpace(register.Email) && null != (user = source.GetUserByEmail(ServerConfiguration.ApplicationIdentifier, register.Email)))
                    {
                        user.LastLoggedInOn = DateTime.UtcNow;
                        user.LastActivityOn = DateTime.UtcNow;
                        user.NameIdentifier = register.NameIdentifier;

                        source.Update(user);
                    }
                    else
                    {
                        var provider = new TableMembershipProvider();
                        MembershipCreateStatus status;
                        provider.CreateUser(register.UserName, Guid.NewGuid().ToString(), register.Email, null, null, true, register.NameIdentifier, out status);
                        if (status == MembershipCreateStatus.Success)
                        {
                            log.Log("New user signed up.");
                            return true;
                        }
                        else
                        {
                            log.Log("New user failed to signed up; status: '{0}'".FormatWithCulture(status));
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Contract Invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(null != log);
        }

        /// <summary>
        /// Profiles
        /// </summary>
        /// <returns>Profiles</returns>
        private IEnumerable<Abc.Website.Models.UserPublicProfile> Profiles()
        {
            try
            {
                var core = new UserCore();
                var publicProfiles = core.PublicProfilesFull(Application.Current);

                return (from profile in publicProfiles.Select(p => p.Convert())
                        where !string.IsNullOrWhiteSpace(profile.UserName)
                        orderby profile.PreferedProfile descending
                            , profile.CreatedOn descending
                        select profile).Take(16);
            }
            catch (Exception ex)
            {
                log.Log(ex, EventTypes.Warning, (int)Fault.Unknown);
            }

            return null;
        }
        #endregion
    }
}