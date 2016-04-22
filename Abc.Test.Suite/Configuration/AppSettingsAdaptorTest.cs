// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AppSettingsAdaptorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using System.Configuration;
    using Abc.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppSettingsAdaptorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new AppSettingsAdaptor();
        }

        [TestMethod]
        public void ApplicationIdentifier()
        {
            var settings = new AppSettingsAdaptor();
            Assert.AreEqual<string>(settings.Configuration["ApplicationIdentifier"], ConfigurationManager.AppSettings["ApplicationIdentifier"]);
        }

        [TestMethod]
        public void AllSettings()
        {
            var settings = new AppSettingsAdaptor();
            foreach (var setting in settings.Configuration)
            {
                Assert.AreEqual<string>(setting.Value, ConfigurationManager.AppSettings[setting.Key]);
            }
        }
        #endregion
    }
}