// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationInfoDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationInfoDataTest
    {
        #region Valid Tests
        [TestMethod]
        public void Constructor()
        {
            new ApplicationInfoData(Guid.NewGuid());
        }

        [TestMethod]
        public void Key()
        {
            Assert.AreEqual<string>(string.Empty, ApplicationInfoData.Key);
        }

        [TestMethod]
        public void ApplicationId()
        {
            var appId = Guid.NewGuid();
            var appInfo = new ApplicationInfoData(appId);
            Assert.AreEqual<Guid>(appId, appInfo.ApplicationId);
        }

        [TestMethod]
        public void Name()
        {
            var name = StringHelper.ValidString();
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.Name = name;
            Assert.AreEqual<string>(name, appInfo.Name);
        }

        [TestMethod]
        public void PublicKey()
        {
            var name = StringHelper.ValidString();
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.PublicKey = name;
            Assert.AreEqual<string>(name, appInfo.PublicKey);
        }

        [TestMethod]
        public void Description()
        {
            var description = StringHelper.ValidString();
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.Description = description;
            Assert.AreEqual<string>(description, appInfo.Description);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var createdOn = DateTime.UtcNow;
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.CreatedOn = createdOn;
            Assert.AreEqual<DateTime>(createdOn, appInfo.CreatedOn);
        }

        [TestMethod]
        public void CreatedBy()
        {
            var createdBy = Guid.NewGuid();
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.CreatedBy = createdBy;
            Assert.AreEqual<Guid>(createdBy, appInfo.CreatedBy);
        }

        [TestMethod]
        public void Owner()
        {
            var owner = Guid.NewGuid();
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.Owner = owner;
            Assert.AreEqual<Guid>(owner, appInfo.Owner);
        }

        [TestMethod]
        public void LastUpdatedOn()
        {
            var lastUpdatedOn = DateTime.UtcNow;
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.LastUpdatedOn = lastUpdatedOn;
            Assert.AreEqual<DateTime>(lastUpdatedOn, appInfo.LastUpdatedOn);
        }

        [TestMethod]
        public void LastUpdatedBy()
        {
            var lastUpdatedBy = Guid.NewGuid();
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.LastUpdatedBy = lastUpdatedBy;
            Assert.AreEqual<Guid>(lastUpdatedBy, appInfo.LastUpdatedBy);
        }

        [TestMethod]
        public void Environment()
        {
            var environment = StringHelper.ValidString();
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.Environment = environment;
            Assert.AreEqual<string>(environment, appInfo.Environment);
        }

        [TestMethod]
        public void Active()
        {
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.Active = true;
            Assert.IsTrue(appInfo.Active);
            appInfo.Active = false;
            Assert.IsFalse(appInfo.Active);
        }

        [TestMethod]
        public void Deleted()
        {
            var appInfo = new ApplicationInfoData(Guid.NewGuid());
            appInfo.Deleted = true;
            Assert.IsTrue(appInfo.Deleted);
            appInfo.Deleted = false;
            Assert.IsFalse(appInfo.Deleted);
        }

        [TestMethod]
        public void Convert()
        {
            var appInfo = new ApplicationInfoData(Guid.NewGuid())
            {
                Deleted = true,
                Active = true,
                Description = StringHelper.ValidString(),
                Environment = StringHelper.ValidString(),
                Name = StringHelper.ValidString(),
                PublicKey = StringHelper.ValidString(),
                Owner = Guid.NewGuid(),
            };

            var converted = appInfo.Convert();
            Assert.AreEqual<bool>(appInfo.Active, converted.Active);
            Assert.AreEqual<bool>(appInfo.Deleted, converted.Deleted);
            Assert.AreEqual<string>(appInfo.Description, converted.Description);
            Assert.AreEqual<string>(appInfo.Environment, converted.Environment);
            Assert.AreEqual<string>(appInfo.Name, converted.Name);
            Assert.AreEqual<string>(appInfo.PublicKey, converted.PublicKey);
            Assert.AreEqual<Guid>(appInfo.Owner, converted.OwnerId);
        }
        #endregion
    }
}