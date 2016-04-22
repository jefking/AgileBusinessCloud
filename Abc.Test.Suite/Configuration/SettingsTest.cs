// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='SettingsTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using System;
    using Abc.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SettingsTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            Assert.IsNotNull(Settings.Instance);
        }

        [TestMethod]
        public void AddSingle()
        {
            var adaptor = new ConfigurationAdaptorTest();
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            adaptor.Configuration.Add(key, value);

            var settings = Settings.Instance;
            settings.Add(adaptor);
            Assert.AreEqual<string>(value, settings.Get(key));
        }

        [TestMethod]
        public void AddMultiple()
        {
            var adaptorA = new ConfigurationAdaptorTest();
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            adaptorA.Configuration.Add(key, value);

            var adaptorB = new ConfigurationAdaptorTest();
            var wrongValue = Guid.NewGuid().ToString();
            adaptorB.Configuration.Add(key, wrongValue);

            var settings = Settings.Instance;
            settings.Add(adaptorB);
            settings.Add(adaptorA);
            Assert.AreEqual<string>(value, settings.Get(key));
        }

        [TestMethod]
        public void GetNull()
        {
            var settings = Settings.Instance;
            Assert.IsNull(settings.Get(null));
        }

        [TestMethod]
        public void GetInvalid()
        {
            var settings = Settings.Instance;
            Assert.IsNull(settings.Get(StringHelper.NullEmptyWhiteSpace()));
        }

        [TestMethod]
        public void GetDefaultValue()
        {
            var settings = Settings.Instance;
            var defaultValue = Guid.NewGuid();
            Assert.AreEqual<Guid>(defaultValue, settings.Get<Guid>(Guid.NewGuid().ToString(), defaultValue));
        }

        [TestMethod]
        public void ApplicationIdentifier()
        {
            Assert.AreEqual<Guid>(ServerConfiguration.ApplicationIdentifier, Settings.ApplicationIdentifier);
        }

        [TestMethod]
        public void ObjectIndexer()
        {
            Assert.AreEqual<string>(ServerConfiguration.ApplicationIdentifier.ToString(), Settings.Instance["ApplicationIdentifier"]);
        }
        #endregion
    }
}