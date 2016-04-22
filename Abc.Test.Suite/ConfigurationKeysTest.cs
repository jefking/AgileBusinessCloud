// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationKeysTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Configuration Keys Test
    /// </summary>
    [TestClass]
    public class ConfigurationKeysTest
    {
        #region Valid Cases
        [TestMethod]
        public void MailGunApiKey()
        {
            Assert.AreEqual<string>("MailGunApi", ConfigurationKeys.MailGunApiKey);
        }

        [TestMethod]
        public void MailGunApiKeyKey()
        {
            Assert.AreEqual<string>("MailGunApiKey", ConfigurationKeys.MailGunApiKeyKey);
        }
        #endregion
    }
}