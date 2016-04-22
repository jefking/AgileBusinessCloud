// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Interfaces.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using System.Collections.Generic;

    /// <summary>
    /// User Core Interface for Mocking
    /// </summary>
    public interface IUserCore
    {
        #region Methods
        /// <summary>
        /// Get Profile Page
        /// </summary>
        /// <param name="application">Page</param>
        /// <returns>Profile Pages</returns>
        IEnumerable<ProfilePage> PublicProfiles(Application application);

        /// <summary>
        /// Public Profiles Full
        /// </summary>
        /// <param name="application">Application</param>
        /// <returns>Full Public Profiles</returns>
        IEnumerable<UserPublicProfile> PublicProfilesFull(Application application, bool withPreferences = true);

        /// <summary>
        /// Save Profile Page
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>Profile Page</returns>
        ProfilePage Save(ProfilePage page);
        #endregion
    }
}