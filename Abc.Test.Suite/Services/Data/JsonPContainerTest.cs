// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='JsonPContainerTest.cs'>
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
    public class JsonPContainerTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorMethodInvalid()
        {
            new JsonPContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount, StringHelper.NullEmptyWhiteSpace());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void IsJsonContainer()
        {
            Assert.IsNotNull(new JsonPContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount, "happy") as JsonContainer<Entity>);
        }

        [TestMethod]
        public void JsonPFormat()
        {
            Assert.AreEqual<string>("{0}({1})", JsonPContainer<Entity>.JsonPFormat);
        }

        [TestMethod]
        public void Constructor()
        {
            new JsonPContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount, "happy");
        }

        [TestMethod]
        public void Method()
        {
            var method = StringHelper.ValidString();
            var data = new JsonPContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount, method);
            Assert.AreEqual<string>(method, data.Method);
        }

        [TestMethod]
        public void Store()
        {
            var random = new Random();
            var method = "Amazing";
            var data = new JsonPContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount, method);
            data.EnsureExist();
            var entity = new Entity()
            {
                PartitionKey = Guid.NewGuid().ToBase64(),
                RowKey = Guid.NewGuid().ToAscii85(),
                ToTest = random.Next()
            };
            var id = Guid.NewGuid().ToString();
            data.Save(id, entity);
        }
        #endregion
    }
}