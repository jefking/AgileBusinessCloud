// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Interfaces.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using LinqToTwitter;
    using System.Linq;

    /// <summary>
    /// Twitter Source Interface
    /// </summary>
    public interface ITwitterSource
    {
        #region Methods
        /// <summary>
        /// Tweets By User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="max">Maximum</param>
        /// <returns>Tweets</returns>
        IQueryable<Status> ByUser(string user, int max);
        #endregion
    }
}