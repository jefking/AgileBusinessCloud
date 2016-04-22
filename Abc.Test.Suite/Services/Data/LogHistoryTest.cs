// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogHistoryTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using System.Collections.Generic;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LogHistoryTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new LogHistory<MessageDisplay>();
        }

        [TestMethod]
        public void Messages()
        {
            var msg = new LogHistory<MessageDisplay>();
            var data = new List<MessageDisplay>().ToArray();
            msg.Items = data;
            Assert.AreEqual<MessageDisplay[]>(data, msg.Items);
        }

        [TestMethod]
        public void GeneratedOn()
        {
            var msg = new LogHistory<MessageDisplay>();
            Assert.IsNull(msg.GeneratedOn);
            var data = DateTime.UtcNow;
            msg.GeneratedOn = data;
            Assert.AreEqual<DateTime?>(data, msg.GeneratedOn);
        }

        [TestMethod]
        public void MaximumDate()
        {
            var msg = new LogHistory<MessageDisplay>();
            Assert.IsNull(msg.MaximumDate);
            var data = DateTime.UtcNow;
            msg.MaximumDate = data;
            Assert.AreEqual<DateTime?>(data, msg.MaximumDate);
        }

        [TestMethod]
        public void MinimumDate()
        {
            var msg = new LogHistory<MessageDisplay>();
            Assert.IsNull(msg.MinimumDate);
            var data = DateTime.UtcNow;
            msg.MinimumDate = data;
            Assert.AreEqual<DateTime?>(data, msg.MinimumDate);
        }

        [TestMethod]
        public void Count()
        {
            var random = new Random();
            var msg = new LogHistory<MessageDisplay>();
            var data = random.Next();
            msg.Count = data;
            Assert.AreEqual<int>(data, msg.Count);
        }
        #endregion
    }
}