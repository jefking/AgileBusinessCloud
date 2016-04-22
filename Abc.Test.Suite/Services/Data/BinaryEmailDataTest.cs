// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BinaryEmailDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BinaryEmailDataTest
    {
        #region Error Cases
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BinaryEmailData();
        }

        [TestMethod]
        public void RowKey()
        {
            var email = new BinaryEmailData(Guid.NewGuid());
            Assert.IsFalse(string.IsNullOrWhiteSpace(email.RowKey));
        }

        [TestMethod]
        public void PartitionKey()
        {
            Guid applicationId = Guid.NewGuid();

            var email = new BinaryEmailData(applicationId);

            Assert.AreEqual<string>(applicationId.ToString(), email.PartitionKey);
        }

        [TestMethod]
        public void ApplicationId()
        {
            Guid applicationId = Guid.NewGuid();

            var email = new BinaryEmailData(applicationId);

            Assert.AreEqual<Guid>(applicationId, email.ApplicationId);
        }

        [TestMethod]
        public void Sender()
        {
            string sender = StringHelper.ValidString();
            var email = new BinaryEmailData()
            {
                Sender = sender
            };

            Assert.AreEqual<string>(sender, email.Sender);
        }

        [TestMethod]
        public void Recipient()
        {
            string recipient = StringHelper.ValidString();
            var email = new BinaryEmailData()
            {
                Recipient = recipient
            };

            Assert.AreEqual<string>(recipient, email.Recipient);
        }

        [TestMethod]
        public void RawMessage()
        {
            var random = new Random();
            var rawMessage = new byte[512];
            random.NextBytes(rawMessage);
            var email = new BinaryEmailData()
            {
                RawMessage = rawMessage
            };

            Assert.AreEqual<byte[]>(rawMessage, email.RawMessage);
            Assert.IsTrue(rawMessage.ContentEquals(email.RawMessage));
        }
        #endregion
    }
}