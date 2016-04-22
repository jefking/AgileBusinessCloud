// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='CodeStormSocialTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Data
{
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class CodeStormSocialTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new CodeStormSocial();
        }

        [TestMethod]
        public void Handle()
        {
            var item = new CodeStormSocial();
            var data = StringHelper.ValidString();
            item.AbcHandle = data;
            Assert.AreEqual<string>(data, item.AbcHandle);
        }

        [TestMethod]
        public void TwitterHandle()
        {
            var item = new CodeStormSocial();
            var data = StringHelper.ValidString();
            item.TwitterHandle = data;
            Assert.AreEqual<string>(data, item.TwitterHandle);
        }

        [TestMethod]
        public void GitHubHandle()
        {
            var item = new CodeStormSocial();
            var data = StringHelper.ValidString();
            item.GitHubHandle = data;
            Assert.AreEqual<string>(data, item.GitHubHandle);
        }

        [TestMethod]
        public void TwitterLinks()
        {
            var item = new CodeStormSocial();
            var data = new List<string>();
            item.TwitterLinks = data;
            Assert.AreEqual<IEnumerable<string>>(data, item.TwitterLinks);
        }

        [TestMethod]
        public void TwitterMentions()
        {
            var item = new CodeStormSocial();
            var data = new List<Mention>();
            item.TwitterMentions = data;
            Assert.AreEqual<IEnumerable<Mention>>(data, item.TwitterMentions);
        }
        #endregion
    }
}