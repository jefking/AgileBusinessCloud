// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TwitterHarvestorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Process
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Abc.Services.Process;
    using LinqToTwitter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class TwitterHarvestorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorUserCoreNull()
        {
            new TwitterHarvestor(null, new JsonContainer<CodeStormSocial>(ServerConfiguration.Default), new TwitterSource());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorContainerNull()
        {
            new TwitterHarvestor(new UserCore(), null, new TwitterSource());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTwitterSourcesNull()
        {
            new TwitterHarvestor(new UserCore(), new JsonContainer<CodeStormSocial>(ServerConfiguration.Default), null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new TwitterHarvestor();
        }

        [TestMethod]
        public void ConstructorParameters()
        {
            new TwitterHarvestor(new UserCore(), new JsonContainer<CodeStormSocial>(ServerConfiguration.Default), new TwitterSource());
        }

        [TestMethod]
        public void IsScheduledManager()
        {
            Assert.IsNotNull(new TwitterHarvestor() as ScheduledManager);
        }

        [TestMethod]
        public void Execute()
        {
            var profile = new UserPublicProfile()
            {
                TwitterHandle = "DudeYa",
            };

            var profiles = new List<UserPublicProfile>();
            profiles.Add(profile);

            var tweets = new Status[] { new Status(), new Status() }.AsQueryable();

            var container = new Mock<ITextContainer<CodeStormSocial>>(MockBehavior.Strict);
            var userCore = new Mock<IUserCore>(MockBehavior.Strict);
            var twitter = new Mock<ITwitterSource>(MockBehavior.Strict);
            userCore.Setup(r => r.PublicProfilesFull(It.IsAny<Application>(), true)).Returns(profiles);
            container.Setup(r => r.Save(It.IsAny<string>(), It.IsAny<CodeStormSocial>()));
            twitter.Setup(t => t.ByUser(profile.TwitterHandle, 5)).Returns(tweets);

            var item = new TwitterHarvestor(userCore.Object, container.Object, twitter.Object);

            item.Execute();

            userCore.Verify(r => r.PublicProfilesFull(It.IsAny<Application>(), true), Times.Once());
            container.Verify(r => r.Save(It.IsAny<string>(), It.IsAny<CodeStormSocial>()), Times.Once());
            twitter.Verify(t => t.ByUser(profile.TwitterHandle, 5), Times.Once());
        }

        [TestMethod]
        public void ExecuteNoHandle()
        {
            var profile = new UserPublicProfile()
            {
                TwitterHandle = StringHelper.NullEmptyWhiteSpace(),
            };

            var profiles = new List<UserPublicProfile>();
            profiles.Add(profile);

            var tweets = new Status[] { new Status(), new Status() }.AsQueryable();

            var container = new Mock<ITextContainer<CodeStormSocial>>(MockBehavior.Strict);
            var userCore = new Mock<IUserCore>(MockBehavior.Strict);
            var twitter = new Mock<ITwitterSource>(MockBehavior.Strict);
            userCore.Setup(r => r.PublicProfilesFull(It.IsAny<Application>(), true)).Returns(profiles);
            container.Setup(r => r.Save(It.IsAny<string>(), It.IsAny<CodeStormSocial>()));
            twitter.Setup(t => t.ByUser(profile.TwitterHandle, 5)).Returns(tweets);

            var item = new TwitterHarvestor(userCore.Object, container.Object, twitter.Object);

            item.Execute();

            userCore.Verify(r => r.PublicProfilesFull(It.IsAny<Application>(), true), Times.Once());
            container.Verify(r => r.Save(It.IsAny<string>(), It.IsAny<CodeStormSocial>()), Times.Once());
            twitter.Verify(t => t.ByUser(profile.TwitterHandle, 5), Times.Never());
        }

        [TestMethod]
        public void ExecuteByUserThrows()
        {
            var profile = new UserPublicProfile()
            {
                TwitterHandle = "DudeYa",
            };

            var profiles = new List<UserPublicProfile>();
            profiles.Add(profile);

            var container = new Mock<ITextContainer<CodeStormSocial>>(MockBehavior.Strict);
            var userCore = new Mock<IUserCore>(MockBehavior.Strict);
            var twitter = new Mock<ITwitterSource>(MockBehavior.Strict);
            userCore.Setup(r => r.PublicProfilesFull(It.IsAny<Application>(), true)).Returns(profiles);
            container.Setup(r => r.Save(It.IsAny<string>(), It.IsAny<CodeStormSocial>()));
            twitter.Setup(t => t.ByUser(profile.TwitterHandle, 5)).Throws<Exception>();

            var item = new TwitterHarvestor(userCore.Object, container.Object, twitter.Object);

            item.Execute();

            userCore.Verify(r => r.PublicProfilesFull(It.IsAny<Application>(), true), Times.Once());
            container.Verify(r => r.Save(It.IsAny<string>(), It.IsAny<CodeStormSocial>()), Times.Once());
            twitter.Verify(t => t.ByUser(profile.TwitterHandle, 5), Times.Once());
        }
        #endregion
    }
}