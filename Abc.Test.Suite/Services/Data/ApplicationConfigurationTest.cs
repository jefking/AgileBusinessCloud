// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationConfigurationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Application Configuration Test
    /// </summary>
    [TestClass]
    public class ApplicationConfigurationTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorEmptyApplicationId()
        {
            var key = StringHelper.ValidString();
            var ac = new ApplicationConfiguration(Guid.Empty, key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorInvalidKey()
        {
            var id = Guid.NewGuid();
            var key = StringHelper.NullEmptyWhiteSpace();
            var ac = new ApplicationConfiguration(id, key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorKeyWithSpaces()
        {
            var id = Guid.NewGuid();
            var key = string.Format("  {0} ", StringHelper.ValidString());
            var ac = new ApplicationConfiguration(id, key);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void SerializationConstructor()
        {
            new ApplicationConfiguration();
        }

        [TestMethod]
        public void Constructor()
        {
            var before = DateTime.UtcNow;
            var id = Guid.NewGuid();
            var key = StringHelper.ValidString();
            var ac = new ApplicationConfiguration(id, key);

            Assert.AreEqual<string>(key, ac.RowKey);
            Assert.AreEqual<Guid>(id, ac.ApplicationId);
            Assert.IsTrue(before <= ac.CreatedOn);
        }

        [TestMethod]
        public void Value()
        {
            var ac = new ApplicationConfiguration();
            var value = StringHelper.ValidString();
            ac.Value = value;

            Assert.AreEqual<string>(value, ac.Value);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var ac = new ApplicationConfiguration();
            var createdOn = DateTime.UtcNow;
            ac.CreatedOn = createdOn;

            Assert.AreEqual<DateTime>(createdOn, ac.CreatedOn);
        }

        [TestMethod]
        public void CreatedBy()
        {
            var ac = new ApplicationConfiguration();
            var createdBy = Guid.NewGuid();
            ac.CreatedBy = createdBy;

            Assert.AreEqual<Guid>(createdBy, ac.CreatedBy);
        }

        [TestMethod]
        public void LastUpdatedOn()
        {
            var ac = new ApplicationConfiguration();
            var lastUpdatedOn = DateTime.UtcNow;
            ac.LastUpdatedOn = lastUpdatedOn;

            Assert.AreEqual<DateTime>(lastUpdatedOn, ac.LastUpdatedOn);
        }

        [TestMethod]
        public void LastUpdatedBy()
        {
            var ac = new ApplicationConfiguration();
            var lastUpdatedBy = Guid.NewGuid();
            ac.LastUpdatedBy = lastUpdatedBy;

            Assert.AreEqual<Guid>(lastUpdatedBy, ac.LastUpdatedBy);
        }

        [TestMethod]
        public void Convert()
        {
            var id = Guid.NewGuid();
            var key = StringHelper.ValidString();
            var ac = new ApplicationConfiguration(id, key);
            ac.Value = StringHelper.ValidString();

            var c = ac.Convert();
            Assert.AreEqual<string>(ac.RowKey, c.Key);
            Assert.AreEqual<string>(ac.Value, c.Value);
        }
        #endregion
    }
}