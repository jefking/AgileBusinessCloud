// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='DataCostCalculatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Data Cost Calculator Test
    /// </summary>
    [TestClass]
    public class DataCostCalculatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CalculateNull()
        {
            DataCostCalculator.Calculate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CalculateICollectionNull()
        {
            DataCostCalculator.Calculate((ICollection)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CalculateOverheadNull()
        {
            DataCostCalculator.Overhead(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CalculateStorageCost()
        {
            DataCostCalculator.RawDataLength(null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Overhead()
        {
            var length = DataCostCalculator.Overhead(new TestData().GetType());
            Assert.AreEqual<int>(108, length, "Property Names should match overhead.");
        }

        [TestMethod]
        public void StorageCost()
        {
            var random = new Random();
            var data = new TestData()
            {
                BoolTest = true,
                DateTimeTest = DateTime.UtcNow,
                IntTest = random.Next(),
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString()
            };

            var length = DataCostCalculator.RawDataLength(data);
            Assert.AreEqual<int>(165, length, "Property values should match.");
        }

        [TestMethod]
        public void Calculate()
        {
            var random = new Random();
            var data = new TestData()
            {
                BoolTest = true,
                DateTimeTest = DateTime.UtcNow,
                IntTest = random.Next(),
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString()
            };

            int storageCost = DataCostCalculator.RawDataLength(data);
            int overhead = DataCostCalculator.Overhead(new TestData().GetType());

            Assert.AreEqual<int>(storageCost + overhead, DataCostCalculator.Calculate(data), "Data sizing should match up.");
        }

        [TestMethod]
        public void CalculateList()
        {
            var random = new Random();
            var data = new TestData()
            {
                BoolTest = true,
                DateTimeTest = DateTime.UtcNow,
                IntTest = random.Next(),
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString()
            };
            int dataCost = DataCostCalculator.Calculate(data);

            var count = random.Next(15);
            var items = new List<TestData>(count);
            for (int i = 0; i < count; i++)
            {
                items.Add(data);
            }

            Assert.AreEqual<int>(dataCost * count, DataCostCalculator.Calculate(items));
        }
        #endregion

        #region Classes
        /// <summary>
        /// Test Data
        /// </summary>
        private class TestData : TableServiceEntity
        {
            public bool BoolTest
            {
                get;
                set;
            }

            public int IntTest
            {
                get;
                set;
            }

            public DateTime DateTimeTest
            {
                get;
                set;
            }
        }
        #endregion
    }
}