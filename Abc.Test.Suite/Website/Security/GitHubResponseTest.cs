// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='GitHubResponseTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website.Security
{
    using Abc.Website.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class GitHubResponseTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDataInvalid()
        {
            GitHubResponse.Parse(StringHelper.NullEmptyWhiteSpace());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new GitHubResponse();
        }

        [TestMethod]
        public void AccessToken()
        {
            var item = new GitHubResponse();
            var data = StringHelper.ValidString();
            item.AccessToken = data;
            Assert.AreEqual<string>(data, item.AccessToken);
        }

        [TestMethod]
        public void TokenType()
        {
            var item = new GitHubResponse();
            var data = StringHelper.ValidString();
            item.TokenType = data;
            Assert.AreEqual<string>(data, item.TokenType);
        }

        [TestMethod]
        public void Parse()
        {
            var item = GitHubResponse.Parse("access_token=e72e16c7e42f292c6912e7710c838347ae178b4a&token_type=bearer");
            Assert.AreEqual<string>("e72e16c7e42f292c6912e7710c838347ae178b4a", item.AccessToken);
            Assert.AreEqual<string>("bearer", item.TokenType);
        }
        #endregion
    }
}