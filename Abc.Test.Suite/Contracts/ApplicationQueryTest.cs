// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationQueryTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationQueryTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ApplicationQuery();
        }

        [TestMethod]
        public void ApplicationIdentifier()
        {
            var query = new ApplicationQuery();
            var data = Guid.NewGuid();
            query.ApplicationIdentifier = data;
            Assert.AreEqual<Guid>(data, query.ApplicationIdentifier);
        }
        #endregion
    }
}