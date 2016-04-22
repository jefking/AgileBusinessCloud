// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserProfileRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserProfileRowTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorApplicationIdentifierEmpty()
        {
            new UserProfileRow(Guid.Empty, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorHandleInvalid()
        {
            new UserProfileRow(Guid.NewGuid(), StringHelper.NullEmptyWhiteSpace());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void DefaultConstructor()
        {
            new UserProfileRow();
        }

        [TestMethod]
        public void ApplicationIdentifier()
        {
            var data = Guid.NewGuid();
            var row = new UserProfileRow(data, StringHelper.ValidString());
            Assert.AreEqual<Guid>(data, row.ApplicationIdentifier);
            Assert.AreEqual<string>(data.ToString(), row.PartitionKey);
        }

        [TestMethod]
        public void Handle()
        {
            var data = StringHelper.ValidString();
            var row = new UserProfileRow(Guid.NewGuid(), data);
            Assert.AreEqual<string>(data, row.RowKey);
        }

        [TestMethod]
        public void ApplicationIdentifierEmpty()
        {
            var row = new UserProfileRow();
            Assert.AreEqual<Guid>(Guid.Empty, row.ApplicationIdentifier);
            Assert.IsNull(row.PartitionKey);
        }

        [TestMethod]
        public void OwnerIdentifier()
        {
            var row = new UserProfileRow(Guid.NewGuid(), StringHelper.ValidString());
            var data = Guid.NewGuid();
            row.OwnerIdentifier = data;
            Assert.AreEqual<Guid>(data, row.OwnerIdentifier);
        }

        [TestMethod]
        public void PreferedProfile()
        {
            var row = new UserProfileRow(Guid.NewGuid(), StringHelper.ValidString());
            Assert.IsNull(row.PreferedProfile);
            var data = true;
            row.PreferedProfile = data;
            Assert.AreEqual<bool?>(data, row.PreferedProfile);
        }

        [TestMethod]
        public void Word()
        {
            var row = new UserProfileRow(Guid.NewGuid(), StringHelper.ValidString());
            var data = StringHelper.ValidString();
            row.Word = data;
            Assert.AreEqual<string>(data, row.Word);
        }

        [TestMethod]
        public void Points()
        {
            var random = new Random();
            var row = new UserProfileRow(Guid.NewGuid(), StringHelper.ValidString());
            Assert.IsNull(row.Points);
            var data = random.Next();
            row.Points = data;
            Assert.AreEqual<int?>(data, row.Points);
        }

        [TestMethod]
        public void Convert()
        {
            var row = new UserProfileRow(Guid.NewGuid(), StringHelper.ValidString())
            {
                OwnerIdentifier = Guid.NewGuid(),
                Word = StringHelper.ValidString(),
            };

            var item = row.Convert();

            Assert.AreEqual<Guid>(row.ApplicationIdentifier, item.ApplicationIdentifier);
            Assert.AreEqual<Guid>(row.OwnerIdentifier, item.OwnerIdentifier);
            Assert.AreEqual<string>(row.RowKey, item.Handle);
            Assert.AreEqual<string>(row.Word, item.Word);
            Assert.IsFalse(item.PreferedProfile);
            Assert.AreEqual<int>(0, item.Points);
        }

        [TestMethod]
        public void ConvertPreferedProfile()
        {
            var row = new UserProfileRow(Guid.NewGuid(), StringHelper.ValidString())
            {
                OwnerIdentifier = Guid.NewGuid(),
                PreferedProfile = true,
            };

            var item = row.Convert();

            Assert.AreEqual<Guid>(row.ApplicationIdentifier, item.ApplicationIdentifier);
            Assert.AreEqual<Guid>(row.OwnerIdentifier, item.OwnerIdentifier);
            Assert.AreEqual<string>(row.RowKey, item.Handle);
            Assert.IsTrue(row.PreferedProfile.Value);
        }

        [TestMethod]
        public void ConvertPoints()
        {
            var row = new UserProfileRow(Guid.NewGuid(), StringHelper.ValidString())
            {
                OwnerIdentifier = Guid.NewGuid(),
                PreferedProfile = true,
                Points = 1000,
            };

            var item = row.Convert();

            Assert.AreEqual<Guid>(row.ApplicationIdentifier, item.ApplicationIdentifier);
            Assert.AreEqual<Guid>(row.OwnerIdentifier, item.OwnerIdentifier);
            Assert.AreEqual<string>(row.RowKey, item.Handle);
            Assert.AreEqual<int?>(row.Points, item.Points);
            Assert.IsTrue(row.PreferedProfile.Value);
        }
        #endregion
    }
}