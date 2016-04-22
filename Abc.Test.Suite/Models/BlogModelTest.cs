// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BlogModelTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Models
{
    using System.Collections.Generic;
    using Abc.Services.Contracts;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BlogModelTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BlogModel();
        }

        [TestMethod]
        public void Posts()
        {
            var item = new BlogModel();
            var data = new List<BlogEntry>();
            item.Posts = data;
            Assert.AreEqual<IEnumerable<BlogEntry>>(data, item.Posts);
        }

        [TestMethod]
        public void Post()
        {
            var item = new BlogModel();
            var data = new BlogEntry();
            item.Post = data;
            Assert.AreEqual<BlogEntry>(data, item.Post);
        }
        #endregion
    }
}