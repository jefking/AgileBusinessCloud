// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethodsTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Global.Azure
{
    using System;
    using Abc.Azure;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class ExtensionMethodsTest
    {
        #region Abc.Azure.AzureTable
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetConvertablePartitionKeyInvalid()
        {
            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            var msg = table.Get<MessageDisplay, MessageData>(null, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetConvertableRowKeyInvalid()
        {
            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            var msg = table.Get<MessageDisplay, MessageData>(StringHelper.ValidString(), StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        public void GetConvertable()
        {
            var data = new MessageData(Guid.NewGuid());
            data.Fill();

            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(data);
            var msg = table.Get<MessageDisplay, MessageData>(data.PartitionKey, data.RowKey);

            Assert.AreEqual<Guid>(data.Id, msg.Identifier);
            Assert.AreEqual<Guid>(data.ApplicationId, msg.Token.ApplicationId);
            Assert.AreEqual<string>(data.MachineName, msg.MachineName);
        }
        #endregion
    }
}