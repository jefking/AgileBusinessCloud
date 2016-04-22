// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='MentionTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Data
{
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MentionTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Mention();
        }

        [TestMethod]
        public void Handle()
        {
            var item = new Mention();
            var data = StringHelper.ValidString();
            item.TwitterHandle = data;
            Assert.AreEqual<string>(data, item.TwitterHandle);
        }

        [TestMethod]
        public void AbcHandle()
        {
            var item = new Mention();
            var data = StringHelper.ValidString();
            item.AbcHandle = data;
            Assert.AreEqual<string>(data, item.AbcHandle);
        }
        #endregion
    }
}