// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPublicProfile.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using Abc.Services.Contracts;
    using System;

    /// <summary>
    /// User Public Profile
    /// </summary>
    public class UserPublicProfile : IConvert<ProfilePage>
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
        /// Gets or sets Handle
        /// </summary>
        public string Handle
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
        public int Points
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Owner Identifier
        /// </summary>
        public Guid OwnerIdentifier
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Profile Page</returns>
        ProfilePage IConvert<ProfilePage>.Convert()
        {
            return new ProfilePage()
            {
                Handle = this.Handle,
                PreferedProfile = this.PreferedProfile,
                Points = this.Points,
                Word = this.Word,
                OwnerIdentifier = this.OwnerIdentifier,
            };
        }
        #endregion
    }
}