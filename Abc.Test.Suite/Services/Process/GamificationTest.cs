// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='GamificationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Process
{
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Abc.Services.Game;
    using Abc.Services.Process;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class GamificationTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorUserCoreNull()
        {
            new Gamification(null, new Profile());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorProfileNull()
        {
            new Gamification(new UserCore(), null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Gamification();
        }

        [TestMethod]
        public void ConstructorParams()
        {
            new Gamification(new UserCore(), new Profile());
        }

        [TestMethod]
        public void IsScheduledManager()
        {
            Assert.IsNotNull(new Gamification() as ScheduledManager);
        }

        [TestMethod]
        public void Execute()
        {
            var userCore = new Mock<IUserCore>();
            var profile = new Mock<IProfile>();
            var profiles = new List<UserPublicProfile>();
            var userProfile = new UserPublicProfile()
            {
                PreferedProfile = true,
                GitHubHandle = StringHelper.ValidString(),
            };
            profiles.Add(userProfile);

            userCore.Setup(u => u.PublicProfilesFull(It.IsAny<Application>(), true)).Returns(profiles);
            userCore.Setup(u => u.Save(It.IsAny<ProfilePage>()));
            profile.Setup(p => p.Evaluate(userProfile)).Returns(0);

            var game = new Gamification(userCore.Object, profile.Object);

            game.Execute();

            userCore.Verify(u => u.PublicProfilesFull(It.IsAny<Application>(), true), Times.Once());
            userCore.Verify(u => u.Save(It.IsAny<ProfilePage>()), Times.Once());
            profile.Verify(p => p.Evaluate(userProfile), Times.Once());
        }
        #endregion
    }
}