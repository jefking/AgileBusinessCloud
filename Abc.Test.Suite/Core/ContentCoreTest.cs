// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ContentCoreTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Linq;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Content Management System Core Test
    /// </summary>
    [TestClass]
    public class ContentCoreTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StoreBlogEntryNull()
        {
            var core = new ContentCore();
            core.Store((BlogEntry)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StoreBlogEntrySectionIdentifierEmpty()
        {
            var core = new ContentCore();
            var entry = new BlogEntry()
            {
                Content = StringHelper.ValidString(),
                Identifier = Guid.NewGuid(),
                PostedOn = DateTime.UtcNow,
                SectionIdentifier = Guid.Empty,
                Title = StringHelper.ValidString(),
            };

            core.Store(entry);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StoreBlogEntryTitleInvalid()
        {
            var core = new ContentCore();
            var entry = new BlogEntry()
            {
                Content = StringHelper.ValidString(),
                Identifier = Guid.NewGuid(),
                PostedOn = DateTime.UtcNow,
                SectionIdentifier = Guid.NewGuid(),
                Title = StringHelper.NullEmptyWhiteSpace(),
            };

            core.Store(entry);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StoreBlogEntryContentInvalid()
        {
            var core = new ContentCore();
            var entry = new BlogEntry()
            {
                Content = StringHelper.NullEmptyWhiteSpace(),
                Identifier = Guid.NewGuid(),
                PostedOn = DateTime.UtcNow,
                SectionIdentifier = Guid.NewGuid(),
                Title = StringHelper.ValidString(),
            };

            core.Store(entry);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBlogEntryNull()
        {
            var core = new ContentCore();
            core.Get((BlogEntry)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetBlogEntrySectionIdentifierEmpty()
        {
            var core = new ContentCore();
            var entry = new BlogEntry()
            {
                Content = StringHelper.ValidString(),
                Identifier = Guid.NewGuid(),
                PostedOn = DateTime.UtcNow,
                SectionIdentifier = Guid.Empty,
                Title = StringHelper.ValidString(),
            };

            core.Get(entry);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ContentCore();
        }

        [TestMethod]
        public void RoundTripTextContent()
        {
            Token token = new Token()
            {
                ApplicationId = Guid.NewGuid()
            };
            TextContent data = new TextContent()
            {
                Active = true,
                Content = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.UtcNow,
                Deleted = true,
                UpdatedOn = DateTime.UtcNow,
                Token = token
            };
            Assert.AreEqual<Guid>(Guid.Empty, data.Id);

            ContentCore core = new ContentCore();
            TextContent returned = core.Store(data);
            Assert.IsNotNull(returned);
            Assert.AreNotEqual<Guid>(Guid.Empty, returned.Id);

            TextContent query = new TextContent()
            {
                Token = token,
                Id = returned.Id
            };

            TextContent filled = core.Get(query);
            Assert.IsNotNull(filled);
            Assert.AreEqual<Guid>(returned.Id, filled.Id);
            Assert.AreEqual<bool>(data.Active, filled.Active, "Data Mismatch");
            Assert.AreEqual<string>(data.Content, filled.Content, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.CreatedOn.Date, filled.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(data.Deleted, filled.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.UpdatedOn.Date, filled.UpdatedOn.Date, "Data Mismatch");

            filled.Active = false;
            filled.Content = Guid.NewGuid().ToString();
            filled.CreatedOn = DateTime.UtcNow;
            filled.Deleted = false;
            filled.UpdatedOn = DateTime.UtcNow;
            filled.Token = token;

            returned = core.Store(filled);
            Assert.IsNotNull(returned);
            Assert.AreEqual<Guid>(filled.Id, returned.Id);

            returned.Token = token;
            TextContent updated = core.Get(returned);
            Assert.IsNotNull(updated);
            Assert.AreEqual<Guid>(filled.Id, updated.Id);
            Assert.AreEqual<bool>(filled.Active, updated.Active, "Data Mismatch");
            Assert.AreEqual<string>(filled.Content, updated.Content, "Data Mismatch");
            Assert.AreEqual<DateTime>(filled.CreatedOn.Date, updated.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(filled.Deleted, updated.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(filled.UpdatedOn.Date, updated.UpdatedOn.Date, "Data Mismatch");
        }

        [TestMethod]
        public void RoundTripXmlContent()
        {
            const string Xml = "<xml>{0}</xml>";
            Token token = new Token()
            {
                ApplicationId = Guid.NewGuid()
            };
            XmlContent data = new XmlContent()
            {
                Active = true,
                Content = string.Format(Xml, Guid.NewGuid()),
                CreatedOn = DateTime.UtcNow,
                Deleted = true,
                UpdatedOn = DateTime.UtcNow,
                Token = token
            };
            Assert.AreEqual<Guid>(Guid.Empty, data.Id);

            ContentCore core = new ContentCore();
            XmlContent returned = core.Store(data);
            Assert.IsNotNull(returned);
            Assert.AreNotEqual<Guid>(Guid.Empty, returned.Id);

            XmlContent query = new XmlContent()
            {
                Token = token,
                Id = returned.Id
            };

            XmlContent filled = core.Get(query);
            Assert.IsNotNull(filled);
            Assert.AreEqual<Guid>(returned.Id, filled.Id);
            Assert.AreEqual<bool>(data.Active, filled.Active, "Data Mismatch");
            Assert.AreEqual<string>(data.Content, filled.Content, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.CreatedOn.Date, filled.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(data.Deleted, filled.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.UpdatedOn.Date, filled.UpdatedOn.Date, "Data Mismatch");

            filled.Active = false;
            filled.Content = string.Format(Xml, Guid.NewGuid());
            filled.CreatedOn = DateTime.UtcNow;
            filled.Deleted = false;
            filled.UpdatedOn = DateTime.UtcNow;
            filled.Token = token;

            returned = core.Store(filled);
            Assert.IsNotNull(returned);
            Assert.AreEqual<Guid>(filled.Id, returned.Id);

            returned.Token = token;
            XmlContent updated = core.Get(returned);
            Assert.IsNotNull(updated);
            Assert.AreEqual<Guid>(filled.Id, updated.Id);
            Assert.AreEqual<bool>(filled.Active, updated.Active, "Data Mismatch");
            Assert.AreEqual<string>(filled.Content, updated.Content, "Data Mismatch");
            Assert.AreEqual<DateTime>(filled.CreatedOn.Date, updated.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(filled.Deleted, updated.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(filled.UpdatedOn.Date, updated.UpdatedOn.Date, "Data Mismatch");
        }

        [TestMethod]
        public void RoundTripBinaryData()
        {
            byte[] rawData = new byte[1024];
            Random random = new Random();
            random.NextBytes(rawData);

            BinaryContent data = new BinaryContent()
            {
                Content = rawData,
                ContentType = "na",
            };
            ContentCore core = new ContentCore();
            var returned = core.Store(data);
            Assert.IsNotNull(returned, "Nothing returned");
            Assert.AreNotEqual<Guid>(Guid.Empty, returned.Id, "Identifier not set.");

            var returnedData = core.Get(returned);
            Assert.IsNotNull(returnedData);
            Assert.AreEqual<int>(data.Content.Length, returnedData.Content.Length, "Data is inconsistant.");

            for (int i = 0; i < data.Content.Length; i++)
            {
                if (data.Content[i] != returnedData.Content[i])
                {
                    Assert.Fail("Data is inconsistant.");
                }
            }
        }

        [TestMethod]
        public void RoundTripBlogEntry()
        {
            var core = new ContentCore();
            var entry = new BlogEntry()
            {
                Content = StringHelper.ValidString(),
                PostedOn = DateTime.UtcNow,
                SectionIdentifier = Guid.NewGuid(),
                Title = StringHelper.ValidString(),
            };

            core.Store(entry);

            var item = core.Get(entry).FirstOrDefault();
            Assert.AreNotEqual<Guid>(Guid.Empty, item.Identifier);
            Assert.AreEqual<Guid>(entry.SectionIdentifier, item.SectionIdentifier);
            Assert.AreEqual<DateTime>(entry.PostedOn.Date, item.PostedOn.Date);
            Assert.AreEqual<string>(entry.Title, item.Title);
            Assert.IsNotNull(item.Content);
        }
        #endregion
    }
}