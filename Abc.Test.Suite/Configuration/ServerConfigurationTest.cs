// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerConfigurationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using Abc.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using System;

    [TestClass]
    public class ServerConfigurationTest
    {
        #region Valid Cases
        [TestMethod]
        public void DataConnectionString()
        {
            Assert.AreEqual<string>("DataConnectionString", ServerConfiguration.DataConnectionStringKey);
        }

        [TestMethod]
        public void GitHubClientIdKey()
        {
            Assert.AreEqual<string>("GitHubKey", ServerConfiguration.GitHubClientIdKey);
        }

        [TestMethod]
        public void GitHubSecretKey()
        {
            Assert.AreEqual<string>("GitHubSecret", ServerConfiguration.GitHubSecretKey);
        }

        [TestMethod]
        public void CodeStormDataConnectionString()
        {
            Assert.AreEqual<string>("CodeStorm.DataConnectionString", ServerConfiguration.CodeStormDataConnectionStringKey);
        }

        [TestMethod]
        public void PayPalPaymentDataTransferUrlKey()
        {
            Assert.AreEqual<string>("PayPal.PaymentDataTransferUrl", ServerConfiguration.PayPalPaymentDataTransferUrlKey);
        }

        [TestMethod]
        public void TestKey()
        {
            Assert.AreEqual<string>("95FD7F7F-439B-4D97-95C1-2782B9CE1743", ServerConfiguration.TestKey);
        }

        [TestMethod]
        public void ApplicationIdentifierKey()
        {
            Assert.AreEqual<string>("ApplicationIdentifier", ServerConfiguration.ApplicationIdentifierKey);
        }

        [TestMethod]
        public void ContentDistrobutionUrlKey()
        {
            Assert.AreEqual<string>("Abc.CdnUrl", ServerConfiguration.ContentDistributionUrlKey);
        }

        [TestMethod]
        public void BlobUrlKey()
        {
            Assert.AreEqual<string>("Abc.BlobUrl", ServerConfiguration.BlobUrlKey);
        }

        [TestMethod]
        public void Testing()
        {
            Assert.IsTrue(ServerConfiguration.Testing);
        }

        [TestMethod]
        public void Default()
        {
            Assert.AreEqual<CloudStorageAccount>(CloudStorageAccount.DevelopmentStorageAccount, CloudStorageAccount.DevelopmentStorageAccount);
        }

        [TestMethod]
        public void ApplicationIdentifierReg()
        {
            Assert.AreEqual<Guid>(Abc.Underpinning.Application.Identifier, ServerConfiguration.ApplicationIdentifier);
        }

        [TestMethod]
        public void ApplicationIdentifierAdmin()
        {
            Assert.AreEqual<Guid>(Abc.Underpinning.Administration.Application.Identifier, ServerConfiguration.ApplicationIdentifier);
        }

        [TestMethod]
        public void ContentDistrobutionUrl()
        {
            Assert.AreEqual<string>("http://cdn.agilebusinesscloud.com", ServerConfiguration.ContentDistributionUrl);
        }

        [TestMethod]
        public void BlobUrl()
        {
            Assert.AreEqual<string>("http://content.agilebusinesscloud.com", ServerConfiguration.BlobUrl);
        }

        [TestMethod]
        public void PayPalPaymentDataTransferKey()
        {
            Assert.AreEqual<string>("PayPal.PaymentDataTransfer", ServerConfiguration.PayPalPaymentDataTransferKey);
        }

        [TestMethod]
        public void GitHubClientId()
        {
            Assert.AreEqual<string>(null, ServerConfiguration.GitHubClientId);
        }

        [TestMethod]
        public void GitHubSecret()
        {
            Assert.AreEqual<string>(null, ServerConfiguration.GitHubSecret);
        }
        #endregion
    }
}