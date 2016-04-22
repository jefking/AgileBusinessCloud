// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PayPalPaymentConfirmationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PayPalPaymentConfirmationTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new PayPalPaymentConfirmation();
        }

        [TestMethod]
        public void User()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = new User();
            payment.User = data;
            Assert.AreEqual<User>(data, payment.User);
        }

        [TestMethod]
        public void App()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = Application.Current;
            payment.Application = data;
            Assert.AreEqual<Application>(data, payment.Application);
        }

        [TestMethod]
        public void Successful()
        {
            var payment = new PayPalPaymentConfirmation();
            Assert.IsFalse(payment.Successful);
            payment.Successful = true;
            Assert.IsTrue(payment.Successful);
        }

        [TestMethod]
        public void TransactionId()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.TransactionId = data;
            Assert.AreEqual<string>(data, payment.TransactionId);
        }

        [TestMethod]
        public void PaymentStatus()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.PaymentStatus = data;
            Assert.AreEqual<string>(data, payment.PaymentStatus);
        }

        [TestMethod]
        public void Amount()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Amount = data;
            Assert.AreEqual<string>(data, payment.Amount);
        }

        [TestMethod]
        public void Response()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Response = data;
            Assert.AreEqual<string>(data, payment.Response);
        }

        [TestMethod]
        public void CurrencyCode()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.CurrencyCode = data;
            Assert.AreEqual<string>(data, payment.CurrencyCode);
        }

        [TestMethod]
        public void Email()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Email = data;
            Assert.AreEqual<string>(data, payment.Email);
        }

        [TestMethod]
        public void FirstName()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.FirstName = data;
            Assert.AreEqual<string>(data, payment.FirstName);
        }

        [TestMethod]
        public void LastName()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.LastName = data;
            Assert.AreEqual<string>(data, payment.LastName);
        }

        [TestMethod]
        public void PaymentStatusFromPayPal()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.PaymentStatusFromPayPal = data;
            Assert.AreEqual<string>(data, payment.PaymentStatusFromPayPal);
        }

        [TestMethod]
        public void Gross()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Gross = data;
            Assert.AreEqual<string>(data, payment.Gross);
        }

        [TestMethod]
        public void Currency()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Currency = data;
            Assert.AreEqual<string>(data, payment.Currency);
        }

        [TestMethod]
        public void Custom()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Custom = data;
            Assert.AreEqual<string>(data, payment.Custom);
        }

        [TestMethod]
        public void MCGross()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.MCGross = data;
            Assert.AreEqual<string>(data, payment.MCGross);
        }

        [TestMethod]
        public void ProtectionEligibility()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ProtectionEligibility = data;
            Assert.AreEqual<string>(data, payment.ProtectionEligibility);
        }

        [TestMethod]
        public void AddressStatus()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressStatus = data;
            Assert.AreEqual<string>(data, payment.AddressStatus);
        }

        [TestMethod]
        public void PayerId()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.PayerId = data;
            Assert.AreEqual<string>(data, payment.PayerId);
        }

        [TestMethod]
        public void Tax()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Tax = data;
            Assert.AreEqual<string>(data, payment.Tax);
        }

        [TestMethod]
        public void AddressStreet()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressStreet = data;
            Assert.AreEqual<string>(data, payment.AddressStreet);
        }

        [TestMethod]
        public void PaymentDate()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.PaymentDate = data;
            Assert.AreEqual<string>(data, payment.PaymentDate);
        }

        [TestMethod]
        public void Charset()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Charset = data;
            Assert.AreEqual<string>(data, payment.Charset);
        }

        [TestMethod]
        public void AddressZip()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressZip = data;
            Assert.AreEqual<string>(data, payment.AddressZip);
        }

        [TestMethod]
        public void MCFee()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.MCFee = data;
            Assert.AreEqual<string>(data, payment.MCFee);
        }

        [TestMethod]
        public void AddressCountryCode()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressCountryCode = data;
            Assert.AreEqual<string>(data, payment.AddressCountryCode);
        }

        [TestMethod]
        public void AddressName()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressName = data;
            Assert.AreEqual<string>(data, payment.AddressName);
        }

        [TestMethod]
        public void PayerStatus()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.PayerStatus = data;
            Assert.AreEqual<string>(data, payment.PayerStatus);
        }

        [TestMethod]
        public void Business()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Business = data;
            Assert.AreEqual<string>(data, payment.Business);
        }

        [TestMethod]
        public void AddressCountry()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressCountry = data;
            Assert.AreEqual<string>(data, payment.AddressCountry);
        }

        [TestMethod]
        public void AddressCity()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressCity = data;
            Assert.AreEqual<string>(data, payment.AddressCity);
        }

        [TestMethod]
        public void Quantity()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Quantity = data;
            Assert.AreEqual<string>(data, payment.Quantity);
        }

        [TestMethod]
        public void TXNId()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.TXNId = data;
            Assert.AreEqual<string>(data, payment.TXNId);
        }

        [TestMethod]
        public void PaymentType()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.PaymentType = data;
            Assert.AreEqual<string>(data, payment.PaymentType);
        }

        [TestMethod]
        public void BTNId()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.BTNId = data;
            Assert.AreEqual<string>(data, payment.BTNId);
        }

        [TestMethod]
        public void ReceiverEmail()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ReceiverEmail = data;
            Assert.AreEqual<string>(data, payment.ReceiverEmail);
        }

        [TestMethod]
        public void AddressState()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.AddressState = data;
            Assert.AreEqual<string>(data, payment.AddressState);
        }

        [TestMethod]
        public void Shipping()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Shipping = data;
            Assert.AreEqual<string>(data, payment.Shipping);
        }

        [TestMethod]
        public void PaymentFee()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.PaymentFee = data;
            Assert.AreEqual<string>(data, payment.PaymentFee);
        }

        [TestMethod]
        public void ShippingDiscount()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ShippingDiscount = data;
            Assert.AreEqual<string>(data, payment.ShippingDiscount);
        }

        [TestMethod]
        public void InsuranceAmount()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.InsuranceAmount = data;
            Assert.AreEqual<string>(data, payment.InsuranceAmount);
        }

        [TestMethod]
        public void ReceiverId()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ReceiverId = data;
            Assert.AreEqual<string>(data, payment.ReceiverId);
        }

        [TestMethod]
        public void TXNType()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.TXNType = data;
            Assert.AreEqual<string>(data, payment.TXNType);
        }

        [TestMethod]
        public void ItemName()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ItemName = data;
            Assert.AreEqual<string>(data, payment.ItemName);
        }

        [TestMethod]
        public void Discount()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.Discount = data;
            Assert.AreEqual<string>(data, payment.Discount);
        }

        [TestMethod]
        public void ItemNumber()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ItemNumber = data;
            Assert.AreEqual<string>(data, payment.ItemNumber);
        }

        [TestMethod]
        public void ResidenceCountry()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ResidenceCountry = data;
            Assert.AreEqual<string>(data, payment.ResidenceCountry);
        }

        [TestMethod]
        public void ShippingMethod()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.ShippingMethod = data;
            Assert.AreEqual<string>(data, payment.ShippingMethod);
        }

        [TestMethod]
        public void HandlingAmount()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.HandlingAmount = data;
            Assert.AreEqual<string>(data, payment.HandlingAmount);
        }

        [TestMethod]
        public void TransactionSubject()
        {
            var payment = new PayPalPaymentConfirmation();
            var data = StringHelper.ValidString();
            payment.TransactionSubject = data;
            Assert.AreEqual<string>(data, payment.TransactionSubject);
        }

        [TestMethod]
        public void Valid()
        {
            var payment = new PayPalPaymentConfirmation()
            {
                TransactionId = StringHelper.ValidString(),
            };

            var validator = new Validator<PayPalPaymentConfirmation>();
            Assert.IsTrue(validator.IsValid(payment));
        }

        [TestMethod]
        public void InvalidTransactionIdInvalid()
        {
            var payment = new PayPalPaymentConfirmation()
            {
                TransactionId = StringHelper.NullEmptyWhiteSpace(),
            };

            var validator = new Validator<PayPalPaymentConfirmation>();
            Assert.IsFalse(validator.IsValid(payment));
        }

        [TestMethod]
        public void Convert()
        {
            var payment = new PayPalPaymentConfirmation()
            {
                Amount = StringHelper.ValidString(),
                Application = Abc.Services.Contracts.Application.Current,
                Currency = StringHelper.ValidString(),
                CurrencyCode = StringHelper.ValidString(),
                Custom = StringHelper.ValidString(),
                Email = StringHelper.ValidString(),
                FirstName = StringHelper.ValidString(),
                Gross = StringHelper.ValidString(),
                LastName = StringHelper.ValidString(),
                PaymentStatus = StringHelper.ValidString(),
                PaymentStatusFromPayPal = StringHelper.ValidString(),
                Successful = false,
                TransactionId = StringHelper.ValidString(),
                Response = StringHelper.ValidString(),
                MCGross = StringHelper.ValidString(),
                ProtectionEligibility = StringHelper.ValidString(),
                AddressStatus = StringHelper.ValidString(),
                PayerId = StringHelper.ValidString(),
                Tax = StringHelper.ValidString(),
                AddressStreet = StringHelper.ValidString(),
                PaymentDate = StringHelper.ValidString(),
                Charset = StringHelper.ValidString(),
                AddressZip = StringHelper.ValidString(),
                MCFee = StringHelper.ValidString(),
                AddressCountryCode = StringHelper.ValidString(),
                AddressName = StringHelper.ValidString(),
                PayerStatus = StringHelper.ValidString(),
                Business = StringHelper.ValidString(),
                AddressCountry = StringHelper.ValidString(),
                Quantity = StringHelper.ValidString(),
                TXNId = StringHelper.ValidString(),
                PaymentType = StringHelper.ValidString(),
                BTNId = StringHelper.ValidString(),
                AddressState = StringHelper.ValidString(),
                ReceiverEmail = StringHelper.ValidString(),
                Shipping = StringHelper.ValidString(),
                PaymentFee = StringHelper.ValidString(),
                ShippingDiscount = StringHelper.ValidString(),
                InsuranceAmount = StringHelper.ValidString(),
                ReceiverId = StringHelper.ValidString(),
                TXNType = StringHelper.ValidString(),
                ItemName = StringHelper.ValidString(),
                Discount = StringHelper.ValidString(),
                ItemNumber = StringHelper.ValidString(),
                ResidenceCountry = StringHelper.ValidString(),
                ShippingMethod = StringHelper.ValidString(),
                HandlingAmount = StringHelper.ValidString(),
                TransactionSubject = StringHelper.ValidString(),
                User = new User()
                {
                    Identifier = Guid.NewGuid(),
                },
            };

            var row = payment.Convert();
            Assert.AreEqual<string>(payment.Amount, row.Amount);
            Assert.AreEqual<string>(payment.Currency, row.Currency);
            Assert.AreEqual<string>(payment.CurrencyCode, row.CurrencyCode);
            Assert.AreEqual<string>(payment.Custom, row.Custom);
            Assert.AreEqual<string>(payment.Email, row.Email);
            Assert.AreEqual<string>(payment.FirstName, row.FirstName);
            Assert.AreEqual<string>(payment.Gross, row.Gross);
            Assert.AreEqual<string>(payment.LastName, row.LastName);
            Assert.AreEqual<string>(payment.PaymentStatus, row.PaymentStatus);
            Assert.AreEqual<string>(payment.PaymentStatusFromPayPal, row.PaymentStatusFromPayPal);
            Assert.AreEqual<string>(payment.TransactionId, row.TransactionId);
            Assert.AreEqual<string>(payment.Response, row.Response);
            Assert.AreEqual<string>(payment.MCGross, row.MCGross);
            Assert.AreEqual<string>(payment.ProtectionEligibility, row.ProtectionEligibility);
            Assert.AreEqual<string>(payment.AddressStatus, row.AddressStatus);
            Assert.AreEqual<string>(payment.PayerId, row.PayerId);
            Assert.AreEqual<string>(payment.Tax, row.Tax);
            Assert.AreEqual<string>(payment.AddressStreet, row.AddressStreet);
            Assert.AreEqual<string>(payment.PaymentDate, row.PaymentDate);
            Assert.AreEqual<string>(payment.Charset, row.Charset);
            Assert.AreEqual<string>(payment.AddressZip, row.AddressZip);
            Assert.AreEqual<string>(payment.MCFee, row.MCFee);
            Assert.AreEqual<string>(payment.AddressCountryCode, row.AddressCountryCode);
            Assert.AreEqual<string>(payment.AddressName, row.AddressName);
            Assert.AreEqual<string>(payment.PayerStatus, row.PayerStatus);
            Assert.AreEqual<string>(payment.Business, row.Business);
            Assert.AreEqual<string>(payment.AddressCountry, row.AddressCountry);
            Assert.AreEqual<string>(payment.Quantity, row.Quantity);
            Assert.AreEqual<string>(payment.TXNId, row.TXNId);
            Assert.AreEqual<string>(payment.PaymentType, row.PaymentType);
            Assert.AreEqual<string>(payment.BTNId, row.BTNId);
            Assert.AreEqual<string>(payment.AddressState, row.AddressState);
            Assert.AreEqual<string>(payment.ReceiverEmail, row.ReceiverEmail);
            Assert.AreEqual<string>(payment.Shipping, row.Shipping);
            Assert.AreEqual<string>(payment.PaymentFee, row.PaymentFee);
            Assert.AreEqual<string>(payment.ShippingDiscount, row.ShippingDiscount);
            Assert.AreEqual<string>(payment.InsuranceAmount, row.InsuranceAmount);
            Assert.AreEqual<string>(payment.ReceiverId, row.ReceiverId);
            Assert.AreEqual<string>(payment.TXNType, row.TXNType);
            Assert.AreEqual<string>(payment.ItemName, row.ItemName);
            Assert.AreEqual<string>(payment.Discount, row.Discount);
            Assert.AreEqual<string>(payment.ItemNumber, row.ItemNumber);
            Assert.AreEqual<string>(payment.ResidenceCountry, row.ResidenceCountry);
            Assert.AreEqual<string>(payment.ShippingMethod, row.ShippingMethod);
            Assert.AreEqual<string>(payment.HandlingAmount, row.HandlingAmount);
            Assert.AreEqual<string>(payment.TransactionSubject, row.TransactionSubject);
            Assert.AreEqual<bool>(payment.Successful, row.Successful);
            Assert.AreEqual<Guid>(payment.User.Identifier, row.UserId);
            Assert.AreEqual<Guid>(payment.Application.Identifier, row.ApplicationId);
        }
        #endregion
    }
}