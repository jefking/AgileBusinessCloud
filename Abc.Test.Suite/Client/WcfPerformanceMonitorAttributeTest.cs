// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='WcfPerformanceMonitorAttributeTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using Abc.Web;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WcfPerformanceMonitorAttributeTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new WcfPerformanceMonitorAttribute(typeof(object));
        }
        #endregion
    }
}