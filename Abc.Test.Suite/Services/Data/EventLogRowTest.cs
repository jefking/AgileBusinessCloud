// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EventLogRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EventLogRowTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new EventLogRow();
        }

        [TestMethod]
        public void ConstructorApplicationIdentifier()
        {
            var item = new EventLogRow(Guid.NewGuid());
        }

        [TestMethod]
        public void Source()
        {
            var item = new EventLogRow(Guid.NewGuid());
            var data = StringHelper.ValidString();
            item.Source = data;
            Assert.AreEqual<string>(data, item.Source);
        }

        [TestMethod]
        public void User()
        {
            var item = new EventLogRow(Guid.NewGuid());
            var data = StringHelper.ValidString();
            item.User = data;
            Assert.AreEqual<string>(data, item.User);
        }

        [TestMethod]
        public void EventId()
        {
            var random = new Random();
            var item = new EventLogRow(Guid.NewGuid());
            Assert.IsNull(item.EventId);
            var data = random.Next();
            item.EventId = data;
            Assert.AreEqual<int?>(data, item.EventId);
        }

        [TestMethod]
        public void InstanceId()
        {
            var random = new Random();
            var item = new EventLogRow(Guid.NewGuid());
            Assert.IsNull(item.InstanceId);
            var data = random.Next();
            item.InstanceId = data;
            Assert.AreEqual<long?>(data, item.InstanceId);
        }

        [TestMethod]
        public void EventLogEntryTypeValue()
        {
            var random = new Random();
            var item = new EventLogRow(Guid.NewGuid());
            Assert.AreEqual<int>(0, item.EntryTypeValue);
            var data = random.Next();
            item.EntryTypeValue = data;
            Assert.AreEqual<int>(data, item.EntryTypeValue);
        }
        #endregion
    }
}
