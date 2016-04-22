// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Profile.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Game
{
    using Abc.Services.Data;

    /// <summary>
    /// Profile
    /// </summary>
    public class Profile : IProfile
    {
        #region Methods
        /// <summary>
        /// Evaluate Game Value
        /// </summary>
        /// <param name="profile">Profile</param>
        /// <returns>Value</returns>
        public int Evaluate(UserPublicProfile profile)
        {
            var value = 0;
            if (null != profile)
            {
                if (profile.PreferedProfile)
                {
                    value += 500;
                }

                if (!string.IsNullOrWhiteSpace(profile.Word))
                {
                    value += 200;
                }

                if (!string.IsNullOrWhiteSpace(profile.GitHubHandle))
                {
                    value += 100;
                }

                if (!string.IsNullOrWhiteSpace(profile.TwitterHandle))
                {
                    value += 100;
                }

                if (!string.IsNullOrWhiteSpace(profile.Handle))
                {
                    value += 100;
                }
            }

            return value;
        }
        #endregion
    }
}