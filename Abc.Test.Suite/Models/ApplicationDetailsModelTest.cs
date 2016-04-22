// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationDetailsModelTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationDetailsModelTest
    {
        #region Valid Cases
        /// <summary>
        /// A test for ApplicationDetailsModel Constructor
        /// </summary>
        [TestMethod]
        public void ApplicationDetailsModelConstructor()
        {
            ApplicationDetailsModel target = new ApplicationDetailsModel();
            Assert.IsNotNull(target);
        }

        /// <summary>
        /// A test for ApplicationId
        /// </summary>
        [TestMethod]
        public void ApplicationId()
        {
            ApplicationDetailsModel target = new ApplicationDetailsModel();
            Guid expected = new Guid();
            target.ApplicationId = expected;
            var actual = target.ApplicationId;
            Assert.AreEqual<Guid>(expected, actual);
        }

        [TestMethod]
        public void User()
        {
            var target = new ApplicationDetailsModel();
            var data = new UserPublicProfile();
            target.User = data;
            Assert.AreEqual<UserPublicProfile>(data, target.User);
        }

        /// <summary>
        /// A test for IsValid
        /// </summary>
        [TestMethod]
        public void IsValid()
        {
            ApplicationDetailsModel target = new ApplicationDetailsModel();
            bool expected = false;
            target.IsValid = expected;
            var actual = target.IsValid;
            Assert.AreEqual<bool>(expected, actual);
        }

        [TestMethod]
        public void IsOwner()
        {
            var target = new ApplicationDetailsModel();
            Assert.IsFalse(target.IsOwner);
            target.IsOwner = true;
            Assert.IsTrue(target.IsOwner);
        }

        /// <summary>
        /// A test for New
        /// </summary>
        [TestMethod]
        public void New()
        {
            var target = new ApplicationDetailsModel();
            bool expected = true;
            target.New = expected;
            var actual = target.New;
            Assert.AreEqual<bool>(expected, actual);
        }

        /// <summary>
        /// A test for ValidUntil
        /// </summary>
        [TestMethod]
        public void ValidUntil()
        {
            ApplicationDetailsModel target = new ApplicationDetailsModel();
            DateTime expected = new DateTime();
            target.ValidUntil = expected;
            var actual = target.ValidUntil;
            Assert.AreEqual<DateTime>(expected, actual);
        }

        [TestMethod]
        public void Id()
        {
            var id = Guid.NewGuid();
            var appInfo = new ApplicationDetailsModel();
            appInfo.Id = id;
            Assert.AreEqual<Guid>(id, appInfo.Id);
        }

        [TestMethod]
        public void Name()
        {
            var name = StringHelper.ValidString();
            var appInfo = new ApplicationDetailsModel();
            appInfo.Name = name;
            Assert.AreEqual<string>(name, appInfo.Name);
        }

        [TestMethod]
        public void PublicKey()
        {
            var data = StringHelper.ValidString();
            var appInfo = new ApplicationDetailsModel();
            appInfo.PublicKey = data;
            Assert.AreEqual<string>(data, appInfo.PublicKey);
        }

        [TestMethod]
        public void Description()
        {
            var description = StringHelper.ValidString();
            var appInfo = new ApplicationDetailsModel();
            appInfo.Description = description;
            Assert.AreEqual<string>(description, appInfo.Description);
        }

        [TestMethod]
        public void Environment()
        {
            var environment = StringHelper.ValidString();
            var appInfo = new ApplicationDetailsModel();
            appInfo.Environment = environment;
            Assert.AreEqual<string>(environment, appInfo.Environment);
        }

        [TestMethod]
        public void Active()
        {
            var appInfo = new ApplicationDetailsModel();
            appInfo.Active = true;
            Assert.IsTrue(appInfo.Active);
            appInfo.Active = false;
            Assert.IsFalse(appInfo.Active);
        }

        [TestMethod]
        public void Deleted()
        {
            var appInfo = new ApplicationDetailsModel();
            appInfo.Deleted = true;
            Assert.IsTrue(appInfo.Deleted);
            appInfo.Deleted = false;
            Assert.IsFalse(appInfo.Deleted);
        }
        #endregion
    }
}