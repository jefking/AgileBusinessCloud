// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='OccurrenceDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OccurrenceDataTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new OccurrenceData();
        }

        [TestMethod]
        public void ConstructorApplicationId()
        {
            new OccurrenceData(Guid.NewGuid());
        }

        [TestMethod]
        public void ClassName()
        {
            var data = new OccurrenceData();
            var className = StringHelper.ValidString();
            data.ClassName = className;
            Assert.AreEqual<string>(className, data.ClassName);
        }

        [TestMethod]
        public void SessionIdentifier()
        {
            var data = new OccurrenceData();
            Assert.IsNull(data.SessionIdentifier);
            var session = Guid.NewGuid();
            data.SessionIdentifier = session;
            Assert.AreEqual<Guid?>(session, data.SessionIdentifier);
        }

        [TestMethod]
        public void Duration()
        {
            var data = new OccurrenceData();
            var random = new Random();
            var test = random.Next();
            data.Duration = test;
            Assert.AreEqual<long>(test, data.Duration);
        }

        [TestMethod]
        public void MethodName()
        {
            var data = new OccurrenceData();
            var test = StringHelper.ValidString();
            data.MethodName = test;
            Assert.AreEqual<string>(test, data.MethodName);
        }

        [TestMethod]
        public void ThreadId()
        {
            var data = new OccurrenceData();
            var random = new Random();
            var test = random.Next();
            data.ThreadId = test;
            Assert.AreEqual<int>(test, data.ThreadId);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var data = new OccurrenceData(Guid.NewGuid())
            {
                ClassName = StringHelper.ValidString(),
                DeploymentId = StringHelper.ValidString(),
                Duration = random.Next(),
                MachineName = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                MethodName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                ThreadId = random.Next(),
                SessionIdentifier = Guid.NewGuid(),
            };

            var converted = data.Convert();
            Assert.IsNotNull(converted);
            Assert.AreEqual<string>(data.ClassName, converted.Class);
            Assert.AreEqual<string>(data.DeploymentId, converted.DeploymentId);
            Assert.AreEqual<long>(data.Duration, converted.Duration.Ticks);
            Assert.AreEqual<string>(data.MachineName, converted.MachineName);
            Assert.AreEqual<string>(data.Message, converted.Message);
            Assert.AreEqual<string>(data.MethodName, converted.Method);
            Assert.AreEqual<DateTime>(data.OccurredOn, converted.OccurredOn);
            Assert.AreEqual<int>(data.ThreadId, converted.ThreadId);
            Assert.AreEqual<Guid>(data.ApplicationId, converted.Token.ApplicationId);
            Assert.AreEqual<Guid?>(data.SessionIdentifier, converted.SessionIdentifier);
            Assert.AreEqual<Guid>(Guid.Parse(data.RowKey), converted.Identifier);
        }
        #endregion
    }
}