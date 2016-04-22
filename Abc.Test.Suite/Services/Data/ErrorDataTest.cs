// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ErrorDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Abc.Services.Contracts;

    [TestClass]
    public class ErrorDataTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorAppIdEmpty()
        {
            new ErrorData(Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ErrorData();
        }

        [TestMethod]
        public void ConstructorApplicationId()
        {
            new ErrorData(Guid.NewGuid());
        }

        [TestMethod]
        public void IsLogData()
        {
            Assert.IsNotNull(new ErrorData() as LogData);
        }

        [TestMethod]
        public void IsIConvertErrorDisplay()
        {
            Assert.IsNotNull(new ErrorData() as IConvert<ErrorDisplay>);
        }

        [TestMethod]
        public void ParentId()
        {
            var data = new ErrorData();
            var test = Guid.NewGuid();
            data.ParentId = test;
            Assert.AreEqual<Guid>(test, data.ParentId);
        }

        [TestMethod]
        public void Source()
        {
            var data = new ErrorData();
            var test = StringHelper.ValidString();
            data.Source = test;
            Assert.AreEqual<string>(test, data.Source);
        }

        [TestMethod]
        public void StackTrace()
        {
            var data = new ErrorData();
            var test = StringHelper.ValidString();
            data.StackTrace = test;
            Assert.AreEqual<string>(test, data.StackTrace);
        }

        [TestMethod]
        public void ClassName()
        {
            var data = new ErrorData();
            var test = StringHelper.ValidString();
            data.ClassName = test;
            Assert.AreEqual<string>(test, data.ClassName);
        }

        [TestMethod]
        public void ErrorCode()
        {
            var random = new Random();
            var data = new ErrorData();
            var test = random.Next();
            data.ErrorCode = test;
            Assert.AreEqual<int>(test, data.ErrorCode);
        }

        [TestMethod]
        public void EventTypeValue()
        {
            var data = new ErrorData();
            var test = 2;
            data.EventTypeValue = test;
            Assert.AreEqual<int>(test, data.EventTypeValue);
        }

        [TestMethod]
        public void SessionIdentifier()
        {
            var data = new ErrorData();
            Assert.IsNull(data.SessionIdentifier);
            var session = Guid.NewGuid();
            data.SessionIdentifier = session;
            Assert.AreEqual<Guid?>(session, data.SessionIdentifier);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var data = new ErrorData(Guid.NewGuid())
            {
                ClassName = StringHelper.ValidString(),
                DeploymentId = StringHelper.ValidString(),
                ErrorCode = random.Next(),
                EventTypeValue = 2,
                MachineName = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                ParentId = Guid.NewGuid(),
                Source = StringHelper.ValidString(),
                StackTrace = StringHelper.ValidString(),
                SessionIdentifier = Guid.NewGuid(),
            };

            var converted = data.Convert();
            Assert.AreEqual<string>(data.ClassName, converted.ClassName);
            Assert.AreEqual<string>(data.DeploymentId, converted.DeploymentId);
            Assert.AreEqual<int>(data.ErrorCode, converted.ErrorCode);
            Assert.AreEqual<int>(data.EventTypeValue, (int)converted.EventType);
            Assert.AreEqual<string>(data.MachineName, converted.MachineName);
            Assert.AreEqual<string>(data.Message, converted.Message);
            Assert.AreEqual<DateTime>(data.OccurredOn, converted.OccurredOn);
            Assert.AreEqual<string>(data.Source, converted.Source);
            Assert.AreEqual<string>(data.StackTrace, converted.StackTrace);
            Assert.AreEqual<Guid>(data.ApplicationId, converted.Token.ApplicationId);
            Assert.AreEqual<Guid>(data.ParentId, converted.ParentIdentifier);
            Assert.AreEqual<Guid>(Guid.Parse(data.RowKey), converted.Identifier);
            Assert.AreEqual<Guid?>(data.SessionIdentifier, converted.SessionIdentifier);
        }
        #endregion
    }
}