// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AccountValidationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Web.Security;
    using Abc.Website.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;    
    
    /// <summary>
    /// This is a test class for AccountValidationTest and is intended
    /// to contain all AccountValidationTest Unit Tests
    /// </summary>
    [TestClass]
    public class AccountValidationTest
    {
        #region Valid Cases
        /// <summary>
        /// A test for ErrorCodeToString
        /// </summary>
        [TestMethod]
        public void ErrorCodeToStringTest()
        {
            Random random = new Random();
            string actual = AccountValidation.ErrorCodeToString((MembershipCreateStatus)random.Next(11));
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual));
        }
        #endregion
    }
}