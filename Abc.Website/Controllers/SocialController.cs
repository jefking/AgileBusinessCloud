// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='SocialController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Social Controller
    /// </summary>
    public abstract class SocialController : Controller
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        protected static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Profiles
        /// </summary>
        /// <returns>Profiles</returns>
        protected IEnumerable<UserPublicProfile> Profiles(int take = 16)
        {
            try
            {
                var core = new UserCore();
                var publicProfiles = core.PublicProfilesFull(Application.Current);

                return (from profile in publicProfiles.Select(p => p.Convert())
                        where !string.IsNullOrWhiteSpace(profile.UserName)
                        orderby profile.PreferedProfile descending
                            , profile.CreatedOn descending
                        select profile).Take(take);
            }
            catch (Exception ex)
            {
                logger.Log(ex, EventTypes.Warning, (int)Fault.Unknown);
            }

            return null;
        }
        #endregion
    }
}
