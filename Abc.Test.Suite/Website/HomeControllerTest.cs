// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='HomeControllerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using Abc.Website.Controllers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for HomeControllerTest and is intended
    /// to contain all HomeControllerTest Unit Tests
    /// </summary>
    [TestClass]
    public class HomeControllerTest
    {
        #region Valid Cases
        /// <summary>
        /// A test for HomeController Constructor
        /// </summary>
        [TestMethod]
        public void HomeControllerConstructor()
        {
            HomeController target = new HomeController();
            Assert.IsNotNull(target);
        }

        /// <summary>
        /// A test for About
        /// </summary>
        [TestMethod]
        public void About()
        {
            HomeController target = new HomeController();
            var actual = target.About();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// A test for Contact
        /// </summary>
        [TestMethod]
        public void Contact()
        {
            HomeController target = new HomeController();
            var actual = target.Contact();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// A test for Index
        /// </summary>
        [TestMethod]
        public void Index()
        {
            HomeController target = new HomeController();
            var actual = target.Index();
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}