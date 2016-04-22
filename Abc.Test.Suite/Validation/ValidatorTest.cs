// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ValidatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Validation
{
    using System;
    using System.Linq;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValidatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValidNull()
        {
            var validator = new Validator<Validate>();
            validator.IsValid(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllMessagesNull()
        {
            var validator = new Validator<Validate>();
            validator.AllMessages(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidNullMessageDontTestNull()
        {
            var validator = new Validator<Validate>();
            validator.Validate(null, false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidNullDontTestNull()
        {
            var validator = new Validator<Validate>();
            Assert.IsFalse(validator.IsValid(null, false));
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void ValidAllMessages()
        {
            var validate = new Validate()
            {
                Valid = true,
            };
            var validator = new Validator<Validate>();
            Assert.AreEqual<string>(string.Empty, validator.AllMessages(validate));
        }

        [TestMethod]
        public void ValidValidate()
        {
            var validate = new Validate()
            {
                Valid = true,
            };
            var validator = new Validator<Validate>();
            Assert.AreEqual<int>(0, validator.Validate(validate).Count());
        }

        [TestMethod]
        public void ValidValidateTestForNull()
        {
            var validate = new Validate()
            {
                Valid = true,
            };
            var validator = new Validator<Validate>();
            Assert.AreEqual<int>(0, validator.Validate(validate, true).Count());
        }

        [TestMethod]
        public void ValidIsValid()
        {
            var validate = new Validate()
            {
                Valid = true,
            };
            var validator = new Validator<Validate>();
            Assert.IsTrue(validator.IsValid(validate));
        }

        [TestMethod]
        public void ValidIsValidTestForNull()
        {
            var validate = new Validate()
            {
                Valid = true,
            };
            var validator = new Validator<Validate>();
            Assert.IsTrue(validator.IsValid(validate, true));
        }

        [TestMethod]
        public void ValidIsValidDontTestForNull()
        {
            var validate = new Validate()
            {
                Valid = true,
            };
            var validator = new Validator<Validate>();
            Assert.IsTrue(validator.IsValid(validate, false));
        }

        [TestMethod]
        public void InvalidAllMessages()
        {
            var validate = new Validate()
            {
                Valid = false,
            };
            var validator = new Validator<Validate>();
            Assert.AreEqual<string>("Invalid" + Environment.NewLine, validator.AllMessages(validate));
        }

        [TestMethod]
        public void InvalidValidate()
        {
            var validate = new Validate()
            {
                Valid = false,
            };
            var validator = new Validator<Validate>();
            Assert.AreEqual<int>(1, validator.Validate(validate).Count());
        }

        [TestMethod]
        public void InvalidValidateTestForNull()
        {
            var validate = new Validate()
            {
                Valid = false,
            };
            var validator = new Validator<Validate>();
            Assert.AreEqual<int>(1, validator.Validate(validate, true).Count());
        }

        [TestMethod]
        public void InvalidValidateDontTestForNull()
        {
            var validate = new Validate()
            {
                Valid = false,
            };
            var validator = new Validator<Validate>();
            Assert.AreEqual<int>(1, validator.Validate(validate, false).Count());
        }

        [TestMethod]
        public void InvalidIsValid()
        {
            var validate = new Validate()
            {
                Valid = false,
            };
            var validator = new Validator<Validate>();
            Assert.IsFalse(validator.IsValid(validate));
        }

        [TestMethod]
        public void InvalidIsValidTestForNull()
        {
            var validate = new Validate()
            {
                Valid = false,
            };
            var validator = new Validator<Validate>();
            Assert.IsFalse(validator.IsValid(validate, true));
        }

        [TestMethod]
        public void InvalidIsValidDontTestForNull()
        {
            var validate = new Validate()
            {
                Valid = false,
            };
            var validator = new Validator<Validate>();
            Assert.IsFalse(validator.IsValid(validate, false));
        }

        [TestMethod]
        public void InvalidNull()
        {
            var validator = new Validator<Validate>();
            Assert.IsFalse(validator.IsValid(null, true));
        }

        [TestMethod]
        public void InvalidNullMessage()
        {
            var validator = new Validator<Validate>();
            Assert.AreEqual<int>(1, validator.Validate(null, true).Count());
        }
        #endregion
    }
}