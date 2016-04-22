// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RuleTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Validation
{
    using System;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RuleTest
    {
        #region Error Cases
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ConstructorNullFunction()
        {
            new Rule<object>(null, "invalid");
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void ConstructorNullMessage()
        {
            new Rule<object>(o => o != null, null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void TestTrue()
        {
            var rule = new Rule<object>(o => !string.IsNullOrWhiteSpace(o.ToString()), "this is valid");
            Assert.IsTrue(rule.Test(new object()));
        }

        [TestMethod]
        public void TestFalse()
        {
            var rule = new Rule<object>(o => string.IsNullOrWhiteSpace(o.ToString()), "this is valid");
            Assert.IsFalse(rule.Test(new object()));
            Assert.AreEqual<string>("this is valid", rule.Message);
        }
        #endregion
    }
}