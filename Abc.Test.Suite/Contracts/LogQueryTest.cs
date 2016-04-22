// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogQueryTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;
    using Abc.Azure;
    using Abc.Services;

    [TestClass]
    public class LogQueryTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterListNull()
        {
            var query = new LogQuery();
            query.Filter<MessageDisplay>(null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new LogQuery();
        }

        [TestMethod]
        public void ApplicationIdentifier()
        {
            var query = new LogQuery();
            var data = Guid.NewGuid();
            query.ApplicationIdentifier = data;
            Assert.AreEqual<Guid>(data, query.ApplicationIdentifier);
        }

        [TestMethod]
        public void PartitionKey()
        {
            var query = new LogQuery();
            var data = Guid.NewGuid();
            query.ApplicationIdentifier = data;
            Assert.AreEqual<string>(data.ToString(), query.PartitionKey);
        }

        [TestMethod]
        public void RowKey()
        {
            var query = new LogQuery();
            Assert.IsNull(query.RowKey);
            var data = Guid.NewGuid();
            query.Identifier = data;
            Assert.AreEqual<string>(data.ToString(), query.RowKey);
        }

        [TestMethod]
        public void Deep()
        {
            var query = new LogQuery();
            Assert.IsNull(query.Deep);
            query.Deep = true;
            Assert.IsTrue(query.Deep.Value);
        }

        [TestMethod]
        public void Top()
        {
            var random = new Random();
            var query = new LogQuery();
            Assert.IsNull(query.Top);
            var data = random.Next();
            query.Top = data;
            Assert.AreEqual<int?>(data, query.Top);
        }

        [TestMethod]
        public void To()
        {
            var query = new LogQuery();
            Assert.IsNull(query.To);
            var data = DateTime.UtcNow;
            query.To = data;
            Assert.AreEqual<DateTime?>(data, query.To);
        }

        [TestMethod]
        public void From()
        {
            var query = new LogQuery();
            Assert.IsNull(query.From);
            var data = DateTime.UtcNow;
            query.From = data;
            Assert.AreEqual<DateTime?>(data, query.From);
        }

        [TestMethod]
        public void Identifier()
        {
            var query = new LogQuery();
            Assert.IsNull(query.Identifier);
            var data = Guid.NewGuid();
            query.Identifier = data;
            Assert.AreEqual<Guid?>(data, query.Identifier);
        }

        [TestMethod]
        public void IsUnique()
        {
            var query = new LogQuery();
            Assert.IsFalse(query.IsUnique);
            query.Identifier = Guid.NewGuid();
            Assert.IsTrue(query.IsUnique);
        }

        [TestMethod]
        public void Initialize()
        {
            var query = new LogQuery();
            query.Initialize();
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, query.To.Value.Date);
            Assert.AreEqual<DateTime>(AzureTable<MessageData>.Minimum, query.From.Value);
            Assert.AreEqual<int>(1000, query.Top.Value);
        }

        [TestMethod]
        public void FilterFrom()
        {
            var list = new List<MessageDisplay>();
            var msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2000, 01, 01);
            list.Add(msg);

            msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2012, 01, 01);
            list.Add(msg);

            var query = new LogQuery();
            query.Initialize();
            query.From = new DateTime(2005, 01, 01);
            var filtered = query.Filter<MessageDisplay>(list);
            Assert.AreEqual<int>(1, filtered.Count());
            var item = filtered.First();
            Assert.AreEqual<Guid>(msg.Identifier, item.Identifier);
        }

        [TestMethod]
        public void FilterTo()
        {
            var list = new List<MessageDisplay>();
            var msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2012, 01, 01);
            list.Add(msg);

            msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2000, 01, 01);
            list.Add(msg);

            var query = new LogQuery();
            query.Initialize();
            query.From = new DateTime(2005, 01, 01);
            var filtered = query.Filter<MessageDisplay>(list);
            Assert.AreEqual<int>(1, filtered.Count());
            var item = filtered.First();
            Assert.AreEqual<Guid>(msg.Identifier, item.Identifier);
        }

        [TestMethod]
        public void FilterOrder()
        {
            var list = new List<MessageDisplay>();
            var msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2012, 01, 01);
            list.Add(msg);

            msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2000, 01, 01);
            list.Add(msg);

            var query = new LogQuery();
            query.Initialize();
            var filtered = query.Filter<MessageDisplay>(list);
            Assert.AreEqual<int>(2, filtered.Count());
            var first = filtered.First();
            var last = filtered.Last();
            Assert.IsTrue(first.OccurredOn > last.OccurredOn);
        }

        [TestMethod]
        public void FilterTop()
        {
            var list = new List<MessageDisplay>();
            var msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2000, 01, 01);
            list.Add(msg);

            msg = new MessageDisplay();
            msg.Fill();
            msg.OccurredOn = new DateTime(2012, 01, 01);
            list.Add(msg);

            var query = new LogQuery();
            query.Initialize();
            query.Top = 1;
            var filtered = query.Filter<MessageDisplay>(list);
            Assert.AreEqual<int>(1, filtered.Count());
            var item = filtered.First();
            Assert.AreEqual<Guid>(msg.Identifier, item.Identifier);
        }
        #endregion
    }
}