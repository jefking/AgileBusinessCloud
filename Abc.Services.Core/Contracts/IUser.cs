// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IUser.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    /// <summary>
    /// User Interface
    /// </summary>
    public interface IUser
    {
        #region Properties
        /// <summary>
        /// Gets the User
        /// </summary>
        User User
        {
            get;
        }
        #endregion
    }
}