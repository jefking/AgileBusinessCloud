// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPreferenceTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserPreferenceTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertNullApplication()
        {
            var up = new UserPreference()
            {
                User = new User(),
            };

            up.Convert();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertNullUser()
        {
            var up = new UserPreference()
            {
                Application = new Application(),
            };

            up.Convert();
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new UserPreference();
        }

        [TestMethod]
        public void User()
        {
            var user = new User();
            var up = new UserPreference()
            {
                User = user,
            };

            Assert.AreEqual<User>(user, up.User);
        }

        [TestMethod]
        public void CanCreateApplication()
        {
            var up = new UserPreference();
            Assert.IsFalse(up.CanCreateApplication);
            up.CanCreateApplication = true;
            Assert.IsTrue(up.CanCreateApplication);
        }

        [TestMethod]
        public void Application()
        {
            var app = new Application();
            var up = new UserPreference()
            {
                Application = app,
            };

            Assert.AreEqual<Application>(app, up.Application);
        }

        [TestMethod]
        public void UTCOffset()
        {
            var data = DateTimeOffset.Now;
            var up = new UserPreference()
            {
                UTCOffset = data,
            };

            Assert.AreEqual<DateTimeOffset?>(data, up.UTCOffset);
        }

        [TestMethod]
        public void TwitterHandle()
        {
            var data = StringHelper.ValidString();
            var up = new UserPreference()
            {
                TwitterHandle = data,
            };

            Assert.AreEqual<string>(data, up.TwitterHandle);
        }

        [TestMethod]
        public void GitHubHandle()
        {
            var data = StringHelper.ValidString();
            var up = new UserPreference()
            {
                GitHubHandle = data,
            };

            Assert.AreEqual<string>(data, up.GitHubHandle);
        }

        [TestMethod]
        public void AbcHandle()
        {
            var data = StringHelper.ValidString();
            var up = new UserPreference()
            {
                AbcHandle = data,
            };

            Assert.AreEqual<string>(data, up.AbcHandle);
        }

        [TestMethod]
        public void City()
        {
            var data = StringHelper.ValidString();
            var up = new UserPreference()
            {
                City = data,
            };

            Assert.AreEqual<string>(data, up.City);
        }

        [TestMethod]
        public void Country()
        {
            var data = StringHelper.ValidString();
            var up = new UserPreference()
            {
                Country = data,
            };

            Assert.AreEqual<string>(data, up.Country);
        }

        [TestMethod]
        public void TimeZone()
        {
            var data = TimeZoneInfo.Utc;
            var up = new UserPreference()
            {
                TimeZone = data,
            };

            Assert.AreEqual<TimeZoneInfo>(data, up.TimeZone);
        }

        [TestMethod]
        public void CurrentApplication()
        {
            var app = new Application();
            var up = new UserPreference()
            {
                CurrentApplication = app,
            };

            Assert.AreEqual<Application>(app, up.CurrentApplication);
        }

        [TestMethod]
        public void MaxiumAllowedApplications()
        {
            var random = new Random();
            var upr = new UserPreference();
            Assert.IsNull(upr.MaximumAllowedApplications);
            var data = random.Next();
            upr.MaximumAllowedApplications = data;
            Assert.AreEqual<int?>(data, upr.MaximumAllowedApplications);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var cur = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var up = new UserPreference()
            {
                Application = app,
                User = user,
                CurrentApplication = cur,
                MaximumAllowedApplications = random.Next(),
                TimeZone = TimeZoneInfo.Utc,
                TwitterHandle = StringHelper.ValidString(),
                AbcHandle = StringHelper.ValidString(),
                GitHubHandle = StringHelper.ValidString(),
                City = StringHelper.ValidString(),
                Country = StringHelper.ValidString(),
            };

            var converted = up.Convert();
            Assert.AreEqual<Guid>(up.Application.Identifier, converted.ApplicationIdentifier);
            Assert.AreEqual<Guid>(up.CurrentApplication.Identifier, converted.CurrentApplicationIdentifier.Value);
            Assert.AreEqual<Guid>(up.User.Identifier, converted.UserIdentifier);
            Assert.AreEqual<string>(TimeZoneInfo.Utc.ToSerializedString(), converted.TimeZone);
            Assert.AreEqual<string>(up.TwitterHandle, converted.TwitterHandle);
            Assert.AreEqual<string>(up.AbcHandle, converted.AbcHandle);
            Assert.AreEqual<string>(up.GitHubHandle, converted.GitHubHandle);
            Assert.AreEqual<string>(up.City, converted.City);
            Assert.AreEqual<string>(up.Country, converted.Country);
            Assert.AreEqual<int?>(up.MaximumAllowedApplications, converted.MaxiumAllowedApplications);
        }

        [TestMethod]
        public void ConvertNullProperties()
        {
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var up = new UserPreference()
            {
                Application = app,
                User = user,
            };

            var converted = up.Convert();
            Assert.AreEqual<Guid>(up.Application.Identifier, converted.ApplicationIdentifier);
            Assert.AreEqual<Guid>(up.User.Identifier, converted.UserIdentifier);
            Assert.IsNull(converted.CurrentApplicationIdentifier);
            Assert.IsNull(converted.MaxiumAllowedApplications);
            Assert.IsNull(converted.TwitterHandle);
            Assert.IsNull(converted.GitHubHandle);
            Assert.IsNull(converted.AbcHandle);
            Assert.IsNull(converted.City);
            Assert.IsNull(converted.Country);
        }
        #endregion
    }
}