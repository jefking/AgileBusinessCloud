// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplicationDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserApplicationDataTest
    {
        #region Valid Cases
        [TestMethod]
        public void ApplicationId()
        {
            var appId = Guid.NewGuid();
            var data = new UserApplicationData(Guid.NewGuid(), appId);
            Assert.AreEqual<Guid>(appId, data.ApplicationId);
        }

        [TestMethod]
        public void UserId()
        {
            var userId = Guid.NewGuid();
            var data = new UserApplicationData(userId, Guid.NewGuid());
            Assert.AreEqual<Guid>(userId, data.UserId);
        }

        [TestMethod]
        public void Active()
        {
            var data = new UserApplicationData(Guid.NewGuid(), Guid.NewGuid())
            {
                Active = true,
            };
            Assert.AreEqual<bool>(true, data.Active);
        }

        [TestMethod]
        public void Deleted()
        {
            var data = new UserApplicationData(Guid.NewGuid(), Guid.NewGuid())
            {
                Deleted = true,
            };
            Assert.AreEqual<bool>(true, data.Deleted);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var now = DateTime.UtcNow;
            var data = new UserApplicationData(Guid.NewGuid(), Guid.NewGuid())
            {
                CreatedOn = now,
            };
            Assert.AreEqual<DateTime>(now, data.CreatedOn);
        }

        [TestMethod]
        public void CreatedBy()
        {
            var id = Guid.NewGuid();
            var data = new UserApplicationData(Guid.NewGuid(), Guid.NewGuid())
            {
                CreatedBy = id,
            };
            Assert.AreEqual<Guid>(id, data.CreatedBy);
        }

        [TestMethod]
        public void LastUpdatedOn()
        {
            var now = DateTime.UtcNow;
            var data = new UserApplicationData(Guid.NewGuid(), Guid.NewGuid())
            {
                LastUpdatedOn = now,
            };
            Assert.AreEqual<DateTime>(now, data.LastUpdatedOn);
        }

        [TestMethod]
        public void LastUpdatedBy()
        {
            var id = Guid.NewGuid();
            var data = new UserApplicationData(Guid.NewGuid(), Guid.NewGuid())
            {
                LastUpdatedBy = id,
            };
            Assert.AreEqual<Guid>(id, data.LastUpdatedBy);
        }

        [TestMethod]
        public void Convert()
        {
            var data = new UserApplicationData(Guid.NewGuid(), Guid.NewGuid())
            {
                Active = true,
                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Deleted = true,
                LastUpdatedBy = Guid.NewGuid(),
                LastUpdatedOn = DateTime.UtcNow,
            };

            var converted = data.Convert();

            Assert.AreEqual<bool>(data.Active, converted.Active);
            Assert.AreEqual<bool>(data.Deleted, converted.Deleted);
            Assert.AreEqual<Guid>(data.UserId, converted.User.Identifier);
            Assert.AreEqual<Guid>(data.ApplicationId, converted.Application.Identifier);
        }
        #endregion
    }
}