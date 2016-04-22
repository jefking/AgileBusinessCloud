// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RoleEnvironmentAdaptorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using Abc.Azure.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RoleEnvironmentAdaptorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new RoleEnvironmentAdaptor();
        }

        [TestMethod]
        public void Configuration()
        {
            var configuration = new RoleEnvironmentAdaptor();
            Assert.IsNotNull(configuration.Configuration);
        }
        #endregion
    }
}