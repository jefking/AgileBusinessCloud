// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='NetworkCounterTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using System;
    using Abc.Instrumentation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NetworkCounterTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidInstance()
        {
            var random = new Random();
            new NetworkCounter(StringHelper.NullEmptyWhiteSpace(), random.Next(1, 1000));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidBandwidth()
        {
            var random = new Random();
            new NetworkCounter(StringHelper.ValidString(), random.Next(0));
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            var random = new Random();
            new NetworkCounter(StringHelper.ValidString(), random.Next(1, 19000));
        }

        [TestMethod]
        public void IsCounter()
        {
            var random = new Random();
            Assert.IsNotNull(new NetworkCounter(StringHelper.ValidString(), random.Next(1, 19000)) as Counter);
        }
        #endregion
    }
}