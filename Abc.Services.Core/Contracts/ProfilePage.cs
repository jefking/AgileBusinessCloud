// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ProfilePage.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Abc.Services.Data;

    /// <summary>
    /// Profile Page
    /// </summary>
    [DataContract]
    public class ProfilePage : IApplicationIdentifier, IConvert<UserProfileRow>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Application Identifier
        /// </summary>
        [DataMember]
        public Guid ApplicationIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Owner Identifier
        /// </summary>
        [DataMember]
        public Guid OwnerIdentifier
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
        /// Gets or sets the Handle
        /// </summary>
        [DataMember]
        public string Handle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Existing Handle
        /// </summary>
        [IgnoreDataMember]
        public string ExistingHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Word
        /// </summary>
        [DataMember]
        public string Word
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Points
        /// </summary>
        [DataMember]
        public int Points
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Git Id
        /// </summary>
        [DataMember]
        public int GitId
        {
            get;
            set;
        }

        [DataMember]
        public string GitAvatarUrl
        {
            get;
            set;
        }

        [DataMember]
        public string GitGravatarId
        {
            get;
            set;
        }

        [DataMember]
        public string GitUrl
        {
            get;
            set;
        }

        [DataMember]
        public string GitBlog
        {
            get;
            set;
        }

        [DataMember]
        public bool GitHireable
        {
            get;
            set;
        }

        [DataMember]
        public string GitBiography
        {
            get;
            set;
        }

        [DataMember]
        public int GitPublicGists
        {
            get;
            set;
        }

        [DataMember]
        public int GitPublicRepos
        {
            get;
            set;
        }

        [DataMember]
        public int GitFollowers
        {
            get;
            set;
        }

        [DataMember]
        public int GitFollowing
        {
            get;
            set;
        }

        [DataMember]
        public string GitHtmlUrl
        {
            get;
            set;
        }

        [DataMember]
        public DateTime GitCreatedAt
        {
            get;
            set;
        }

        [DataMember]
        public string GitType
        {
            get;
            set;
        }

        [DataMember]
        public string GitAccessToken
        {
            get;
            set;
        }

        [DataMember]
        public string GitCode
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to User Profile Row
        /// </summary>
        /// <returns>User Profile Row</returns>
        [CLSCompliant(false)]
        public UserProfileRow Convert()
        {
            return new UserProfileRow(this.ApplicationIdentifier, this.Handle)
            {
                OwnerIdentifier = this.OwnerIdentifier,
                PreferedProfile = this.PreferedProfile,
                Word = this.Word,
                Points = this.Points,
                GitType = this.GitType,
                GitCreatedAt = this.GitCreatedAt.Year < 1900 ? null : (DateTime?)this.GitCreatedAt,
                GitHtmlUrl = this.GitHtmlUrl,
                GitFollowers = this.GitFollowers == 0 ? null : (int?)this.GitFollowers,
                GitFollowing = this.GitFollowing == 0 ? null : (int?)this.GitFollowing,
                GitPublicGists = this.GitPublicGists == 0 ? null : (int?)this.GitPublicGists,
                GitPublicRepos = this.GitPublicRepos == 0 ? null : (int?)this.GitPublicRepos,
                GitBiography = this.GitBiography,
                GitBlog = this.GitBlog,
                GitAvatarUrl = this.GitAvatarUrl,
                GitUrl = this.GitUrl,
                GitGravatarId = this.GitGravatarId,
                GitId = this.GitId == 0 ? null : (int?)this.GitId,
                GitAccessToken = this.GitAccessToken,
                GitCode = this.GitCode,
            };
        }
        #endregion
    }
}