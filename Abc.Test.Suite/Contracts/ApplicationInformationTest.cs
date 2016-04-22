// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationInformationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Abc.Underpinning.Administration;
    using Abc.Website;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationInformationTest
    {
        #region Valid Cases
        [TestMethod]
        public void ApplicationId()
        {
            var appId = Guid.NewGuid();
            var appInfo = new ApplicationInformation();
            appInfo.Identifier = appId;
            Assert.AreEqual<Guid>(appId, appInfo.Identifier);
        }

        [TestMethod]
        public void OwnerId()
        {
            var appId = Guid.NewGuid();
            var appInfo = new ApplicationInformation();
            appInfo.OwnerId = appId;
            Assert.AreEqual<Guid>(appId, appInfo.OwnerId);
        }

        [TestMethod]
        public void Name()
        {
            var name = StringHelper.ValidString();
            var appInfo = new ApplicationInformation();
            appInfo.Name = name;
            Assert.AreEqual<string>(name, appInfo.Name);
        }

        [TestMethod]
        public void PublicKey()
        {
            var name = StringHelper.ValidString();
            var appInfo = new ApplicationInformation();
            appInfo.PublicKey = name;
            Assert.AreEqual<string>(name, appInfo.PublicKey);
        }

        [TestMethod]
        public void Description()
        {
            var description = StringHelper.ValidString();
            var appInfo = new ApplicationInformation();
            appInfo.Description = description;
            Assert.AreEqual<string>(description, appInfo.Description);
        }

        [TestMethod]
        public void Environment()
        {
            var environment = StringHelper.ValidString();
            var appInfo = new ApplicationInformation();
            appInfo.Environment = environment;
            Assert.AreEqual<string>(environment, appInfo.Environment);
        }

        [TestMethod]
        public void Active()
        {
            var appInfo = new ApplicationInformation();
            appInfo.Active = true;
            Assert.IsTrue(appInfo.Active);
            appInfo.Active = false;
            Assert.IsFalse(appInfo.Active);
        }

        [TestMethod]
        public void Deleted()
        {
            var appInfo = new ApplicationInformation();
            appInfo.Deleted = true;
            Assert.IsTrue(appInfo.Deleted);
            appInfo.Deleted = false;
            Assert.IsFalse(appInfo.Deleted);
        }

        [TestMethod]
        public void IsNew()
        {
            var appInfo = new ApplicationInformation();
            appInfo.IsNew = true;
            Assert.IsTrue(appInfo.IsNew);
            appInfo.IsNew = false;
            Assert.IsFalse(appInfo.IsNew);
        }

        [TestMethod]
        public void IsValid()
        {
            var appInfo = new ApplicationInformation();
            appInfo.IsValid = true;
            Assert.IsTrue(appInfo.IsValid);
            appInfo.IsValid = false;
            Assert.IsFalse(appInfo.IsValid);
        }

        [TestMethod]
        public void ValidUntil()
        {
            var now = DateTime.UtcNow;
            var appInfo = new ApplicationInformation();
            appInfo.ValidUntil = now;
            Assert.AreEqual<DateTime>(now, appInfo.ValidUntil);
        }

        [TestMethod]
        public void LoadNull()
        {
            var appInfo = new ApplicationInformation();
            appInfo.Load(null);
        }

        [TestMethod]
        public void Load()
        {
            var details = new Details()
            {
                IsValid = true,
                ValidUntil = DateTime.UtcNow,
                ApplicationId = Guid.NewGuid(),
            };

            var appInfo = new ApplicationInformation();
            appInfo.Load(details);
            Assert.AreEqual<bool>(details.IsValid, appInfo.IsValid);
            Assert.AreEqual<DateTime>(details.ValidUntil, appInfo.ValidUntil);
            Assert.AreEqual<Guid>(details.ApplicationId, appInfo.Identifier);
        }

        [TestMethod]
        public void Convert()
        {
            var appInfo = new ApplicationInformation()
            {
                Active = true,
                Identifier = Guid.NewGuid(),
                Deleted = true,
                Description = StringHelper.ValidString(),
                Environment = StringHelper.ValidString(),
                IsValid = true,
                Name = StringHelper.ValidString(),
                PublicKey = StringHelper.ValidString(),
                ValidUntil = DateTime.UtcNow,
                IsNew = true,
                OwnerId = Guid.NewGuid(),
            };

            var model = appInfo.Convert();
            Assert.AreEqual<bool>(model.Active, appInfo.Active);
            Assert.AreEqual<bool>(model.Deleted, appInfo.Deleted);
            Assert.AreEqual<bool>(model.IsValid, appInfo.IsValid);
            Assert.AreEqual<bool>(model.New, appInfo.IsNew);
            Assert.IsFalse(model.IsOwner);
            Assert.AreEqual<Guid>(model.ApplicationId, appInfo.Identifier);
            Assert.AreEqual<string>(model.Description, appInfo.Description);
            Assert.AreEqual<string>(model.Environment, appInfo.Environment);
            Assert.AreEqual<string>(model.Name, appInfo.Name);
            Assert.AreEqual<string>(model.PublicKey, appInfo.PublicKey);
            Assert.AreEqual<DateTime>(model.ValidUntil, appInfo.ValidUntil);
        }

        [TestMethod]
        public void ConvertWithUser()
        {
            var owner = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var appInfo = new ApplicationInformation()
            {
                Active = true,
                Identifier = Guid.NewGuid(),
                Deleted = true,
                Description = StringHelper.ValidString(),
                Environment = StringHelper.ValidString(),
                IsValid = true,
                Name = StringHelper.ValidString(),
                ValidUntil = DateTime.UtcNow,
                IsNew = true,
                OwnerId = owner.Identifier,
            };

            var model = appInfo.Convert(owner);
            Assert.AreEqual<bool>(model.Active, appInfo.Active);
            Assert.AreEqual<bool>(model.Deleted, appInfo.Deleted);
            Assert.AreEqual<bool>(model.IsValid, appInfo.IsValid);
            Assert.AreEqual<bool>(model.New, appInfo.IsNew);
            Assert.IsTrue(model.IsOwner);
            Assert.AreEqual<Guid>(model.ApplicationId, appInfo.Identifier);
            Assert.AreEqual<string>(model.Description, appInfo.Description);
            Assert.AreEqual<string>(model.Environment, appInfo.Environment);
            Assert.AreEqual<string>(model.Name, appInfo.Name);
            Assert.AreEqual<DateTime>(model.ValidUntil, appInfo.ValidUntil);
        }
        #endregion
    }
}