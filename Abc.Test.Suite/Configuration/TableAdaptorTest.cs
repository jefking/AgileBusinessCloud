// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TableAdaptorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using System;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TableAdaptorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new TableAdaptor(Guid.NewGuid());
        }

        [TestMethod]
        public void RoundTripConfiguration()
        {
            var table = new AzureTable<ApplicationConfiguration>(ServerConfiguration.Default);
            var data = new ApplicationConfiguration(Guid.NewGuid(), Guid.NewGuid().ToString())
            {
                Value = Guid.NewGuid().ToString(),
            };
            table.AddEntity(data);

            var adaptor = new TableAdaptor(data.ApplicationId);
            adaptor.Load(null);
            Assert.AreEqual<string>(data.Value, adaptor.Configuration[data.RowKey]);
        }
        #endregion
    }
}