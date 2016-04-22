// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RoleEnvironmentConfigurationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using System;
    using System.Collections.Generic;
    using Abc.Azure.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RoleEnvironmentConfigurationTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new RoleEnvironmentConfigurationDictionary();
        }

        [TestMethod]
        public void TryGetValueInvalidKey()
        {
            var role = new RoleEnvironmentConfigurationDictionary();
            string value;
            Assert.IsFalse(role.TryGetValue(StringHelper.NullEmptyWhiteSpace(), out value));
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryGetValueKeyDoesntExist()
        {
            var role = new RoleEnvironmentConfigurationDictionary();
            string value;
            Assert.IsFalse(role.TryGetValue(StringHelper.ValidString(), out value));
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ContainsKeyInvalidKey()
        {
            var role = new RoleEnvironmentConfigurationDictionary();
            Assert.IsFalse(role.ContainsKey(StringHelper.NullEmptyWhiteSpace()));
        }

        [TestMethod]
        public void IsReadOnly()
        {
            var role = new RoleEnvironmentConfigurationDictionary();
            Assert.IsFalse(role.IsReadOnly);
        }

        [TestMethod]
        public void ValueRoundTrip()
        {
            var role = new RoleEnvironmentConfigurationDictionary();
            Assert.AreEqual<int>(0, role.Count);
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            Assert.IsFalse(role.ContainsKey(key));
            role[key] = value;
            Assert.AreEqual<string>(value, role[key]);
            Assert.AreEqual<int>(1, role.Count);
        }
        #endregion
    }
}