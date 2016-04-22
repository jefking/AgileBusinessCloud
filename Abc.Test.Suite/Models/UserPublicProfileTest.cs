// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPublicProfileTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Models
{
    using System;
    using System.Collections.Generic;
    using Abc.Website.Models;
    using LinqToTwitter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserPublicProfileTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new UserPublicProfile();
        }

        [TestMethod]
        public void UserName()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.UserName = data;
            Assert.AreEqual<string>(data, userRole.UserName);
        }

        [TestMethod]
        public void GitHubHandle()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.GitHubHandle = data;
            Assert.AreEqual<string>(data, userRole.GitHubHandle);
        }

        [TestMethod]
        public void AbcHandle()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.AbcHandle = data;
            Assert.AreEqual<string>(data, userRole.AbcHandle);
        }

        [TestMethod]
        public void City()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.City = data;
            Assert.AreEqual<string>(data, userRole.City);
        }

        [TestMethod]
        public void Country()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.Country = data;
            Assert.AreEqual<string>(data, userRole.Country);
        }

        [TestMethod]
        public void TimeZone()
        {
            var userRole = new UserPublicProfile();
            var data = TimeZoneInfo.Utc;
            userRole.TimeZone = data;
            Assert.AreEqual<TimeZoneInfo>(data, userRole.TimeZone);
        }

        [TestMethod]
        public void Tweets()
        {
            var userRole = new UserPublicProfile();
            var data = new List<Status>();
            userRole.Tweets = data;
            Assert.AreEqual<IEnumerable<Status>>(data, userRole.Tweets);
        }

        [TestMethod]
        public void Gravatar()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.Gravatar = data;
            Assert.AreEqual<string>(data, userRole.Gravatar);
        }

        [TestMethod]
        public void PreferedProfile()
        {
            var userRole = new UserPublicProfile();
            Assert.IsFalse(userRole.PreferedProfile);
            userRole.PreferedProfile = true;
            Assert.IsTrue(userRole.PreferedProfile);
        }

        [TestMethod]
        public void TwitterHandle()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.TwitterHandle = data;
            Assert.AreEqual<string>(data, userRole.TwitterHandle);
        }

        [TestMethod]
        public void ApplicationsUsed()
        {
            var random = new Random();
            var userRole = new UserPublicProfile();
            var data = random.Next();
            userRole.ApplicationsUsed = data;
            Assert.AreEqual<int>(data, userRole.ApplicationsUsed);
        }

        [TestMethod]
        public void ApplicationsMaximum()
        {
            var random = new Random();
            var userRole = new UserPublicProfile();
            var data = random.Next();
            userRole.ApplicationsMaximum = data;
            Assert.AreEqual<int?>(data, userRole.ApplicationsMaximum);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var userRole = new UserPublicProfile();
            var data = DateTime.UtcNow;
            userRole.CreatedOn = data;
            Assert.AreEqual<DateTime>(data, userRole.CreatedOn);
        }
        #endregion
    }
}