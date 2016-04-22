// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EmailCoreTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class EmailCoreTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendBinaryNull()
        {
            var core = new EmailCore();
            core.Send((BinaryEmail)null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendBinaryInvalidSender()
        {
            var core = new EmailCore();
            var email = Binary();
            email.Sender = StringHelper.NullEmptyWhiteSpace();
            core.Send(email);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendBinaryInvalidRecipient()
        {
            var core = new EmailCore();
            var email = Binary();
            email.Recipient = StringHelper.NullEmptyWhiteSpace();
            core.Send(email);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendBinaryNullRawMessage()
        {
            var core = new EmailCore();
            var email = Binary();
            email.RawMessage = null;
            core.Send(email);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendPlainNull()
        {
            var core = new EmailCore();
            core.Send((PlaintextEmail)null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendPlainInvalidSender()
        {
            var core = new EmailCore();
            var email = Plaintext();
            email.Sender = StringHelper.NullEmptyWhiteSpace();
            core.Send(email);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendPlainInvalidRecipient()
        {
            var core = new EmailCore();
            var email = Plaintext();
            email.Recipient = StringHelper.NullEmptyWhiteSpace();
            core.Send(email);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendPlainInvalidSubject()
        {
            var core = new EmailCore();
            var email = Plaintext();
            email.Subject = StringHelper.NullEmptyWhiteSpace();
            core.Send(email);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendPlainInvalidMessage()
        {
            var core = new EmailCore();
            var email = Plaintext();
            email.Message = StringHelper.NullEmptyWhiteSpace();
            core.Send(email);
        }
        #endregion

        #region Initialize
        [TestInitialize]
        public void Init()
        {
            var binaryEmail = new AzureTable<BinaryEmailData>(CloudStorageAccount.DevelopmentStorageAccount);
            binaryEmail.EnsureExist();

            var plaintextEmail = new AzureTable<PlaintextEmailData>(CloudStorageAccount.DevelopmentStorageAccount);
            plaintextEmail.EnsureExist();
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new EmailCore();
        }

        [TestMethod]
        public void SendBinaryEmail()
        {
            string address = "noreply@agilebusinesscloud.com";
            var email = Binary();
            email.Sender = address;
            email.Recipient = address;
            var core = new EmailCore();
            core.Send(email);
        }

        [TestMethod]
        public void SendPlaintextEmail()
        {
            string address = "noreply@agilebusinesscloud.com";
            var email = Plaintext();
            email.Sender = address;
            email.Recipient = address;
            var core = new EmailCore();
            core.Send(email);
        }

        [TestMethod]
        public void SendBinaryEmailStorage()
        {
            string address = "noreply@agilebusinesscloud.com";
            var email = Binary();
            email.Sender = address;
            email.Recipient = address;
            var core = new EmailCore();
            var id = core.Send(email);

            var table = new AzureTable<BinaryEmailData>(CloudStorageAccount.DevelopmentStorageAccount);
            var data = table.QueryBy(email.Token.ApplicationId.ToString(), id.ToString());

            Assert.AreEqual<Guid>(email.Token.ApplicationId, data.ApplicationId);
            Assert.AreEqual<string>(email.Sender, data.Sender);
            Assert.AreEqual<string>(email.Recipient, data.Recipient);
            Assert.IsTrue(email.RawMessage.ContentEquals(data.RawMessage));
        }

        [TestMethod]
        public void SendPlaintextEmailStorage()
        {
            string address = "noreply@agilebusinesscloud.com";
            var email = Plaintext();
            email.Sender = address;
            email.Recipient = address;
            var core = new EmailCore();
            var id = core.Send(email);

            var table = new AzureTable<PlaintextEmailData>(CloudStorageAccount.DevelopmentStorageAccount);
            var data = table.QueryBy(email.Token.ApplicationId.ToString(), id.ToString());

            Assert.AreEqual<Guid>(email.Token.ApplicationId, data.ApplicationId);
            Assert.AreEqual<string>(email.Sender, data.Sender);
            Assert.AreEqual<string>(email.Recipient, data.Recipient);
            Assert.AreEqual<string>(email.Subject, data.Subject);
            Assert.AreEqual<string>(email.Message, data.Message);
        }
        #endregion

        #region Helper Methods
        private static PlaintextEmail Plaintext()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
                ValidationKey = StringHelper.ValidString(),
            };

            return new PlaintextEmail()
            {
                Token = token,
                Sender = StringHelper.ValidString(),
                Recipient = StringHelper.ValidString(),
                Subject = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
            };
        }

        private static BinaryEmail Binary()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
                ValidationKey = StringHelper.ValidString(),
            };

            var random = new Random();
            var rawMessage = new byte[128];
            random.NextBytes(rawMessage);
            return new BinaryEmail()
            {
                Token = token,
                Sender = StringHelper.ValidString(),
                Recipient = StringHelper.ValidString(),
                RawMessage = rawMessage,
            };
        }
        #endregion
    }
}