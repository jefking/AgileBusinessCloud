// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BinaryEmailTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BinaryEmailTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertNullToken()
        {
            var email = new BinaryEmail();
            email.Convert();
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BinaryEmail();
        }

        [TestMethod]
        public void TokenGetSet()
        {
            var token = new Token();
            token.ApplicationId = Guid.NewGuid();
            token.ValidationKey = StringHelper.ValidString();

            var email = new BinaryEmail()
            {
                Token = token
            };

            Assert.AreEqual<Token>(token, email.Token);
            Assert.AreEqual<Guid>(token.ApplicationId, email.Token.ApplicationId);
            Assert.AreEqual<string>(token.ValidationKey, email.Token.ValidationKey);
        }

        [TestMethod]
        public void Sender()
        {
            string sender = StringHelper.ValidString();
            var email = new BinaryEmail()
            {
                Sender = sender
            };

            Assert.AreEqual<string>(sender, email.Sender);
        }

        [TestMethod]
        public void Recipient()
        {
            string recipient = StringHelper.ValidString();
            var email = new BinaryEmail()
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
            var email = new BinaryEmail()
            {
                RawMessage = rawMessage
            };

            Assert.AreEqual<byte[]>(rawMessage, email.RawMessage);
            Assert.IsTrue(rawMessage.ContentEquals(email.RawMessage));
        }

        [TestMethod]
        public void Convert()
        {
            var token = new Token();
            token.ApplicationId = Guid.NewGuid();
            token.ValidationKey = StringHelper.ValidString();
            var random = new Random();
            var rawMessage = new byte[512];
            random.NextBytes(rawMessage);

            var email = new BinaryEmail()
            {
                Token = token,
                RawMessage = rawMessage,
                Recipient = StringHelper.ValidString(),
                Sender = StringHelper.ValidString(),
            };

            var data = email.Convert();
            Assert.AreEqual<Guid>(email.Token.ApplicationId, data.ApplicationId);
            Assert.AreEqual<string>(email.Sender, data.Sender);
            Assert.AreEqual<string>(email.Recipient, data.Recipient);
            Assert.AreEqual<byte[]>(email.RawMessage, data.RawMessage);
            Assert.IsTrue(email.RawMessage.ContentEquals(data.RawMessage));
        }

        [TestMethod]
        public void Valid()
        {
            var random = new Random();
            var rawMessage = new byte[512];
            random.NextBytes(rawMessage);
            var email = new BinaryEmail()
            {
                RawMessage = rawMessage,
            };

            var validator = new Validator<BinaryEmail>();
            Assert.IsTrue(validator.IsValid(email));
        }

        [TestMethod]
        public void InValidRawMessage()
        {
            var email = new BinaryEmail()
            {
                RawMessage = null,
            };

            var validator = new Validator<BinaryEmail>();
            Assert.IsFalse(validator.IsValid(email));
        }
        #endregion
    }
}