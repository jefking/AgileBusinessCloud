// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPublicProfileTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserPublicProfileTest
    {
        #region Valid Tests
        [TestMethod]
        public void Constructor()
        {
            new UserPublicProfile();
        }

        [TestMethod]
        public void UserName()
        {
            var user = new UserPublicProfile();
            var data = StringHelper.ValidString();
            user.UserName = data;
            Assert.AreEqual<string>(data, user.UserName);
        }

        [TestMethod]
        public void Gravatar()
        {
            var user = new UserPublicProfile();
            var data = StringHelper.ValidString();
            user.Gravatar = data;
            Assert.AreEqual<string>(data, user.Gravatar);
        }
        #endregion
    }
}
