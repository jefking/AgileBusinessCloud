// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogItemTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LogItemTest
    {
        #region Valid Cases
        [TestMethod]
        public void OccurredOn()
        {
            var data = DateTime.UtcNow;
            var logItem = (LogItem)new Message();
            logItem.OccurredOn = data;
            Assert.AreEqual<DateTime>(data, logItem.OccurredOn);
        }

        [TestMethod]
        public void MachineName()
        {
            var data = StringHelper.ValidString();
            var logItem = (LogItem)new ErrorItem();
            logItem.MachineName = data;
            Assert.AreEqual<string>(data, logItem.MachineName);
        }

        [TestMethod]
        public void DeploymentId()
        {
            var data = StringHelper.ValidString();
            var logItem = (LogItem)new Occurrence();
            logItem.DeploymentId = data;
            Assert.AreEqual<string>(data, logItem.DeploymentId);
        }

        [TestMethod]
        public void Message()
        {
            var data = StringHelper.ValidString();
            var logItem = (LogItem)new Message();
            logItem.Message = data;
            Assert.AreEqual<string>(data, logItem.Message);
        }
        #endregion
    }
}