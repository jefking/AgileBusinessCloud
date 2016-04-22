// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='HardcodedConfigurationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HardcodedConfigurationTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new HardCoded();
        }

        [TestMethod]
        public void Configuration()
        {
            var hc = new HardCoded();
            Assert.AreEqual<int>(1, hc.Configuration.Count);
            Assert.AreEqual<string>(hc.InitialValue, hc.Configuration[hc.InitialKey]);
        }
        #endregion
    }
}