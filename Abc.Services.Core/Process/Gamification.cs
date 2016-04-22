// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Gamification.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Process
{
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Game;
    using System;

    /// <summary>
    /// Gamification
    /// </summary>
    public class Gamification : ScheduledManager
    {
        #region Members
        /// <summary>
        /// User Core
        /// </summary>
        private readonly IUserCore userCore;

        /// <summary>
        /// Profile
        /// </summary>
        private readonly IProfile profile;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructors
        /// </summary>
        public Gamification()
            : this(new UserCore(), new Profile())
        {
        }

        /// <summary>
        /// Parameter Constructor
        /// </summary>
        /// <param name="userCore">User Core</param>
        /// <param name="profile">Profile</param>
        public Gamification(IUserCore userCore, IProfile profile)
            : base(30, (int)TimeSpan.FromHours(12).TotalSeconds)
        {
            if (null == userCore)
            {
                throw new ArgumentNullException("userCore");
            }
            else if (null == profile)
            {
                throw new ArgumentNullException("profile");
            }
            else
            {
                this.userCore = userCore;
                this.profile = profile;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute Gamification Routine
        /// </summary>
        public override void Execute()
        {
            using (new PerformanceMonitor())
            {
                var application = Application.Default;
                int value;
                var profiles = userCore.PublicProfilesFull(application, true);
                foreach (var profile in profiles)
                {
                    try
                    {
                        value = this.profile.Evaluate(profile);

                        var save = ((IConvert<ProfilePage>)profile).Convert();
                        save.Points = value;
                        save.ApplicationIdentifier = application.Identifier;

                        this.userCore.Save(save);
                    }
                    catch (Exception ex)
                    {
                        log.Log(ex, EventTypes.Critical, (int)ServiceFault.Unknown);
                    }
                }
            }
        }
        #endregion
    }
}