// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserPreferenceRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserPreferenceRowTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyApplicationId()
        {
            new UserPreferenceRow(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyUserId()
        {
            new UserPreferenceRow(Guid.NewGuid(), Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new UserPreferenceRow();
        }

        [TestMethod]
        public void ConstructorAppIdUserId()
        {
            new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
        }

        [TestMethod]
        public void UserIdentifier()
        {
            var id = Guid.NewGuid();
            var upr = new UserPreferenceRow(Guid.NewGuid(), id);
            Assert.AreEqual<Guid>(id, upr.UserIdentifier);
        }

        [TestMethod]
        public void UTCOffset()
        {
            var data = StringHelper.ValidString();
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            upr.UTCOffset = data;
            Assert.AreEqual<string>(data, upr.UTCOffset);
        }

        [TestMethod]
        public void TwitterHandle()
        {
            var data = StringHelper.ValidString();
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            upr.TwitterHandle = data;
            Assert.AreEqual<string>(data, upr.TwitterHandle);
        }

        [TestMethod]
        public void GitHubHandle()
        {
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            var data = StringHelper.ValidString();
            upr.GitHubHandle = data;
            Assert.AreEqual<string>(data, upr.GitHubHandle);
        }

        [TestMethod]
        public void AbcHandle()
        {
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            var data = StringHelper.ValidString();
            upr.AbcHandle = data;
            Assert.AreEqual<string>(data, upr.AbcHandle);
        }

        [TestMethod]
        public void City()
        {
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            var data = StringHelper.ValidString();
            upr.City = data;
            Assert.AreEqual<string>(data, upr.City);
        }

        [TestMethod]
        public void Country()
        {
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            var data = StringHelper.ValidString();
            upr.Country = data;
            Assert.AreEqual<string>(data, upr.Country);
        }

        [TestMethod]
        public void TimeZone()
        {
            var data = StringHelper.ValidString();
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            upr.TimeZone = data;
            Assert.AreEqual<string>(data, upr.TimeZone);
        }

        [TestMethod]
        public void ApplicationIdentifier()
        {
            var id = Guid.NewGuid();
            var upr = new UserPreferenceRow(id, Guid.NewGuid());
            Assert.AreEqual<Guid>(id, upr.ApplicationIdentifier);
        }

        [TestMethod]
        public void MaxiumAllowedApplications()
        {
            var random = new Random();
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsNull(upr.MaxiumAllowedApplications);
            var data = random.Next();
            upr.MaxiumAllowedApplications = data;
            Assert.AreEqual<int?>(data, upr.MaxiumAllowedApplications);
        }

        [TestMethod]
        public void CurrentApplicationIdentifier()
        {
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsNull(upr.CurrentApplicationIdentifier);
            var id = Guid.NewGuid();
            upr.CurrentApplicationIdentifier = id;
            Assert.AreEqual<Guid>(id, upr.CurrentApplicationIdentifier.Value);
        }

        [TestMethod]
        public void Convert()
        {
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid())
            {
                CurrentApplicationIdentifier = Guid.NewGuid(),
                TwitterHandle = StringHelper.ValidString(),
                AbcHandle = StringHelper.ValidString(),
                City = StringHelper.ValidString(),
                GitHubHandle = StringHelper.ValidString(),
                Country = StringHelper.ValidString(),
            };

            var converted = upr.Convert();
            Assert.AreEqual<Guid>(upr.ApplicationIdentifier, converted.Application.Identifier);
            Assert.AreEqual<Guid>(upr.UserIdentifier, converted.User.Identifier);
            Assert.AreEqual<string>(upr.TwitterHandle, converted.TwitterHandle);
            Assert.AreEqual<string>(upr.AbcHandle, converted.AbcHandle);
            Assert.AreEqual<string>(upr.City, converted.City);
            Assert.AreEqual<string>(upr.GitHubHandle, converted.GitHubHandle);
            Assert.AreEqual<string>(upr.Country, converted.Country);
            Assert.AreEqual<int?>(0, converted.MaximumAllowedApplications);
            Assert.AreEqual<Guid>(upr.CurrentApplicationIdentifier.Value, converted.CurrentApplication.Identifier);
            Assert.AreEqual<TimeZoneInfo>(TimeZoneInfo.Utc, converted.TimeZone);
            upr.TimeZone = TimeZoneInfo.Local.ToSerializedString();
            converted = upr.Convert();
            Assert.AreEqual<string>(TimeZoneInfo.Local.ToSerializedString(), converted.TimeZone.ToSerializedString());
        }

        [TestMethod]
        public void ConvertMaxiumAllowedApplications()
        {
            var random = new Random();
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid())
            {
                CurrentApplicationIdentifier = Guid.NewGuid(),
                MaxiumAllowedApplications = random.Next(),
            };

            var converted = upr.Convert();
            Assert.AreEqual<Guid>(upr.ApplicationIdentifier, converted.Application.Identifier);
            Assert.AreEqual<Guid>(upr.UserIdentifier, converted.User.Identifier);
            Assert.AreEqual<int?>(upr.MaxiumAllowedApplications, converted.MaximumAllowedApplications);
            Assert.AreEqual<Guid>(upr.CurrentApplicationIdentifier.Value, converted.CurrentApplication.Identifier);
        }

        [TestMethod]
        public void ConvertNullCurrentApplication()
        {
            var upr = new UserPreferenceRow(Guid.NewGuid(), Guid.NewGuid());

            var converted = upr.Convert();
            Assert.AreEqual<Guid>(upr.ApplicationIdentifier, converted.Application.Identifier);
            Assert.AreEqual<Guid>(upr.UserIdentifier, converted.User.Identifier);
            Assert.AreEqual<Guid>(Guid.Empty, converted.CurrentApplication.Identifier);
        }
        #endregion
    }
}