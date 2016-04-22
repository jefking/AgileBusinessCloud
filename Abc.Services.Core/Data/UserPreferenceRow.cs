// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPreferenceRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// User Preference Row
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("ApplicationUserPreference")]
    public class UserPreferenceRow : TableServiceEntity, IConvert<UserPreference>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UserPreferenceRow class
        /// </summary>
        public UserPreferenceRow()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserPreferenceRow class
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        /// <param name="userIdentifier">User Identifier</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public UserPreferenceRow(Guid applicationIdentifier, Guid userIdentifier)
            : base(applicationIdentifier.ToString(), userIdentifier.ToString())
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationIdentifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userIdentifier);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets UserIdentifier
        /// </summary>
        public Guid UserIdentifier
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.RowKey) ? Guid.Empty : Guid.Parse(this.RowKey);
            }
        }

        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public Guid ApplicationIdentifier
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.RowKey) ? Guid.Empty : Guid.Parse(this.PartitionKey);
            }
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
        /// Gets or sets UTC Offset
        /// </summary>
        [Obsolete]
        public string UTCOffset
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
        /// Gets or sets Maxium Allowed Applications
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelled Wrong, but is now in production")]
        public int? MaxiumAllowedApplications
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets UTC Offset
        /// </summary>
        public string TimeZone
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to User Preference
        /// </summary>
        /// <returns>User Preference</returns>
        public UserPreference Convert()
        {
            var user = new User()
            {
                Identifier = this.UserIdentifier,
            };
            var application = new Application()
            {
                Identifier = this.ApplicationIdentifier,
            };
            var currentApplication = new Application()
            {
                Identifier = null == this.CurrentApplicationIdentifier ? Guid.Empty : this.CurrentApplicationIdentifier.Value,
            };

            var timeZone = string.IsNullOrWhiteSpace(this.TimeZone) ? TimeZoneInfo.Utc : TimeZoneInfo.FromSerializedString(this.TimeZone);

            return new UserPreference()
            {
                User = user,
                Application = application,
                CurrentApplication = currentApplication,
                MaximumAllowedApplications = this.MaxiumAllowedApplications ?? 0,
                TimeZone = timeZone,
                TwitterHandle = this.TwitterHandle,
                GitHubHandle = this.GitHubHandle,
                AbcHandle = this.AbcHandle,
                Country = this.Country,
                City = this.City,
            };
        }
        #endregion
    }
}