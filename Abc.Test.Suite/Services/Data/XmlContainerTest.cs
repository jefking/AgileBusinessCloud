// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='XmlContainerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Azure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class XmlContainerTest
    {
        #region Valid Cases
        [TestMethod]
        public void SaveGet()
        {
            Random random = new Random();
            TextContainer<EntityWithDataStore> container = new XmlContainer<EntityWithDataStore>(CloudStorageAccount.DevelopmentStorageAccount);
            container.EnsureExist();
            var entity = new EntityWithDataStore()
            {
                PartitionKey = Guid.NewGuid().ToBase64(),
                RowKey = Guid.NewGuid().ToAscii85(),
                ToTest = random.Next()
            };
            var id = Guid.NewGuid().ToString();
            container.Save(id, entity);

            var returned = container.Get(id);

            Assert.IsNotNull(returned);
            Assert.AreEqual<string>(entity.PartitionKey, returned.PartitionKey);
            Assert.AreEqual<string>(entity.RowKey, returned.RowKey);
            Assert.AreEqual<int>(entity.ToTest, returned.ToTest);
        }
        #endregion
    }
}