// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserPublicProfileTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Data
{
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class UserPublicProfileTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new UserPublicProfile();
        }

        [TestMethod]
        public void UserName()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.UserName = data;
            Assert.AreEqual<string>(data, userRole.UserName);
        }

        [TestMethod]
        public void Points()
        {
            var random = new Random();
            var userRole = new UserPublicProfile();
            var data = random.Next();
            userRole.Points = data;
            Assert.AreEqual<int>(data, userRole.Points);
        }

        [TestMethod]
        public void Word()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.Word = data;
            Assert.AreEqual<string>(data, userRole.Word);
        }

        [TestMethod]
        public void GitHubHandle()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.GitHubHandle = data;
            Assert.AreEqual<string>(data, userRole.GitHubHandle);
        }

        [TestMethod]
        public void Handle()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.Handle = data;
            Assert.AreEqual<string>(data, userRole.Handle);
        }

        [TestMethod]
        public void Gravatar()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.Gravatar = data;
            Assert.AreEqual<string>(data, userRole.Gravatar);
        }

        [TestMethod]
        public void PreferedProfile()
        {
            var userRole = new UserPublicProfile();
            Assert.IsFalse(userRole.PreferedProfile);
            userRole.PreferedProfile = true;
            Assert.IsTrue(userRole.PreferedProfile);
        }

        [TestMethod]
        public void TwitterHandle()
        {
            var userRole = new UserPublicProfile();
            var data = StringHelper.ValidString();
            userRole.TwitterHandle = data;
            Assert.AreEqual<string>(data, userRole.TwitterHandle);
        }

        [TestMethod]
        public void OwnerIdentifier()
        {
            var userRole = new UserPublicProfile();
            var data = Guid.NewGuid();
            userRole.OwnerIdentifier = data;
            Assert.AreEqual<Guid>(data, userRole.OwnerIdentifier);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var profile = new UserPublicProfile()
            {
                Handle = StringHelper.ValidString(),
                PreferedProfile = true,
                Points = random.Next(),
                Word = StringHelper.ValidString(),
                OwnerIdentifier = Guid.NewGuid(),
            };

            var converted = ((IConvert<ProfilePage>)profile).Convert();
            Assert.IsNotNull(converted);
            Assert.IsNull(converted.ExistingHandle);
            Assert.AreEqual<string>(profile.Handle, converted.Handle);
            Assert.AreEqual<string>(profile.Word, converted.Word);
            Assert.AreEqual<int>(profile.Points, converted.Points);
            Assert.AreEqual<bool>(profile.PreferedProfile, converted.PreferedProfile);
            Assert.AreEqual<Guid>(profile.OwnerIdentifier, converted.OwnerIdentifier);
        }
        #endregion
    }
}