// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EmailTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using Abc.Services.Contracts;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EmailTest
    {
        #region Valid Cases
        [TestMethod]
        public void Valid()
        {
            var email = new PlaintextEmail()
            {
                Sender = StringHelper.ValidString(),
                Recipient = StringHelper.ValidString(),
            };

            var validator = new Validator<Email>();
            Assert.IsTrue(validator.IsValid(email));
        }

        [TestMethod]
        public void SenderNotSpecified()
        {
            var email = new PlaintextEmail()
            {
                Sender = StringHelper.NullEmptyWhiteSpace(),
                Recipient = StringHelper.ValidString(),
            };

            var validator = new Validator<Email>();
            Assert.IsFalse(validator.IsValid(email));
        }

        [TestMethod]
        public void RecipientNotSpecified()
        {
            var email = new PlaintextEmail()
            {
                Sender = StringHelper.ValidString(),
                Recipient = StringHelper.NullEmptyWhiteSpace(),
            };

            var validator = new Validator<Email>();
            Assert.IsFalse(validator.IsValid(email));
        }

        [TestMethod]
        public void SenderTooLong()
        {
            var email = new PlaintextEmail()
            {
                Sender = StringHelper.LongerThanMaximumRowLength(),
                Recipient = StringHelper.ValidString(),
            };

            var validator = new Validator<Email>();
            Assert.IsFalse(validator.IsValid(email));
        }

        [TestMethod]
        public void RecipientTooLong()
        {
            var email = new PlaintextEmail()
            {
                Sender = StringHelper.ValidString(),
                Recipient = StringHelper.LongerThanMaximumRowLength(),
            };

            var validator = new Validator<Email>();
            Assert.IsFalse(validator.IsValid(email));
        }
        #endregion
    }
}