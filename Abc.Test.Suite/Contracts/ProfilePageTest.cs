// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ProfilePageTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProfilePageTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ProfilePage();
        }

        [TestMethod]
        public void ApplicationIdentifier()
        {
            var item = new ProfilePage();
            var data = Guid.NewGuid();
            item.ApplicationIdentifier = data;
            Assert.AreEqual<Guid>(data, item.ApplicationIdentifier);
        }

        [TestMethod]
        public void OwnerIdentifier()
        {
            var item = new ProfilePage();
            var data = Guid.NewGuid();
            item.OwnerIdentifier = data;
            Assert.AreEqual<Guid>(data, item.OwnerIdentifier);
        }

        [TestMethod]
        public void PreferedProfile()
        {
            var item = new ProfilePage();
            Assert.IsFalse(item.PreferedProfile);
            item.PreferedProfile = true;
            Assert.IsTrue(item.PreferedProfile);
        }

        [TestMethod]
        public void Handle()
        {
            var item = new ProfilePage();
            var data = StringHelper.ValidString();
            item.Handle = data;
            Assert.AreEqual<string>(data, item.Handle);
        }

        [TestMethod]
        public void ExistingHandle()
        {
            var item = new ProfilePage();
            var data = StringHelper.ValidString();
            item.ExistingHandle = data;
            Assert.AreEqual<string>(data, item.ExistingHandle);
        }

        [TestMethod]
        public void Word()
        {
            var row = new ProfilePage();
            var data = StringHelper.ValidString();
            row.Word = data;
            Assert.AreEqual<string>(data, row.Word);
        }

        [TestMethod]
        public void Points()
        {
            var random = new Random();
            var row = new ProfilePage();
            var data = random.Next();
            row.Points = data;
            Assert.AreEqual<int?>(data, row.Points);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var item = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = StringHelper.ValidString(),
                Word = StringHelper.ValidString(),
                OwnerIdentifier = Guid.NewGuid(),
                Points = random.Next(),
            };

            var row = item.Convert();

            Assert.AreEqual<Guid>(item.ApplicationIdentifier, row.ApplicationIdentifier);
            Assert.AreEqual<Guid>(item.OwnerIdentifier, row.OwnerIdentifier);
            Assert.AreEqual<string>(item.Handle, row.RowKey);
            Assert.AreEqual<string>(item.Word, row.Word);
            Assert.AreEqual<int?>(item.Points, row.Points);
            Assert.IsFalse(row.PreferedProfile.Value);
        }

        [TestMethod]
        public void ConvertPreferedProfile()
        {
            var item = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = StringHelper.ValidString(),
                OwnerIdentifier = Guid.NewGuid(),
                PreferedProfile = true
            };

            var row = item.Convert();

            Assert.AreEqual<Guid>(item.ApplicationIdentifier, row.ApplicationIdentifier);
            Assert.AreEqual<Guid>(item.OwnerIdentifier, row.OwnerIdentifier);
            Assert.AreEqual<string>(item.Handle, row.RowKey);
            Assert.IsTrue(row.PreferedProfile.Value);
        }
        #endregion
    }
}