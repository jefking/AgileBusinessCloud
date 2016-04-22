// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BytesStoredTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BytesStored();
        }

        [TestMethod]
        public void ApplicationId()
        {
            var data = new BytesStored();
            var test = Guid.NewGuid();
            data.ApplicationId = test;
            Assert.AreEqual<Guid>(test, data.ApplicationId);
        }

        [TestMethod]
        public void DataCostType()
        {
            var random = new Random();
            var data = new BytesStored();
            var test = random.Next();
            data.DataCostType = test;
            Assert.AreEqual<int>(test, data.DataCostType);
        }

        [TestMethod]
        public void OccurredOn()
        {
            var data = new BytesStored();
            var test = DateTime.UtcNow;
            data.OccurredOn = test;
            Assert.AreEqual<DateTime>(test, data.OccurredOn);
        }

        [TestMethod]
        public void Bytes()
        {
            var random = new Random();
            var data = new BytesStored();
            var test = random.Next();
            data.Bytes = test;
            Assert.AreEqual<int>(test, data.Bytes);
        }

        [TestMethod]
        public void ObjectType()
        {
            var data = new BytesStored();
            var test = StringHelper.ValidString();
            data.ObjectType = test;
            Assert.AreEqual<string>(test, data.ObjectType);
        }
        #endregion
    }
}
