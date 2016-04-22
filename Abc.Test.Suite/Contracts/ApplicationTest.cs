// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationTest
    {
        #region Valid Cases
        [TestMethod]
        public void ApplicationId()
        {
            var id = Guid.NewGuid();
            var app = new Application()
            {
                Identifier = id,
            };

            Assert.AreEqual<Guid>(id, app.Identifier);
        }

        [TestMethod]
        public void Current()
        {
            Assert.IsNotNull(Application.Current);
            Assert.AreEqual<Guid>(ConfigurationSettings.ApplicationIdentifier, Application.Current.Identifier);
        }

        [TestMethod]
        public void Default()
        {
            Assert.IsNotNull(Application.Default);
            Assert.AreEqual<Guid>(new Guid("3BD8FBF6-E89A-4FE0-8369-A314519D1F6F"), Application.Default.Identifier);
        }
        #endregion
    }
}