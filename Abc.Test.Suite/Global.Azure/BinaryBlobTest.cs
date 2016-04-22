// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BinaryBlobTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Global.Azure
{
    using System;
    using Abc.Azure;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class BinaryBlobTest
    {
        #region Classes
        [AzureDataStore("logging")]
        [Serializable]
        public class SerializeMePlease
        {
            public Guid Identifier
            {
                get;
                set;
            }
        }
        #endregion

        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorAccountNull()
        {
            new BinaryBlob<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveItemNull()
        {
            var blob = new BinaryBlob<SerializeMePlease>(CloudStorageAccount.DevelopmentStorageAccount);
            blob.Save(StringHelper.ValidString(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveObjectIdInvalid()
        {
            var blob = new BinaryBlob<SerializeMePlease>(CloudStorageAccount.DevelopmentStorageAccount);
            blob.Save(StringHelper.NullEmptyWhiteSpace(), new SerializeMePlease());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetObjectIdInvalid()
        {
            var blob = new BinaryBlob<SerializeMePlease>(CloudStorageAccount.DevelopmentStorageAccount);
            blob.Get(StringHelper.NullEmptyWhiteSpace());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BinaryBlob<object>(CloudStorageAccount.DevelopmentStorageAccount);
        }

        [TestMethod]
        public void ConstructorDataStore()
        {
            new BinaryBlob<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
        }

        [TestMethod]
        public void RoundTrip()
        {
            var random = new Random();
            var objectId = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var blob = new BinaryBlob<SerializeMePlease>(CloudStorageAccount.DevelopmentStorageAccount);
            var data = new SerializeMePlease()
            {
                Identifier = Guid.NewGuid(),
            };

            blob.Save(objectId, data);

            var got = blob.Get(objectId);
            Assert.AreEqual<Guid>(data.Identifier, got.Identifier);
        }
        #endregion
    }
}