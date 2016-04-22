// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='MvcPerformanceMonitorAttributeTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using System.Web.Mvc;
    using Abc.Web;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MvcPerformanceMonitorAttributeTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new MvcPerformanceMonitorAttribute();
        }

        [TestMethod]
        public void IsActionFilterAttribute()
        {
            Assert.IsNotNull(new MvcPerformanceMonitorAttribute() as ActionFilterAttribute);
        }

        [TestMethod]
        public void ConstructorBool()
        {
            new MvcPerformanceMonitorAttribute(true);
        }

        [TestMethod]
        public void OnActionExecutingNull()
        {
            var mvc = new MvcPerformanceMonitorAttribute();
            mvc.OnActionExecuting(null);
        }

        [TestMethod]
        public void OnActionExecutedNull()
        {
            var mvc = new MvcPerformanceMonitorAttribute();
            mvc.OnResultExecuted(null);
        }
        #endregion
    }
}