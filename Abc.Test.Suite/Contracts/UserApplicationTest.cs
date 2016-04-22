// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplicationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserApplicationTest
    {
        #region Valid Cases
        [TestMethod]
        public void User()
        {
            var user = new User();
            var ua = new UserApplication()
            {
                User = user,
            };

            Assert.AreEqual<User>(user, ua.User);
        }

        [TestMethod]
        public void Active()
        {
            var active = true;
            var ua = new UserApplication()
            {
                Active = active,
            };

            Assert.AreEqual<bool>(active, ua.Active);
        }

        [TestMethod]
        public void Deleted()
        {
            var deleted = true;
            var ua = new UserApplication()
            {
                Deleted = deleted,
            };

            Assert.AreEqual<bool>(deleted, ua.Deleted);
        }

        [TestMethod]
        public void Convert()
        {
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };

            var application = new Application()
            {
                Identifier = Guid.NewGuid(),
            };

            var ua = new UserApplication()
            {
                User = user,
                Active = true,
                Application = application,
                Deleted = true,
            };

            var converted = ua.Convert();

            Assert.AreEqual<bool>(ua.Active, converted.Active);
            Assert.AreEqual<bool>(ua.Deleted, converted.Deleted);
            Assert.AreEqual<Guid>(ua.User.Identifier, converted.UserId);
            Assert.AreEqual<Guid>(ua.Application.Identifier, converted.ApplicationId);
        }
        #endregion
    }
}