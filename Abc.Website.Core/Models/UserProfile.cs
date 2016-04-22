// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserProfile.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using LinqToTwitter;

    /// <summary>
    /// User Profile Information
    /// </summary>
    public class UserProfile
    {
        #region Properties
        /// <summary>
        /// Gets or sets Gravatar
        /// </summary>
        public string Gravatar
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Word
        /// </summary>
        public string Word
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets User Name
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Twitter Handle
        /// </summary>
        public string TwitterHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Git Hub Handle
        /// </summary>
        public string GitHubHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ABC Handle
        /// </summary>
        public string AbcHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets City
        /// </summary>
        public string City
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Country
        /// </summary>
        public string Country
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Current Application Identifier
        /// </summary>
        public Guid? CurrentApplicationIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Time Zone
        /// </summary>
        public TimeZoneInfo TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Maxium Allowed Applications
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelled Wrong, but is now in production")]
        public int? MaximumAllowedApplications
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Name CreatedOn
        /// </summary>
        public DateTime CreatedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Time Zones
        /// </summary>
        public IEnumerable<SelectListItem> TimeZones
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Tweets
        /// </summary>
        public IEnumerable<Status> Tweets
        {
            get;
            set;
        }

        public int GitId
        {
            get;
            set;
        }

        public string GitAvatarUrl
        {
            get;
            set;
        }

        public string GitGravatarId
        {
            get;
            set;
        }

        public string GitUrl
        {
            get;
            set;
        }

        public string GitBlog
        {
            get;
            set;
        }

        public bool GitHireable
        {
            get;
            set;
        }

        public string GitBiography
        {
            get;
            set;
        }

        public int GitPublicGists
        {
            get;
            set;
        }

        public int GitPublicRepos
        {
            get;
            set;
        }

        public int GitFollowers
        {
            get;
            set;
        }

        public int GitFollowing
        {
            get;
            set;
        }

        public string GitHtmlUrl
        {
            get;
            set;
        }

        public DateTime GitCreatedAt
        {
            get;
            set;
        }

        public string GitType
        {
            get;
            set;
        }

        public string GitAccessToken
        {
            get;
            set;
        }

        public string GitCode
        {
            get;
            set;
        }
        #endregion
    }
}
