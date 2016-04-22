// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserProfileRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// User Profile Row
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("UserProfile")]
    public class UserProfileRow : TableServiceEntity, IApplicationIdentifier, IConvert<ProfilePage>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UserProfileRow class
        /// </summary>
        public UserProfileRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserProfileRow class
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        /// <param name="userIdentifier">User Identifier</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public UserProfileRow(Guid applicationIdentifier, string handle)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationIdentifier);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(handle));

            this.PartitionKey = applicationIdentifier.ToString();
            this.RowKey = handle.ToLowerInvariant();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public Guid ApplicationIdentifier
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.PartitionKey) ? Guid.Empty : Guid.Parse(this.PartitionKey);
            }
        }

        /// <summary>
        /// Gets or sets the Owner Identifier
        /// </summary>
        public Guid OwnerIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Prefered Profile
        /// </summary>
        public bool? PreferedProfile
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
        /// Gets or sets Points
        /// </summary>
        public int? Points
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Git Id
        /// </summary>
        public int? GitId
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

        public bool? GitHireable
        {
            get;
            set;
        }

        public string GitBiography
        {
            get;
            set;
        }

        public int? GitPublicGists
        {
            get;
            set;
        }
        
        public int? GitPublicRepos
        {
            get;
            set;
        }

        public int? GitFollowers
        {
            get;
            set;
        }
        
        public int? GitFollowing
        {
            get;
            set;
        }

        public string GitHtmlUrl
        {
            get;
            set;
        }

        public DateTime? GitCreatedAt
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

        #region Methods
        /// <summary>
        /// Convert to Profile Page
        /// </summary>
        /// <returns>Profile Page</returns>
        public ProfilePage Convert()
        {
            return new ProfilePage()
            {
                ApplicationIdentifier = this.ApplicationIdentifier,
                Handle = this.RowKey,
                OwnerIdentifier = this.OwnerIdentifier,
                PreferedProfile = this.PreferedProfile.HasValue && this.PreferedProfile.Value,
                Points = this.Points.HasValue ? this.Points.Value : 0,
                Word = this.Word,
                GitAccessToken = this.GitAccessToken,
                GitCode = this.GitCode,
            };
        }
        #endregion
    }
}