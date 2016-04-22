// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceDigestTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Process
{
    using Abc.Services.Process;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PerformanceDigestTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new PerformanceDigest();
        }

        [TestMethod]
        public void IsLoggingDigest()
        {
            Assert.IsNotNull(new PerformanceDigest() as LoggingDigest);
        }
        #endregion
    }
}