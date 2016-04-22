// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ErrorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using System;
    using Abc.Web;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ErrorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Error();
        }

        [TestMethod]
        public void Code()
        {
            var error = new Error();
            var random = new Random();
            var data = random.Next();
            error.Code = data;
            Assert.AreEqual<int>(data, error.Code);
        }

        [TestMethod]
        public void Message()
        {
            var error = new Error();
            var data = StringHelper.ValidString();
            error.Message = data;
            Assert.AreEqual<string>(data, error.Message);
        }
        #endregion
    }
}