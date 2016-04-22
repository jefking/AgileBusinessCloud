// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredQueryTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BytesStoredQueryTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new BytesStoredQuery();
        }

        [TestMethod]
        public void Top()
        {
            var random = new Random();
            var query = new BytesStoredQuery();
            Assert.IsNull(query.Top);
            var data = random.Next();
            query.Top = data;
            Assert.AreEqual<int?>(data, query.Top);
        }

        [TestMethod]
        public void To()
        {
            var query = new BytesStoredQuery();
            Assert.IsNull(query.To);
            var data = DateTime.UtcNow;
            query.To = data;
            Assert.AreEqual<DateTime?>(data, query.To);
        }

        [TestMethod]
        public void From()
        {
            var query = new BytesStoredQuery();
            Assert.IsNull(query.From);
            var data = DateTime.UtcNow;
            query.From = data;
            Assert.AreEqual<DateTime?>(data, query.From);
        }

        [TestMethod]
        public void DataCost()
        {
            var query = new BytesStoredQuery();
            Assert.IsNull(query.DataCostType);
            var data = DataCostType.Egress;
            query.DataCostType = data;
            Assert.AreEqual<DataCostType?>(data, query.DataCostType);
        }
        #endregion
    }
}