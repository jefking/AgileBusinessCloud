// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethodsTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services
{
    using Abc.Azure;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using LinqToTwitter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using System;
    using System.Linq;

    [TestClass]
    public class ExtensionMethodsTest
    {
        #region Abc.Azure.BinaryBlob<T>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDigestObjectIdInvalid()
        {
            var blob = new BinaryBlob<LogHistory<MessageDisplay>>(CloudStorageAccount.DevelopmentStorageAccount);
            blob.GetDigest<LogHistory<MessageDisplay>>(StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        public void GetDigest()
        {
            var random = new Random();
            var digest = new LogHistory<MessageDisplay>()
            {
                Count = random.Next(),
                GeneratedOn = DateTime.UtcNow,
                MaximumDate = DateTime.UtcNow.AddSeconds(random.Next()),
                MinimumDate = DateTime.UtcNow.AddMinutes(random.Next()),
            };
            var blob = new BinaryBlob<LogHistory<MessageDisplay>>(CloudStorageAccount.DevelopmentStorageAccount);

            var objectId = string.Format("happy{0}doodie", random.Next());
            blob.Save(objectId, digest);

            var item = blob.GetDigest<LogHistory<MessageDisplay>>(objectId);
            Assert.AreEqual<int>(digest.Count, item.Count);
            Assert.AreEqual<DateTime?>(digest.GeneratedOn, item.GeneratedOn);
            Assert.AreEqual<DateTime?>(digest.MaximumDate, item.MaximumDate);
            Assert.AreEqual<DateTime?>(digest.MinimumDate, item.MinimumDate);
        }
        #endregion

        #region Abc.Azure.AzureTable<T>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryQueryNull()
        {
            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.Query<MessageData>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryQueryFromNull()
        {
            var query = new LogQuery();
            query.Fill();
            query.From = null;
            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.Query<MessageData>(query);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryQueryToNull()
        {
            var query = new LogQuery();
            query.Fill();
            query.To = null;
            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.Query<MessageData>(query);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryQueryTopNull()
        {
            var query = new LogQuery();
            query.Fill();
            query.Top = null;
            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.Query<MessageData>(query);
        }

        [TestMethod]
        public void Query()
        {
            var table = new AzureTable<MessageData>(CloudStorageAccount.DevelopmentStorageAccount);
            
            var appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = new MessageData(appId);
                data.Fill();

                table.AddEntity(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                From = DateTime.UtcNow.AddMinutes(-1),
                To = DateTime.UtcNow.AddMilliseconds(1),
                Top = 1,
            };

            var messages = table.Query<MessageData>(query).ToList();
            Assert.IsNotNull(messages, "Message data should not be null.");
            Assert.AreEqual<int>(1, messages.Count(), "1 Message Should be contained.");

            query.Top = 3;

            messages = table.Query<MessageData>(query).ToList();
            Assert.IsNotNull(messages, "Message data should not be null.");
            Assert.AreEqual<int>(3, messages.Count(), "3 Message Should be contained.");
        }
        #endregion

        #region LinqToTwitter.Status
        [TestMethod]
        public void ParseLinksNone()
        {
            var status = new Status()
            {
                Text = StringHelper.NullEmptyWhiteSpace(),
            };

            var links = status.ParseLinks();
            Assert.IsNotNull(links);
            Assert.AreEqual<int>(0, links.Count());
        }

        [TestMethod]
        public void ParseLinks()
        {
            var status = new Status()
            {
                Text = "hahaha http://www.google.com is such a good site!.",
            };

            var links = status.ParseLinks();
            Assert.IsNotNull(links);
            Assert.AreEqual<int>(1, links.Count());
            Assert.AreEqual<string>("http://www.google.com", links.First());
        }

        [TestMethod]
        public void ParseMentionsNone()
        {
            var status = new Status()
            {
                Text = StringHelper.NullEmptyWhiteSpace(),
            };

            var links = status.ParseMentions();
            Assert.IsNotNull(links);
            Assert.AreEqual<int>(0, links.Count());
        }

        [TestMethod]
        public void ParseMentions()
        {
            var status = new Status()
            {
                Text = "hahaha @GoodDude is such a good site!.",
            };

            var data = status.ParseMentions();
            Assert.IsNotNull(data);
            Assert.AreEqual<int>(1, data.Count());
            Assert.AreEqual<string>("GoodDude", data.First());
        }
        #endregion
    }
}