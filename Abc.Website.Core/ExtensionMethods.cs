// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethods.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;
    using Microsoft.IdentityModel.Claims;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Principal;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region Members
        /// <summary>
        /// User Core
        /// </summary>
        private static readonly UserCore userCore = new UserCore();
        #endregion

        #region System.Security.Principal.IIdentity
        /// <summary>
        /// Get Identities Email Address
        /// </summary>
        /// <param name="identity">Identity</param>
        /// <returns>Email Address</returns>
        public static string EmailAddress(this IIdentity identity)
        {
            var claimIdentity = identity as IClaimsIdentity;

            return identity == null ? null : claimIdentity.ClaimValue(ClaimTypes.Email);
        }

        /// <summary>
        /// Get Identities Email Address
        /// </summary>
        /// <param name="identity">Identity</param>
        /// <returns>Email Address</returns>
        public static string NameIdentifier(this IIdentity identity)
        {
            var claimIdentity = identity as IClaimsIdentity;

            return identity == null ? null : claimIdentity.ClaimValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Claim Value
        /// </summary>
        /// <param name="identity">Identity</param>
        /// <param name="claimType">Claim Type</param>
        /// <returns>Value of specified Claim</returns>
        public static string ClaimValue(this IClaimsIdentity identity, string claimType)
        {
            return identity.Claims == null ? null : (from data in identity.Claims
                                                     where data.ClaimType.ToUpperInvariant() == claimType.ToUpperInvariant()
                                                     select data.Value).FirstOrDefault();
        }

        /// <summary>
        /// Is Manager Loaded in current identity
        /// </summary>
        /// <param name="identity">Identity</param>
        /// <returns>Is Manager</returns>
        public static bool IsManager(this IIdentity identity)
        {
            Contract.Requires(null != identity);

            var claimIdentity = identity as IClaimsIdentity;

            return null != claimIdentity && claimIdentity.Claims.Exists(c => c.ClaimType.ToUpperInvariant() == ClaimTypes.Role.ToUpperInvariant() && c.Value.ToUpperInvariant() == "staff".ToUpperInvariant());
        }

        /// <summary>
        /// Is Manager Loaded in current identity
        /// </summary>
        /// <param name="identity">Identity</param>
        /// <returns>User Data</returns>
        [CLSCompliant(false)]
        public static User Data(this IIdentity identity)
        {
            Contract.Requires(null != identity);

            if (identity.IsAuthenticated)
            {
                var user = new User()
                {
                    NameIdentifier = identity.NameIdentifier()
                };
                var userApp = new UserApplication()
                {
                    Application = Application.Current,
                    User = user,
                };

                return userCore.GetByNameIdentifier(userApp);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Editor
        /// </summary>
        /// <param name="identity">Identity</param>
        /// <returns>User Application</returns>
        public static UserApplication Editor(this IIdentity identity)
        {
            Contract.Requires(null != identity);

            if (identity.IsAuthenticated)
            {
                return new UserApplication()
                {
                    Application = Application.Current,
                    User = identity.Data(),
                };
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Occurrence Data
        /// <summary>
        /// Occurrence Data Execution Time
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Execution Time Span</returns>
        [CLSCompliant(false)]
        public static TimeSpan ExecutionTime(this OccurrenceData data)
        {
            Contract.Requires(null != data);

            return TimeSpan.FromTicks(data.Duration);
        }
        #endregion

        #region Application
        /// <summary>
        /// Convert Information, to Model
        /// </summary>
        /// <param name="information">Application Informations</param>
        /// <param name="user">User</param>
        /// <returns>Application Model</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Calling only with C#")]
        public static IEnumerable<ApplicationDetailsModel> Convert(this IList<ApplicationInformation> information, User user = null)
        {
            Contract.Requires(null != information);

            return information.Select(i => i.Convert(user));
        }

        /// <summary>
        /// Convert Application Information to Application Details Model
        /// </summary>
        /// <param name="appInfo">Application Information</param>
        /// <param name="user">User</param>
        /// <returns>Application Details Model</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Calling only with C#")]
        public static ApplicationDetailsModel Convert(this ApplicationInformation appInfo, User user = null)
        {
            return new ApplicationDetailsModel()
                {
                    ApplicationId = appInfo.Identifier,
                    IsValid = appInfo.IsValid,
                    ValidUntil = appInfo.ValidUntil,
                    Active = appInfo.Active,
                    Deleted = appInfo.Deleted,
                    Description = appInfo.Description,
                    Environment = appInfo.Environment,
                    Name = appInfo.Name,
                    New = appInfo.IsNew,
                    PublicKey = string.IsNullOrWhiteSpace(appInfo.PublicKey) ? appInfo.Identifier.ToAscii85().GetHexMD5() : appInfo.PublicKey,
                    IsOwner = null != user && user.Identifier == appInfo.OwnerId,
                };
        }
        #endregion

        #region Html Helper
        /// <summary>
        /// Display Help For
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TValue">Value</typeparam>
        /// <param name="html">HTML</param>
        /// <param name="expression">Expression</param>
        /// <param name="templateName">Template Type</param>
        /// <returns>MVC Html String</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Generics")]
        public static MvcHtmlString DisplayHelpFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string templateName)
        {
            Contract.Requires(html.ViewData.ModelMetadata.Properties != null);

            var expressionBody = expression.Body as MemberExpression;

            if (null != expressionBody)
            {
                var propertyName = expressionBody.Member.Name;

                return html.DisplayFor(expression, templateName, new { Message = html.ViewData.ModelMetadata.Properties.Single(p => p.PropertyName == propertyName).Description });
            }
            else
            {
                throw new ArgumentException("The supplied expression <{0}> isn't a MemberExpression.".FormatWithCulture(expression));
            }
        }
        #endregion

        #region Url Helper
        /// <summary>
        /// Content cached on CDN
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="resource">content</param>
        /// <returns>Url</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "CDN lowered")]
        public static string ContentCdn(this UrlHelper url, string resource)
        {
            return AzureEnvironment.IsComputeEmulator ? url.Content(resource) : resource.Replace("~", ServerConfiguration.ContentDistributionUrl);
        }

        /// <summary>
        /// Content straight from Blob, to avoid caching
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="resource">content</param>
        /// <returns>Url</returns>
        public static string ContentBlob(this UrlHelper url, string resource)
        {
            return AzureEnvironment.IsComputeEmulator ? url.Content(resource) : resource.Replace("~", ServerConfiguration.BlobUrl);
        }
        #endregion

        #region Abc.Services.Contracts.User
        /// <summary>
        /// Convert User to User public profile
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>User Public Profile</returns>
        public static UserPublicProfile Convert(this Abc.Services.Data.UserPublicProfile user)
        {
            return new UserPublicProfile()
            {
                CreatedOn = user.CreatedOn,
                UserName = user.UserName,
                Gravatar = user.Gravatar,
                AbcHandle = user.Handle,
                PreferedProfile = user.PreferedProfile,
                Points = user.Points,
                Word = user.Word,
            };
        }
        #endregion

        #region Abc.Website.Models.UserPublicProfile
        /// <summary>
        /// Set User Preferences of Public Profile
        /// </summary>
        /// <param name="user">User</param>
        public static void Set(this UserPublicProfile profile, UserPreference preferences)
        {
            profile.AbcHandle = preferences.AbcHandle;
            profile.TwitterHandle = preferences.TwitterHandle;
            profile.TimeZone = preferences.TimeZone;
            profile.GitHubHandle = preferences.GitHubHandle;
            profile.Country = preferences.Country;
            profile.City = preferences.City;
            profile.ApplicationsMaximum = preferences.MaximumAllowedApplications ?? 1;
        }
        #endregion

        #region LinqToTwitterStatus
        /// <summary>
        /// Render Twitter Status
        /// </summary>
        /// <param name="status">Status</param>
        /// <returns>Rendered as HTML</returns>
        [CLSCompliant(false)]
        public static string Render(this LinqToTwitter.Status status)
        {
            var twitter = new Abc.Services.Data.TwitterSource();
            return twitter.RenderHtml(status);
        }
        #endregion

        #region System.DateTime
        /// <summary>
        /// Relative Date Time
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <returns>Relative</returns>
        public static string Relative(this DateTime dateTime)
        {
            var comparedTo = DateTime.UtcNow;
            var diff = comparedTo.Subtract(dateTime);
            if (diff.TotalDays >= 365)
            {
                return string.Concat("on ", dateTime.ToShortDateString());
            }
            if (diff.TotalDays >= 7)
            {
                return string.Concat("on ", dateTime.ToString("MMMM d"));
            }
            else if (diff.TotalDays > 1)
            {
                return string.Format("{0:N0} days ago", diff.TotalDays);
            }
            else if (diff.TotalDays == 1)
            {
                return "yesterday";
            }
            else if (diff.TotalHours >= 2)
            {
                return string.Format("{0:N0} hours ago", diff.TotalHours);
            }
            else if (diff.TotalMinutes >= 60)
            {
                return "more than an hour ago";
            }
            else if (diff.TotalMinutes >= 5)
            {
                return string.Format("{0:N0} minutes ago", diff.TotalMinutes);
            }
            if (diff.TotalMinutes >= 1)
            {
                return "a few minutes ago";
            }
            else
            {
                return "in real-time";
            }
        }
        #endregion
    }
}