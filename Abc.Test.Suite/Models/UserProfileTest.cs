// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserProfileTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using LinqToTwitter;

    [TestClass]
    public class UserProfileTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new UserProfile();
        }

        [TestMethod]
        public void UserName()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.UserName = data;
            Assert.AreEqual<string>(data, userRole.UserName);
        }

        [TestMethod]
        public void GitHubHandle()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.GitHubHandle = data;
            Assert.AreEqual<string>(data, userRole.GitHubHandle);
        }

        [TestMethod]
        public void AbcHandle()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.AbcHandle = data;
            Assert.AreEqual<string>(data, userRole.AbcHandle);
        }

        [TestMethod]
        public void Email()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.Email = data;
            Assert.AreEqual<string>(data, userRole.Email);
        }

        [TestMethod]
        public void City()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.City = data;
            Assert.AreEqual<string>(data, userRole.City);
        }

        [TestMethod]
        public void Country()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.Country = data;
            Assert.AreEqual<string>(data, userRole.Country);
        }

        [TestMethod]
        public void TimeZone()
        {
            var userRole = new UserProfile();
            var data = TimeZoneInfo.Utc;
            userRole.TimeZone = data;
            Assert.AreEqual<TimeZoneInfo>(data, userRole.TimeZone);
        }

        [TestMethod]
        public void Tweets()
        {
            var userRole = new UserProfile();
            var data = new List<Status>();
            userRole.Tweets = data;
            Assert.AreEqual<IEnumerable<Status>>(data, userRole.Tweets);
        }

        [TestMethod]
        public void TimeZones()
        {
            var userRole = new UserProfile();
            var data = TimeZoneInfo.GetSystemTimeZones().ToList().Select(tz => new SelectListItem()
            {
                Text = tz.DisplayName
            });
            userRole.TimeZones = data;
            Assert.AreEqual<IEnumerable<SelectListItem>>(data, userRole.TimeZones);
        }

        [TestMethod]
        public void Gravatar()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.Gravatar = data;
            Assert.AreEqual<string>(data, userRole.Gravatar);
        }

        [TestMethod]
        public void TwitterHandle()
        {
            var userRole = new UserProfile();
            var data = StringHelper.ValidString();
            userRole.TwitterHandle = data;
            Assert.AreEqual<string>(data, userRole.TwitterHandle);
        }

        [TestMethod]
        public void CurrentApplicationIdentifier()
        {
            var userRole = new UserProfile();
            Assert.IsNull(userRole.CurrentApplicationIdentifier);
            var data = Guid.NewGuid();
            userRole.CurrentApplicationIdentifier = data;
            Assert.AreEqual<Guid?>(data, userRole.CurrentApplicationIdentifier);
        }

        [TestMethod]
        public void MaximumAllowedApplications()
        {
            var random = new Random();
            var userRole = new UserProfile();
            Assert.IsNull(userRole.MaximumAllowedApplications);
            var data = random.Next();
            userRole.MaximumAllowedApplications = data;
            Assert.AreEqual<int?>(data, userRole.MaximumAllowedApplications);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var userRole = new UserProfile();
            var data = DateTime.UtcNow;
            userRole.CreatedOn = data;
            Assert.AreEqual<DateTime>(data, userRole.CreatedOn);
        }
        #endregion
    }
}