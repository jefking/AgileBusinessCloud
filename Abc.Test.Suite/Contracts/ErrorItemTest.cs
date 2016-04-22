// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ErrorItemTest.cs'>
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
    public class ErrorItemTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ErrorItem();
        }

        [TestMethod]
        public void Parent()
        {
            var item = new ErrorItem();
            var data = new ErrorItem();
            item.Parent = data;
            Assert.AreEqual<ErrorItem>(data, item.Parent);
        }

        [TestMethod]
        public void Source()
        {
            var item = new ErrorItem();
            var data = StringHelper.ValidString();
            item.Source = data;
            Assert.AreEqual<string>(data, item.Source);
        }

        [TestMethod]
        public void StackTrace()
        {
            var item = new ErrorItem();
            var data = StringHelper.ValidString();
            item.StackTrace = data;
            Assert.AreEqual<string>(data, item.StackTrace);
        }

        [TestMethod]
        public void ClassName()
        {
            var item = new ErrorItem();
            var data = StringHelper.ValidString();
            item.ClassName = data;
            Assert.AreEqual<string>(data, item.ClassName);
        }

        [TestMethod]
        public void ErrorCode()
        {
            var random = new Random();
            var item = new ErrorItem();
            var data = random.Next();
            item.ErrorCode = data;
            Assert.AreEqual<int>(data, item.ErrorCode);
        }

        [TestMethod]
        public void EventType()
        {
            var item = new ErrorItem();
            var data = EventTypes.Stop;
            item.EventType = data;
            Assert.AreEqual<EventTypes>(data, item.EventType);
        }

        [TestMethod]
        public void SessionIdentifier()
        {
            var data = new ErrorItem();
            Assert.IsNull(data.SessionIdentifier);
            var session = Guid.NewGuid();
            data.SessionIdentifier = session;
            Assert.AreEqual<Guid?>(session, data.SessionIdentifier);
        }

        [TestMethod]
        public void Valid()
        {
            var item = new ErrorItem()
            {
                Message = StringHelper.ValidString(),
                ClassName = StringHelper.ValidString(),
                SessionIdentifier = null,
            };

            var validator = new Validator<ErrorItem>();
            Assert.IsTrue(validator.IsValid(item));
        }

        [TestMethod]
        public void ValidSessionIdentifier()
        {
            var random = new Random();
            var occurrence = new ErrorItem()
            {
                Message = StringHelper.ValidString(),
                ClassName = StringHelper.ValidString(),
                SessionIdentifier = Guid.NewGuid(),
            };

            var validator = new Validator<ErrorItem>();
            Assert.IsTrue(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void InvalidSessionIdentifier()
        {
            var random = new Random();
            var occurrence = new ErrorItem()
            {
                Message = StringHelper.ValidString(),
                ClassName = StringHelper.ValidString(),
                SessionIdentifier = Guid.Empty,
            };

            var validator = new Validator<ErrorItem>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void MessageInvalid()
        {
            var item = new ErrorItem()
            {
                Message = StringHelper.NullEmptyWhiteSpace(),
                ClassName = StringHelper.ValidString(),
            };

            var validator = new Validator<ErrorItem>();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void MessageTooLong()
        {
            var item = new ErrorItem()
            {
                Message = StringHelper.LongerThanMaximumRowLength(),
                ClassName = StringHelper.ValidString(),
            };

            var validator = new Validator<ErrorItem>();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void ClassNameInvalid()
        {
            var item = new ErrorItem()
            {
                Message = StringHelper.ValidString(),
                ClassName = StringHelper.NullEmptyWhiteSpace(),
            };

            var validator = new Validator<ErrorItem>();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void ClassNameTooLong()
        {
            var item = new ErrorItem()
            {
                Message = StringHelper.ValidString(),
                ClassName = StringHelper.LongerThanMaximumRowLength(),
            };

            var validator = new Validator<ErrorItem>();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };

            var error = new ErrorItem()
            {
                ClassName = StringHelper.ValidString(),
                DeploymentId = StringHelper.ValidString(),
                ErrorCode = random.Next(),
                EventType = EventTypes.Verbose,
                MachineName = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                Source = StringHelper.ValidString(),
                StackTrace = StringHelper.ValidString(),
                Token = token,
                SessionIdentifier = Guid.NewGuid(),
            };

            var converted = error.Convert();
            Assert.AreEqual<Guid>(error.Token.ApplicationId, converted.ApplicationId);
            Assert.AreEqual<DateTime>(error.OccurredOn, converted.OccurredOn);
            Assert.AreEqual<string>(error.MachineName, converted.MachineName);
            Assert.AreEqual<string>(error.DeploymentId, converted.DeploymentId);
            Assert.AreEqual<string>(error.Message, converted.Message);
            Assert.AreEqual<string>(error.Source, converted.Source);
            Assert.AreEqual<string>(error.StackTrace, converted.StackTrace);
            Assert.AreEqual<int>(error.ErrorCode, converted.ErrorCode);
            Assert.AreEqual<string>(error.ClassName, converted.ClassName);
            Assert.AreEqual<int>((int)error.EventType, converted.EventTypeValue);
            Assert.AreEqual<Guid?>(error.SessionIdentifier, converted.SessionIdentifier);
        }
        #endregion
    }
}
