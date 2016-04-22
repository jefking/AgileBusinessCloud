// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='GitHubProfile.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Security
{
    using Abc.Services.Contracts;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// GitHub Profile
    /// </summary>
    public class GitHubProfile : IConvert<UserPreference>, IConvert<ProfilePage>
    {
        #region Properties
        public string Login
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public string Avatar_Url
        {
            get;
            set;
        }

        public string Gravatar_Id
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Company
        {
            get;
            set;
        }

        public string Blog
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public bool Hireable
        {
            get;
            set;
        }

        public string Bio
        {
            get;
            set;
        }

        public int Public_Gists
        {
            get;
            set;
        }

        public int Public_Repos
        {
            get;
            set;
        }

        public int Followers
        {
            get;
            set;
        }

        public int Following
        {
            get;
            set;
        }

        public string Html_Url
        {
            get;
            set;
        }

        public DateTime Created_At
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public UserPreference Convert()
        {
            return new UserPreference()
            {
                City = this.Location,
                AbcHandle = this.Login,
                GitHubHandle = this.Login,
            };
        }

        ProfilePage IConvert<ProfilePage>.Convert()
        {
            return new ProfilePage()
            {
                Handle = this.Login,
                Word = this.Company,
                GitType = this.Type,
                GitCreatedAt = this.Created_At,
                GitHtmlUrl = this.Html_Url,
                GitFollowers = this.Followers,
                GitFollowing = this.Following,
                GitPublicGists = this.Public_Gists,
                GitPublicRepos = this.Public_Repos,
                GitBiography = this.Bio,
                GitBlog = this.Blog,
                GitAvatarUrl = this.Avatar_Url,
                GitUrl = this.Url,
                GitGravatarId = this.Gravatar_Id,
                GitId = this.Id,
            };
        }
        #endregion
    }
}