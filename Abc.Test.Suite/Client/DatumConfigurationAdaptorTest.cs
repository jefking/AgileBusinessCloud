// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='DatumConfigurationAdaptorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using System.Linq;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class DatumConfigurationAdaptorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ServerConfigurationAdaptor();
        }

        [TestMethod]
        public void Dispose()
        {
            using (new ServerConfigurationAdaptor())
            {
            }
        }

        [TestMethod]
        public void Configuration()
        {
            var table = new AzureTable<ApplicationConfiguration>(CloudStorageAccount.DevelopmentStorageAccount);
            var configs = table.QueryByPartition(ConfigurationSettings.ApplicationIdentifier.ToString()).ToList();

            var adaptor = new ServerConfigurationAdaptor();
            var datumConfig = adaptor.Configuration;
            foreach (var item in configs)
            {
                if (!datumConfig.ContainsKey(item.RowKey))
                {
                    Assert.Fail("Unknown Key");
                }
                else if (item.Value != datumConfig[item.RowKey])
                {
                    Assert.Fail("Value is not equal?");
                }
            }
        }

        [TestMethod]
        public void LoadNull()
        {
            var adaptor = new ServerConfigurationAdaptor();
            adaptor.Load(null);
        }

        [TestMethod]
        public void Load()
        {
            var adaptor = new ServerConfigurationAdaptor();
            adaptor.Load(new object());
        }
        #endregion
    }
}