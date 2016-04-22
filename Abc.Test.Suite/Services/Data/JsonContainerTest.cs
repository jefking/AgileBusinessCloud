// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='JsonContainerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using Abc.Azure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using System;

    [TestClass]
    public class JsonContainerTest
    {
        #region Valid Cases
        [TestMethod]
        public void IsJsonContainer()
        {
            Assert.IsNotNull(new JsonContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount) as TextContainer<Entity>);
        }

        [TestMethod]
        public void SaveGet()
        {
            var random = new Random();
            var container = new JsonContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            container.EnsureExist();
            var entity = new Entity()
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