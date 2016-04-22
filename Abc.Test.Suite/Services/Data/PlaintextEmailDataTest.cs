// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PlaintextEmailDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PlaintextEmailDataTest
    {
        #region Error Cases
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new PlaintextEmailData();
        }

        [TestMethod]
        public void ApplicationId()
        {
            Guid applicationId = Guid.NewGuid();

            var email = new PlaintextEmailData(applicationId);

            Assert.AreEqual<Guid>(applicationId, email.ApplicationId);
        }

        [TestMethod]
        public void RowKey()
        {
            var email = new PlaintextEmailData(Guid.NewGuid());
            Assert.IsFalse(string.IsNullOrWhiteSpace(email.RowKey));
        }

        [TestMethod]
        public void PartitionKey()
        {
            Guid applicationId = Guid.NewGuid();

            var email = new PlaintextEmailData(applicationId);

            Assert.AreEqual<string>(applicationId.ToString(), email.PartitionKey);
        }

        [TestMethod]
        public void Sender()
        {
            string sender = StringHelper.ValidString();
            var email = new PlaintextEmailData()
            {
                Sender = sender
            };

            Assert.AreEqual<string>(sender, email.Sender);
        }

        [TestMethod]
        public void Recipient()
        {
            string recipient = StringHelper.ValidString();
            var email = new PlaintextEmailData()
            {
                Recipient = recipient
            };

            Assert.AreEqual<string>(recipient, email.Recipient);
        }

        [TestMethod]
        public void Subject()
        {
            string subject = StringHelper.ValidString();
            var email = new PlaintextEmailData()
            {
                Subject = subject
            };

            Assert.AreEqual<string>(subject, email.Subject);
        }

        [TestMethod]
        public void Message()
        {
            string message = StringHelper.ValidString();
            var email = new PlaintextEmailData()
            {
                Message = message
            };

            Assert.AreEqual<string>(message, email.Message);
        }
        #endregion
    }
}