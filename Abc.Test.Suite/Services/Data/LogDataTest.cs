// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LogDataTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorApplicationIdentifierInvalid()
        {
            new OccurrenceData(Guid.Empty);
        }
        #endregion

        #region Valid Cases
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
        public void MachineName()
        {
            var item = new ErrorData();
            var data = StringHelper.ValidString();
            item.MachineName = data;
            Assert.AreEqual<string>(data, item.MachineName);
        }

        [TestMethod]
        public void DeploymentId()
        {
            var item = new ErrorData();
            var data = StringHelper.ValidString();
            item.DeploymentId = data;
            Assert.AreEqual<string>(data, item.DeploymentId);
        }

        [TestMethod]
        public void Message()
        {
            var item = new MessageData();
            var data = StringHelper.ValidString();
            item.Message = data;
            Assert.AreEqual<string>(data, item.Message);
        }

        [TestMethod]
        public void OccurredOn()
        {
            var item = new MessageData();
            var data = DateTime.UtcNow;
            item.OccurredOn = data;
            Assert.AreEqual<DateTime>(data, item.OccurredOn);
        }
        #endregion
    }
}