// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AuthController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Code.Controllers
{
    using Abc;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;
    using Abc.Website.Security;
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using System.Web.Security;

    /// <summary>
    /// Authentication Controller
    /// </summary>
    public class AuthController : Controller
    {
        #region Members
        /// <summary>
        /// GitHub Authorization Post
        /// </summary>
        private const string GitHubAuthorizationPost = "https://github.com/login/oauth/access_token";

        /// <summary>
        /// GitHub API
        /// </summary>
        private const string GitHubApi = "https://api.github.com/{0}?access_token={1}";

        /// <summary>
        /// Log Core
        /// </summary>
        private static readonly LogCore log = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Index Page (Authentication)
        /// </summary>
        /// <remarks>
        /// GET: /Auth/
        /// </remarks>
        /// <returns></returns>
        public ActionResult Index()
        {
            this.ViewBag.ClientId = ServerConfiguration.GitHubClientId;

            return View();
        }

        /// <summary>
        /// Logout (Authentication)
        /// </summary>
        /// <remarks>
        /// GET: /Logout
        /// </remarks>
        /// <returns></returns>
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// GitHub Auth API
        /// </summary>
        /// <remarks>
        /// GET: /Auth/GitHub
        /// </remarks>
        /// <returns></returns>
        public ActionResult GitHub()
        {
            var code = Request.Params["code"];
            if (!string.IsNullOrWhiteSpace(code))
            {
                try
                {
                    string responseData = null;
                    using (var client = new WebClient())
                    {
                        var nameValuePairs = new NameValueCollection();
                        nameValuePairs.Add("code", code);
                        nameValuePairs.Add("client_secret", ServerConfiguration.GitHubSecret);
                        nameValuePairs.Add("client_id", ServerConfiguration.GitHubClientId);
                        client.Headers.Add("content-type", "application/x-www-form-urlencoded");
                        var response = client.UploadValues(GitHubAuthorizationPost, nameValuePairs);
                        responseData = client.Encoding.GetString(response);
                    }

                    if (!string.IsNullOrWhiteSpace(responseData))
                    {
                        var gitHubResponse = GitHubResponse.Parse(responseData);
                        string profileJson = null;
                        using (var client = new WebClient())
                        {
                            profileJson = client.DownloadString(string.Format(GitHubApi, "user", gitHubResponse.AccessToken));
                        }

                        if (!string.IsNullOrWhiteSpace(profileJson))
                        {
                            var serializer = new JavaScriptSerializer();

                            var profile = serializer.Deserialize<GitHubProfile>(profileJson);
                            if (this.Login(profile)) //New User
                            {
                                var source = new DomainSource();
                                var user = source.GetUserByNameIdentifier(Application.Default.Identifier, string.Format("github{0}", profile.Id));
                                var preference = profile.Convert();
                                preference.Application = Application.Default;
                                preference.User = user.Convert();
                                var core = new UserCore();
                                core.Save(preference);
                                var profilePage = ((IConvert<ProfilePage>)profile).Convert();
                                profilePage.ApplicationIdentifier = Application.Default.Identifier;
                                profilePage.OwnerIdentifier = user.Id;
                                profilePage.GitCode = code;
                                profilePage.GitAccessToken = gitHubResponse.AccessToken;
                                core.Save(profilePage);
                                return this.RedirectToAction("Welcome", "Home");
                            }
                            else
                            {
                                return this.RedirectToAction("Index", "Profile", new { username = profile.Login });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Log(ex, Abc.Services.Contracts.EventTypes.Critical, (int)ServiceFault.Unknown);
                }
            }

            return this.RedirectToAction("Index", "Auth");
        }

        private bool Login(GitHubProfile profile)
        {
            var newUser = false;
            var register = new RegisterModel()
            {
                Email = profile.Email,
                NameIdentifier = string.Format("github{0}", profile.Id),
                UserName = profile.Name,
            };

            var source = new DomainSource();
            UserData user = null;
            if (!string.IsNullOrWhiteSpace(register.NameIdentifier) && null != (user = source.GetUserByNameIdentifier(Application.Default.Identifier, register.NameIdentifier)))
            {
                user.LastLoggedInOn = DateTime.UtcNow;
                user.LastActivityOn = DateTime.UtcNow;

                source.Update(user);
            }
            else if (!string.IsNullOrWhiteSpace(register.Email) && null != (user = source.GetUserByEmail(Application.Default.Identifier, register.Email)))
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
                    newUser = true;
                }
                else
                {
                    log.Log(string.Format("New user failed to signed up; status: '{0}'", status));
                }
            }

            FormsAuthentication.SetAuthCookie(register.Email, true);

            return newUser;
        }
        #endregion
    }
}