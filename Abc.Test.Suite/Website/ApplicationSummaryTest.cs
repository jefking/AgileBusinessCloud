// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationSummaryTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using System;
    using Abc.Website;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationSummaryTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ApplicationSummary();
        }

        [TestMethod]
        public void Details()
        {
            var model = new ApplicationSummary();
            var data = new ApplicationDetailsModel();
            model.Details = data;
            Assert.AreEqual<ApplicationDetailsModel>(data, model.Details);
        }

        [TestMethod]
        public void ServerCount()
        {
            var random = new Random();
            var model = new ApplicationSummary();
            var data = random.Next();
            model.ServerCount = data;
            Assert.AreEqual<int>(data, model.ServerCount);
        }

        [TestMethod]
        public void MessageCount()
        {
            var random = new Random();
            var model = new ApplicationSummary();
            var data = random.Next();
            model.MessageCount = data;
            Assert.AreEqual<int>(data, model.MessageCount);
        }

        [TestMethod]
        public void OccurrenceCount()
        {
            var random = new Random();
            var model = new ApplicationSummary();
            var data = random.Next();
            model.OccurrenceCount = data;
            Assert.AreEqual<int>(data, model.OccurrenceCount);
        }

        [TestMethod]
        public void ErrorCount()
        {
            var random = new Random();
            var model = new ApplicationSummary();
            var data = random.Next();
            model.ErrorCount = data;
            Assert.AreEqual<int>(data, model.ErrorCount);
        }
        #endregion
    }
}