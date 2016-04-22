// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ContentTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContentTest
    {
        #region Valid Cases
        [TestMethod]
        public void CreatedOn()
        {
            var data = DateTime.UtcNow;
            var content = new Content();
            content.CreatedOn = data;
            Assert.AreEqual<DateTime>(data, content.CreatedOn);
        }

        [TestMethod]
        public void UpdatedOn()
        {
            var data = DateTime.UtcNow;
            var content = new Content();
            content.UpdatedOn = data;
            Assert.AreEqual<DateTime>(data, content.UpdatedOn);
        }

        [TestMethod]
        public void Deleted()
        {
            var content = new Content();
            Assert.IsFalse(content.Deleted);
            content.Deleted = true;
            Assert.IsTrue(content.Deleted);
        }

        [TestMethod]
        public void Active()
        {
            var content = new Content();
            Assert.IsFalse(content.Active);
            content.Active = true;
            Assert.IsTrue(content.Active);
        }

        [TestMethod]
        public void Id()
        {
            var data = Guid.NewGuid();
            var content = new Content();
            content.Id = data;
            Assert.AreEqual<Guid>(data, content.Id);
        }
        #endregion
    }
}
