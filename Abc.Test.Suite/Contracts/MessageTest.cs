// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='MessageTest.cs'>
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
    public class MessageTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Message();
        }

        [TestMethod]
        public void SessionIdentifier()
        {
            var data = new Message();
            Assert.IsNull(data.SessionIdentifier);
            var session = Guid.NewGuid();
            data.SessionIdentifier = session;
            Assert.AreEqual<Guid?>(session, data.SessionIdentifier);
        }

        [TestMethod]
        public void Convert()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };
            var message = new Message()
            {
                OccurredOn = DateTime.UtcNow,
                MachineName = StringHelper.ValidString(),
                DeploymentId = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                Token = token,
                SessionIdentifier = Guid.NewGuid(),
            };

            var data = message.Convert();
            Assert.AreEqual<DateTime>(message.OccurredOn, data.OccurredOn);
            Assert.AreEqual<string>(message.MachineName, data.MachineName);
            Assert.AreEqual<string>(message.DeploymentId, data.DeploymentId);
            Assert.AreEqual<string>(message.Message, data.Message);
            Assert.AreEqual<Guid>(message.Token.ApplicationId, data.ApplicationId);
            Assert.AreEqual<Guid?>(message.SessionIdentifier, data.SessionIdentifier);
        }

        [TestMethod]
        public void Valid()
        {
            var message = new Message()
            {
                Message = StringHelper.ValidString(),
            };

            var validator = new Validator<Message>();
            Assert.IsTrue(validator.IsValid(message));
        }

        [TestMethod]
        public void ValidSessionIdentifier()
        {
            var random = new Random();
            var occurrence = new Message()
            {
                Message = StringHelper.ValidString(),
                SessionIdentifier = Guid.NewGuid(),
            };

            var validator = new Validator<Message>();
            Assert.IsTrue(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void InvalidSessionIdentifier()
        {
            var random = new Random();
            var occurrence = new Message()
            {
                Message = StringHelper.ValidString(),
                SessionIdentifier = Guid.Empty,
            };

            var validator = new Validator<Message>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void MessageNotSpecified()
        {
            var message = new Message()
            {
                Message = StringHelper.NullEmptyWhiteSpace(),
            };

            var validator = new Validator<Message>();
            Assert.IsFalse(validator.IsValid(message));
        }

        [TestMethod]
        public void MessageTooLong()
        {
            var message = new Message()
            {
                Message = StringHelper.LongerThanMaximumRowLength(),
            };

            var validator = new Validator<Message>();
            Assert.IsFalse(validator.IsValid(message));
        }
        #endregion
    }
}