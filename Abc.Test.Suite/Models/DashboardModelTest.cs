// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='DashboardModelTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Models
{
    using System.Collections.Generic;
    using Abc.Services.Contracts;
    using Abc.Website;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DashboardModelTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new DashboardModel();
        }

        [TestMethod]
        public void Messages()
        {
            var model = new DashboardModel();
            var data = new List<MessageDisplay>();
            model.Messages = data;
            Assert.AreEqual<IEnumerable<MessageDisplay>>(data, model.Messages);
        }

        [TestMethod]
        public void Occurrences()
        {
            var model = new DashboardModel();
            var data = new List<OccurrenceDisplay>();
            model.Occurrences = data;
            Assert.AreEqual<IEnumerable<OccurrenceDisplay>>(data, model.Occurrences);
        }

        [TestMethod]
        public void Servers()
        {
            var model = new DashboardModel();
            var data = new List<string>();
            model.Servers = data;
            Assert.AreEqual<IEnumerable<string>>(data, model.Servers);
        }

        [TestMethod]
        public void Errors()
        {
            var model = new DashboardModel();
            var data = new List<ErrorDisplay>();
            model.Errors = data;
            Assert.AreEqual<IEnumerable<ErrorDisplay>>(data, model.Errors);
        }

        [TestMethod]
        public void Summary()
        {
            var model = new DashboardModel();
            var data = new ApplicationSummary();
            model.Summary = data;
            Assert.AreEqual<ApplicationSummary>(data, model.Summary);
        }
        #endregion
    }
}