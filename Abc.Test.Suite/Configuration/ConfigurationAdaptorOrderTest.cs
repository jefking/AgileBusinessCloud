// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationAdaptorOrderTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using System;
    using Abc.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigurationAdaptorOrderTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullAdaptor()
        {
            var random = new Random();
            new ConfigurationAdaptorOrder(null, random.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorNegativeOrder()
        {
            var random = new Random();
            new ConfigurationAdaptorOrder(new ConfigurationAdaptorTest(), random.Next() * -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompareToNull()
        {
            var random = new Random();
            var adaptor = new ConfigurationAdaptorOrder(new ConfigurationAdaptorTest(), random.Next());
            adaptor.CompareTo(null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            var random = new Random();
            new ConfigurationAdaptorOrder(new ConfigurationAdaptorTest(), random.Next());
        }

        [TestMethod]
        public void Adaptor()
        {
            var random = new Random();
            var adaptor = new ConfigurationAdaptorTest();
            var config = new ConfigurationAdaptorOrder(adaptor, random.Next());
            Assert.AreEqual<IConfigurationAdaptor>(adaptor, config.Adaptor);
        }

        [TestMethod]
        public void Order()
        {
            var random = new Random();
            var order = random.Next();
            var config = new ConfigurationAdaptorOrder(new ConfigurationAdaptorTest(), order);
            Assert.AreEqual<int>(order, config.Order);
        }

        [TestMethod]
        public void CompareTo()
        {
            var random = new Random();
            var small = random.Next();
            var large = random.Next(small + 1, int.MaxValue);
            var firstAdaptor = new ConfigurationAdaptorOrder(new ConfigurationAdaptorTest(), small);
            var secondAdaptor = new ConfigurationAdaptorOrder(new ConfigurationAdaptorTest(), large);
            Assert.IsTrue(firstAdaptor.CompareTo(secondAdaptor) == 1);
            Assert.IsTrue(firstAdaptor.CompareTo(firstAdaptor) == 0);
            Assert.IsTrue(secondAdaptor.CompareTo(firstAdaptor) == -1);
            Assert.IsTrue(secondAdaptor.CompareTo(secondAdaptor) == 0);
        }
        #endregion
    }
}