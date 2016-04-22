// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BlogRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    [TestClass]
    public class BlogRowTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorPosterInvalid()
        {
            var poster = Guid.Empty;
            var identifier = Guid.NewGuid();

            new BlogRow(poster, identifier);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorIdentifierInvalid()
        {
            var poster = Guid.NewGuid();
            var identifier = Guid.Empty;

            new BlogRow(poster, identifier);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BlogRow();
        }

        [TestMethod]
        public void IsIConvertBlogEntry()
        {
            Assert.IsNotNull(new BlogRow() as IConvert<BlogEntry>);
        }

        [TestMethod]
        public void IsTableServiceEntity()
        {
            Assert.IsNotNull(new BlogRow() as TableServiceEntity);
        }

        [TestMethod]
        public void ConstructorData()
        {
            var poster = Guid.NewGuid();
            var identifier = Guid.NewGuid();

            var row = new BlogRow(poster, identifier);
        }

        [TestMethod]
        public void RowKey()
        {
            var poster = Guid.NewGuid();
            var identifier = Guid.NewGuid();

            var row = new BlogRow(poster, identifier);
            Assert.AreEqual<string>(identifier.ToString(), row.RowKey);
        }

        [TestMethod]
        public void PartitionKey()
        {
            var poster = Guid.NewGuid();
            var identifier = Guid.NewGuid();

            var row = new BlogRow(poster, identifier);
            Assert.AreEqual<string>(poster.ToString(), row.PartitionKey);
        }

        [TestMethod]
        public void Title()
        {
            var row = new BlogRow();
            var data = StringHelper.ValidString();
            row.Title = data;
            Assert.AreEqual<string>(data, row.Title);
        }

        [TestMethod]
        public void PostedOn()
        {
            var row = new BlogRow();
            var data = DateTime.UtcNow;
            row.PostedOn = data;
            Assert.AreEqual<DateTime>(data, row.PostedOn);
        }

        [TestMethod]
        public void Convert()
        {
            var poster = Guid.NewGuid();
            var identifier = Guid.NewGuid();

            var row = new BlogRow(poster, identifier)
            {
                PostedOn = DateTime.UtcNow,
                Title = StringHelper.ValidString(),
            };
            var converted = row.Convert();
            Assert.AreEqual<string>(row.PartitionKey, converted.SectionIdentifier.ToString());
            Assert.AreEqual<string>(row.Title, converted.Title);
            Assert.AreEqual<string>(row.RowKey, converted.Identifier.ToString());
            Assert.AreEqual<DateTime>(row.PostedOn, converted.PostedOn);
        }
        #endregion
    }
}