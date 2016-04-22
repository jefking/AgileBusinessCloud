// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AzureEnvironmentTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Global.Azure
{
    using System;
    using Abc.Azure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AzureEnvironmentTest
    {
        #region Valid Cases
        [TestMethod]
        public void RoleIsAvailable()
        {
            Assert.IsFalse(AzureEnvironment.RoleIsAvailable);
        }

        [TestMethod]
        public void IsComputeEmulator()
        {
            Assert.IsFalse(AzureEnvironment.IsComputeEmulator);
        }

        [TestMethod]
        public void DeploymentId()
        {
            Assert.IsNull(AzureEnvironment.DeploymentId);
        }

        [TestMethod]
        public void ServerName()
        {
            Assert.AreEqual<string>(Environment.MachineName,  AzureEnvironment.ServerName);
        }
        #endregion
    }
}