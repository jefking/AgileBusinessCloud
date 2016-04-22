// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPreference.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Abc.Services.Data;

    /// <summary>
    /// User Preference
    /// </summary>
    [DataContract]
    public class UserPreference : IConvert<UserPreferenceRow>, IUser, IApplication
    {
        #region Properties
        /// <summary>
        /// Gets or sets User
        /// </summary>
        [DataMember]
        public User User
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Twitter Handle
        /// </summary>
        [DataMember]
        public string TwitterHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Git Hub Handle
        /// </summary>
        [DataMember]
        public string GitHubHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ABC Handle
        /// </summary>
        [DataMember]
        public string AbcHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets City
        /// </summary>
        [DataMember]
        public string City
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Country
        /// </summary>
        [DataMember]
        public string Country
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Application
        /// </summary>
        [DataMember]
        public Application Application
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets CurrentApplication
        /// </summary>
        [DataMember]
        public Application CurrentApplication
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Maximum Allowed Applications
        /// </summary>
        public int? MaximumAllowedApplications
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Can Create Application
        /// </summary>
        public bool CanCreateApplication
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets UTC Offset
        /// </summary>
        [Obsolete]
        public DateTimeOffset? UTCOffset
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
        #endregion

        #region Methods
        /// <summary>
        /// Convert to User Preference Row
        /// </summary>
        /// <returns>User Preference</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Information for logging")]
        [CLSCompliant(false)]
        public UserPreferenceRow Convert()
        {
            if (null == this.Application)
            {
                throw new ArgumentNullException();
            }
            else if (null == this.User)
            {
                throw new ArgumentNullException();
            }
            else
            {
                return new UserPreferenceRow(this.Application.Identifier, this.User.Identifier)
                {
                    CurrentApplicationIdentifier = this.CurrentApplication == null || Guid.Empty == this.CurrentApplication.Identifier ? null : (Guid?)this.CurrentApplication.Identifier,
                    MaxiumAllowedApplications = this.MaximumAllowedApplications,
                    TimeZone = this.TimeZone == null ? null : this.TimeZone.ToSerializedString(),
                    TwitterHandle = this.TwitterHandle,
                    City = this.City,
                    Country = this.Country,
                    AbcHandle = this.AbcHandle,
                    GitHubHandle = this.GitHubHandle,
                };
            }
        }
        #endregion
    }
}