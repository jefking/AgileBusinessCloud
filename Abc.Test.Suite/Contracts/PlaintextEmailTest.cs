// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PlainTextEmailTest.cs'>
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
    public class PlaintextEmailTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertNullToken()
        {
            var email = new PlaintextEmail();
            email.Convert();
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new PlaintextEmail();
        }

        [TestMethod]
        public void TokenGetSet()
        {
            var token = new Token();
            token.ApplicationId = Guid.NewGuid();
            token.ValidationKey = StringHelper.ValidString();

            var email = new PlaintextEmail()
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
            var email = new PlaintextEmail()
            {
                Sender = sender
            };

            Assert.AreEqual<string>(sender, email.Sender);
        }

        [TestMethod]
        public void Recipient()
        {
            string recipient = StringHelper.ValidString();
            var email = new PlaintextEmail()
            {
                Recipient = recipient
            };

            Assert.AreEqual<string>(recipient, email.Recipient);
        }

        [TestMethod]
        public void Subject()
        {
            string subject = StringHelper.ValidString();
            var email = new PlaintextEmail()
            {
                Subject = subject
            };

            Assert.AreEqual<string>(subject, email.Subject);
        }

        [TestMethod]
        public void Message()
        {
            string message = StringHelper.ValidString();
            var email = new PlaintextEmail()
            {
                Message = message
            };

            Assert.AreEqual<string>(message, email.Message);
        }

        [TestMethod]
        public void Convert()
        {
            var token = new Token();
            token.ApplicationId = Guid.NewGuid();
            token.ValidationKey = StringHelper.ValidString();
            var email = new PlaintextEmail()
            {
                Token = token,
                Message = StringHelper.ValidString(),
                Subject = StringHelper.ValidString(),
                Recipient = StringHelper.ValidString(),
                Sender = StringHelper.ValidString(),
            };

            var data = email.Convert();
            Assert.AreEqual<Guid>(email.Token.ApplicationId, data.ApplicationId);
            Assert.AreEqual<string>(email.Sender, data.Sender);
            Assert.AreEqual<string>(email.Recipient, data.Recipient);
            Assert.AreEqual<string>(email.Subject, data.Subject);
            Assert.AreEqual<string>(email.Message, data.Message);
        }

        [TestMethod]
        public void Valid()
        {
            var email = new PlaintextEmail()
            {
                Subject = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
            };

            var validator = new Validator<PlaintextEmail>();
            Assert.IsTrue(validator.IsValid(email));
        }

        [TestMethod]
        public void SubjectTooLong()
        {
            var email = new PlaintextEmail()
            {
                Subject = StringHelper.LongerThanMaximumRowLength(),
                Message = StringHelper.ValidString(),
            };

            var validator = new Validator<PlaintextEmail>();
            Assert.IsFalse(validator.IsValid(email));
        }

        [TestMethod]
        public void SubjectInvalid()
        {
            var email = new PlaintextEmail()
            {
                Subject = StringHelper.NullEmptyWhiteSpace(),
                Message = StringHelper.ValidString(),
            };

            var validator = new Validator<PlaintextEmail>();
            Assert.IsFalse(validator.IsValid(email));
        }

        [TestMethod]
        public void MessageTooLong()
        {
            var email = new PlaintextEmail()
            {
                Subject = StringHelper.ValidString(),
                Message = StringHelper.LongerThanMaximumRowLength(),
            };

            var validator = new Validator<PlaintextEmail>();
            Assert.IsFalse(validator.IsValid(email));
        }

        [TestMethod]
        public void MessageInvalid()
        {
            var email = new PlaintextEmail()
            {
                Subject = StringHelper.ValidString(),
                Message = StringHelper.NullEmptyWhiteSpace(),
            };

            var validator = new Validator<PlaintextEmail>();
            Assert.IsFalse(validator.IsValid(email));
        }
        #endregion
    }
}
