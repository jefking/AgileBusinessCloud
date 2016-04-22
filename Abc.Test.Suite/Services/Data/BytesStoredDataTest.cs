// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BytesStoredDataTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorApplicationIdEmpty()
        {
            new BytesStoredData(Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BytesStoredData();
        }

        [TestMethod]
        public void ConstructorApplicationId()
        {
            new BytesStoredData(Guid.NewGuid());
        }

        [TestMethod]
        public void ApplicationId()
        {
            var appId = Guid.NewGuid();
            var data = new BytesStoredData(appId);
            Assert.AreEqual<Guid>(appId, data.ApplicationId);
        }

        [TestMethod]
        public void DataCostType()
        {
            var random = new Random();
            var data = new BytesStoredData(Guid.NewGuid());
            var test = random.Next();
            data.DataCostType = test;
            Assert.AreEqual<int>(test, data.DataCostType);
        }

        [TestMethod]
        public void Bytes()
        {
            var random = new Random();
            var data = new BytesStoredData(Guid.NewGuid());
            var test = random.Next();
            data.Bytes = test;
            Assert.AreEqual<int>(test, data.Bytes);
        }

        [TestMethod]
        public void ObjectType()
        {
            var data = new BytesStoredData(Guid.NewGuid());
            var test = StringHelper.ValidString();
            data.ObjectType = test;
            Assert.AreEqual<string>(test, data.ObjectType);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var data = new BytesStoredData(Guid.NewGuid())
            {
                Bytes = random.Next(),
                DataCostType = random.Next(),
                ObjectType = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
            };

            var converted = data.Convert();
            Assert.AreEqual<int>(data.Bytes, converted.Bytes);
            Assert.AreEqual<int>(data.DataCostType, converted.DataCostType);
            Assert.AreEqual<string>(data.ObjectType, converted.ObjectType);
            Assert.AreEqual<DateTime>(data.OccurredOn, converted.OccurredOn);
            Assert.AreEqual<Guid>(data.ApplicationId, converted.ApplicationId);
        }
        #endregion
    }
}