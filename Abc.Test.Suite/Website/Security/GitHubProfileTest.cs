// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='GitHubProfileTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website.Security
{
    using Abc.Website.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class GitHubProfileTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new GitHubProfile();
        }

        [TestMethod]
        public void Login()
        {
            var item = new GitHubProfile();
            var data = StringHelper.ValidString();
            item.Login = data;
            Assert.AreEqual<string>(data, item.Login);
        }

        [TestMethod]
        public void Email()
        {
            var item = new GitHubProfile();
            var data = StringHelper.ValidString();
            item.Email = data;
            Assert.AreEqual<string>(data, item.Email);
        }

        [TestMethod]
        public void Id()
        {
            var random = new Random();
            var item = new GitHubProfile();
            var data = random.Next();
            item.Id = data;
            Assert.AreEqual<int>(data, item.Id);
        }
        #endregion
    }
}