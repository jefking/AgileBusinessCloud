// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ThroughputTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ThroughputTest
    {
        #region Valid Cases
        [TestMethod]
        public void Exceptions()
        {
            var random = new Random();
            var data = new Throughput();
            var expected = random.Next();
            data.Exceptions = expected;
            Assert.AreEqual<int>(expected, data.Exceptions);
        }

        [TestMethod]
        public void Messages()
        {
            var random = new Random();
            var data = new Throughput();
            var expected = random.Next();
            data.Messages = expected;
            Assert.AreEqual<int>(expected, data.Messages);
        }

        [TestMethod]
        public void Performance()
        {
            var random = new Random();
            var data = new Throughput();
            var expected = random.Next();
            data.Performance = expected;
            Assert.AreEqual<int>(expected, data.Performance);
        }

        [TestMethod]
        public void EventLog()
        {
            var random = new Random();
            var data = new Throughput();
            var expected = random.Next();
            data.EventLog = expected;
            Assert.AreEqual<int>(expected, data.EventLog);
        }

        [TestMethod]
        public void ServerStatistics()
        {
            var random = new Random();
            var data = new Throughput();
            var expected = random.Next();
            data.ServerStatistics = expected;
            Assert.AreEqual<int>(expected, data.ServerStatistics);
        }

        [TestMethod]
        public void Constructor()
        {
            new Throughput();
        }
        #endregion
    }
}