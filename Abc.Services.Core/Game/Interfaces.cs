// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Interfaces.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Game
{
    using Abc.Services.Data;

    /// <summary>
    /// Profile Interface
    /// </summary>
    public interface IProfile
    {
        #region Methods
        /// <summary>
        /// Evaluate Game Value
        /// </summary>
        /// <param name="profile">Profile</param>
        /// <returns>Value</returns>
        int Evaluate(UserPublicProfile profile);
        #endregion
    }
}