// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BlogEntryTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BlogEntryTest
    {
        #region Valid Cases
        [TestMethod]
        public void MarkWoodward()
        {
            Assert.AreEqual<Guid>(Guid.Parse("6EC0D941-E6F6-4DCA-8B11-D2C648853583"), BlogEntry.MarkWoodward);
        }

        [TestMethod]
        public void Company()
        {
            Assert.AreEqual<Guid>(Guid.Parse("2CAC744E-3544-4DBA-9167-FB75EA9516DA"), BlogEntry.Company);
        }

        [TestMethod]
        public void JefKing()
        {
            Assert.AreEqual<Guid>(Guid.Parse("42F64C8D-28AC-4306-B699-CBAA11854B55"), BlogEntry.JefKing);
        }

        [TestMethod]
        public void JaimeBueza()
        {
            Assert.AreEqual<Guid>(Guid.Parse("076793A3-9FCF-4DBD-8121-5CC05ADC476B"), BlogEntry.JaimeBueza);
        }

        [TestMethod]
        public void Constructor()
        {
            new BlogEntry();
        }

        [TestMethod]
        public void IsIIdentifierGuid()
        {
            Assert.IsNotNull(new BlogEntry() as IIdentifier<Guid>);
        }

        [TestMethod]
        public void IsIConvertBlogRow()
        {
            Assert.IsNotNull(new BlogEntry() as IConvert<BlogRow>);
        }

        [TestMethod]
        public void Identifier()
        {
            var entry = new BlogEntry();
            var data = Guid.NewGuid();
            entry.Identifier = data;
            Assert.AreEqual<Guid>(data, entry.Identifier);
        }

        [TestMethod]
        public void SectionIdentifier()
        {
            var entry = new BlogEntry();
            var data = Guid.NewGuid();
            entry.SectionIdentifier = data;
            Assert.AreEqual<Guid>(data, entry.SectionIdentifier);
        }

        [TestMethod]
        public void Content()
        {
            var entry = new BlogEntry();
            var data = StringHelper.ValidString();
            entry.Content = data;
            Assert.AreEqual<string>(data, entry.Content);
        }

        [TestMethod]
        public void Title()
        {
            var entry = new BlogEntry();
            var data = StringHelper.ValidString();
            entry.Title = data;
            Assert.AreEqual<string>(data, entry.Title);
        }

        [TestMethod]
        public void PostedOn()
        {
            var entry = new BlogEntry();
            var data = DateTime.UtcNow;
            entry.PostedOn = data;
            Assert.AreEqual<DateTime>(data, entry.PostedOn);
        }

        [TestMethod]
        public void Convert()
        {
            var entry = new BlogEntry()
            {
                Content = StringHelper.ValidString(),
                Identifier = Guid.NewGuid(),
                SectionIdentifier = Guid.NewGuid(),
                Title = StringHelper.ValidString(),
                PostedOn = DateTime.UtcNow,
            };

            var converted = entry.Convert();
            Assert.AreEqual<string>(entry.SectionIdentifier.ToString(), converted.PartitionKey);
            Assert.AreEqual<string>(entry.Title, converted.Title);
            Assert.AreEqual<string>(entry.Identifier.ToString(), converted.RowKey);
            Assert.AreEqual<DateTime>(entry.PostedOn, converted.PostedOn);
        }
        #endregion
    }
}