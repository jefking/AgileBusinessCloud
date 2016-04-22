// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPublicProfile.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using LinqToTwitter;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// User Public Profile
    /// </summary>
    public class UserPublicProfile
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
        /// Gets or sets Tweets
        /// </summary>
        public IEnumerable<Status> Tweets
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
        /// Applications Used
        /// </summary>
        public int ApplicationsUsed
        {
            get;
            set;
        }

        /// <summary>
        /// Applications Maximum
        /// </summary>
        public int ApplicationsMaximum
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
        /// Gets or sets Time Zone
        /// </summary>
        public TimeZoneInfo TimeZone
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
        /// Gets or sets Prefered Profile
        /// </summary>
        public bool PreferedProfile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Points
        /// </summary>
        public int Points
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
        #endregion
    }
}