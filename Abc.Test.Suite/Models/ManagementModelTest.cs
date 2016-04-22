// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ManagementModelTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using System;
    using System.Collections.Generic;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Abc.Services.Contracts;

    [TestClass]
    public class ManagementModelTest
    {
        #region Error Cases
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ManagementModel();
        }

        [TestMethod]
        public void ApplicationNull()
        {
            var model = new ManagementModel();
            Assert.IsNull(model.Application);
            model.Application = null;
            Assert.IsNull(model.Application);
        }

        [TestMethod]
        public void Application()
        {
            var model = new ManagementModel();
            var app = new ApplicationDetailsModel()
            {
                ApplicationId = Guid.NewGuid(),
                IsValid = true,
                ValidUntil = DateTime.UtcNow,
            };

            model.Application = app;

            Assert.AreEqual<Guid>(app.ApplicationId, model.Application.ApplicationId);
            Assert.AreEqual<bool>(app.IsValid, model.Application.IsValid);
            Assert.AreEqual<DateTime>(app.ValidUntil, model.Application.ValidUntil);
        }

        [TestMethod]
        public void Applications()
        {
            var model = new ManagementModel();
            var data = new List<ApplicationDetailsModel>();
            model.Applications = data;

            Assert.AreEqual<IEnumerable<ApplicationDetailsModel>>(data, model.Applications);
        }

        [TestMethod]
        public void Data()
        {
            var model = new ManagementModel();
            var items = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(StringHelper.ValidString());
            }

            model.Data = items;

            bool found = false;
            foreach (string item in items)
            {
                found = false;
                foreach (string data in model.Data)
                {
                    if (item == data)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void DataNull()
        {
            var model = new ManagementModel();
            Assert.IsNull(model.Data);
            model.Data = null;
            Assert.IsNull(model.Data);
        }

        [TestMethod]
        public void Preference()
        {
            var model = new ManagementModel();
            var data = new UserPreference();
            model.Preference = data;
            Assert.AreEqual<UserPreference>(data, model.Preference);
        }
        #endregion
    }
}
