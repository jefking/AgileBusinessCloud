// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RegisterModelTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for RegisterModelTest and is intended
    /// to contain all RegisterModelTest Unit Tests
    /// </summary>
    [TestClass]
    public class RegisterModelTest
    {
        #region Valid Cases
        /// <summary>
        /// A test for RegisterModel Constructor
        /// </summary>
        [TestMethod]
        public void RegisterModelConstructor()
        {
            var target = new RegisterModel();
            Assert.IsNotNull(target);
        }

        /// <summary>
        /// A test for Email
        /// </summary>
        [TestMethod]
        public void Email()
        {
            var target = new RegisterModel();
            var expected = StringHelper.ValidString();
            target.Email = expected;
            var actual = target.Email;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        /// A test for NameIdentifier
        /// </summary>
        [TestMethod]
        public void NameIdentifier()
        {
            var target = new RegisterModel();
            var expected = StringHelper.ValidString();
            target.NameIdentifier = expected;
            var actual = target.NameIdentifier;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        /// A test for UserName
        /// </summary>
        [TestMethod]
        public void UserName()
        {
            var target = new RegisterModel();
            var expected = StringHelper.ValidString();
            target.UserName = expected;
            var actual = target.UserName;
            Assert.AreEqual<string>(expected, actual);
        }
        #endregion
    }
}