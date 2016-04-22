// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PaymentCoreTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Core
{
    using System;
    using System.Linq;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PaymentCoreTest
    {
        #region Members
        private const string PayPalResponseValue = @"SUCCESS
first_name=Jane+Doe
last_name=Smith
payment_status=Completed 
payer_email=janiedoesmith@hotmail.com
payment_gross=3.99
mc_currency=USD 
custom=For+the+purchase+of+the+rare+book+Green+Eggs+%26+Ham";

        private const string PayPalResponseFull = @"mc_gross=0.01 
protection_eligibility=Eligible 
address_status=confirmed 
payer_id=LHXGFUDRHPPEL 
tax=0.00 
address_street=546+beatty%0D%0A309 
payment_date=18%3A49%3A02+Jan+30%2C+2012+PST 
payment_status=Completed 
charset=windows-1252 
address_zip=v6b2l3 
first_name=Jeffrey 
mc_fee=0.01 
address_country_code=CA 
address_name=Jeffrey+King 
custom= 
payer_status=unverified 
business=jef%40agilebusinesscloud.com 
address_country=Canada 
address_city=vancouver 
quantity=1 
payer_email=jamieking80%40gmail.com 
txn_id=5JW20211VT0352427 
payment_type=instant 
btn_id=41305525 
last_name=King 
address_state=British+Columbia 
receiver_email=jef%40agilebusinesscloud.com 
payment_fee= 
shipping_discount=0.00 
insurance_amount=0.00 
receiver_id=6ZLN7RZS446C2 
txn_type=web_accept 
item_name=One+Cent 
discount=0.00 
mc_currency=CAD 
item_number=0002 
residence_country=CA 
shipping_method=Default 
handling_amount=0.00 
transaction_subject=One+Cent 
payment_gross= 
shipping=0.00";
        #endregion

        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidatePurchasePaymentNull()
        {
            var core = new PaymentCore();
            core.ValidatePurchase(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessResponsePaymentNull()
        {
            PaymentCore.ProcessResponse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidatePurchaseTransactionIdInvalid()
        {
            var payment = new PayPalPaymentConfirmation()
            {
                TransactionId = StringHelper.NullEmptyWhiteSpace(),
            };

            var core = new PaymentCore();
            core.ValidatePurchase(payment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessSuccessfulResponsePaymentNull()
        {
            PaymentCore.ProcessSuccessfulResponse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessSuccessfulResponseInvalid()
        {
            var payment = new PayPalPaymentConfirmation()
            {
                Response = StringHelper.NullEmptyWhiteSpace(),
            };

            PaymentCore.ProcessSuccessfulResponse(payment);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void ProcessResponseSuccessful()
        {
            var random = new Random();
            var payment = new PayPalPaymentConfirmation()
            {
                Application = new Application()
                {
                    Identifier = Guid.NewGuid(),
                },
                User = new User()
                {
                    Identifier = Guid.NewGuid(),
                },
                TransactionId = random.Next().ToString(),
                Response = PayPalResponseValue,
            };

            payment = PaymentCore.ProcessResponse(payment);

            var table = new AzureTable<PayPalPaymentConfirmationRow>(ServerConfiguration.Default);
            var row = table.QueryByPartition(payment.Application.Identifier.ToString()).First();

            Assert.AreEqual<string>(payment.FirstName, row.FirstName);
            Assert.AreEqual<string>(payment.LastName, row.LastName);
            Assert.AreEqual<string>(payment.PaymentStatusFromPayPal, row.PaymentStatusFromPayPal);
            Assert.AreEqual<string>(payment.Email, row.Email);
            Assert.AreEqual<string>(payment.Gross, row.Gross);
            Assert.AreEqual<string>(payment.CurrencyCode, row.CurrencyCode);
            Assert.AreEqual<string>(payment.Custom, row.Custom);
            Assert.IsTrue(payment.Successful);
            Assert.IsTrue(row.Successful);
        }

        [TestMethod]
        public void ProcessResponseSuccessfulApplicationAdded()
        {
            var random = new Random();
            var payment = new PayPalPaymentConfirmation()
            {
                Application = new Application()
                {
                    Identifier = Guid.NewGuid(),
                },
                User = new User()
                {
                    Identifier = Guid.NewGuid(),
                },
                TransactionId = random.Next().ToString(),
                Response = PayPalResponseValue,
            };

            payment = PaymentCore.ProcessResponse(payment);

            var table = new AzureTable<UserPreferenceRow>(ServerConfiguration.Default);
            var row = table.QueryBy(payment.Application.Identifier.ToString(), payment.User.Identifier.ToString());

            Assert.AreEqual<int>(1, row.MaxiumAllowedApplications.Value);
        }

        [TestMethod]
        public void ProcessResponseInvalid()
        {
            var random = new Random();
            var payment = new PayPalPaymentConfirmation()
            {
                Application = new Application()
                {
                    Identifier = Guid.NewGuid(),
                },
                User = new User()
                {
                    Identifier = Guid.NewGuid(),
                },
                TransactionId = random.Next().ToString(),
                Response = StringHelper.ValidString(),
            };

            payment = PaymentCore.ProcessResponse(payment);

            var table = new AzureTable<PayPalPaymentConfirmationRow>(ServerConfiguration.Default);
            var row = table.QueryByPartition(payment.Application.Identifier.ToString()).First();

            Assert.IsFalse(payment.Successful);
            Assert.IsFalse(row.Successful);
        }

        [TestMethod]
        public void ProcessSuccessfulResponse()
        {
            var payment = new PayPalPaymentConfirmation()
            {
                Response = PayPalResponseValue,
            };

            payment = PaymentCore.ProcessSuccessfulResponse(payment);

            Assert.AreEqual<string>("Jane+Doe", payment.FirstName);
            Assert.AreEqual<string>("Smith", payment.LastName);
            Assert.AreEqual<string>("Completed", payment.PaymentStatus);
            Assert.AreEqual<string>("janiedoesmith@hotmail.com", payment.Email);
            Assert.AreEqual<string>("3.99", payment.Gross);
            Assert.AreEqual<string>("USD", payment.CurrencyCode);
            Assert.AreEqual<string>("For+the+purchase+of+the+rare+book+Green+Eggs+%26+Ham", payment.Custom);
        }

        [TestMethod]
        public void ProcessSuccessfulResponseFull()
        {
            var payment = new PayPalPaymentConfirmation()
            {
                Response = PayPalResponseFull,
            };

            payment = PaymentCore.ProcessSuccessfulResponse(payment);
            Assert.AreEqual<string>("0.01", payment.MCGross);
            Assert.AreEqual<string>("Eligible", payment.ProtectionEligibility);
            Assert.AreEqual<string>("confirmed", payment.AddressStatus);
            Assert.AreEqual<string>("LHXGFUDRHPPEL", payment.PayerId);
            Assert.AreEqual<string>("0.00", payment.Tax);
            Assert.AreEqual<string>("546+beatty%0D%0A309", payment.AddressStreet);
            Assert.AreEqual<string>("18%3A49%3A02+Jan+30%2C+2012+PST", payment.PaymentDate);
            Assert.AreEqual<string>("Completed", payment.PaymentStatus);
            Assert.AreEqual<string>("windows-1252", payment.Charset);
            Assert.AreEqual<string>("v6b2l3", payment.AddressZip);
            Assert.AreEqual<string>("Jeffrey", payment.FirstName);
            Assert.AreEqual<string>("0.01", payment.MCFee);
            Assert.AreEqual<string>("CA", payment.AddressCountryCode);
            Assert.AreEqual<string>("Jeffrey+King", payment.AddressName);
            Assert.AreEqual<string>(null, payment.PaymentFee);
            Assert.AreEqual<string>(null, payment.Custom);
            Assert.AreEqual<string>(null, payment.Gross);
            Assert.AreEqual<string>("unverified", payment.PayerStatus);
            Assert.AreEqual<string>("1", payment.Quantity);
            Assert.AreEqual<string>("jef%40agilebusinesscloud.com", payment.Business);
            Assert.AreEqual<string>("Canada", payment.AddressCountry);
            Assert.AreEqual<string>("vancouver", payment.AddressCity);
            Assert.AreEqual<string>("jamieking80%40gmail.com", payment.Email);
            Assert.AreEqual<string>("5JW20211VT0352427", payment.TXNId);
            Assert.AreEqual<string>("instant", payment.PaymentType);
            Assert.AreEqual<string>("41305525", payment.BTNId);
            Assert.AreEqual<string>("King", payment.LastName);
            Assert.AreEqual<string>("British+Columbia", payment.AddressState);
            Assert.AreEqual<string>("jef%40agilebusinesscloud.com", payment.ReceiverEmail);
            Assert.AreEqual<string>("0.00", payment.ShippingDiscount);
            Assert.AreEqual<string>("0.00", payment.InsuranceAmount);
            Assert.AreEqual<string>("6ZLN7RZS446C2", payment.ReceiverId);
            Assert.AreEqual<string>("web_accept", payment.TXNType);
            Assert.AreEqual<string>("One+Cent", payment.ItemName);
            Assert.AreEqual<string>("0.00", payment.Discount);
            Assert.AreEqual<string>("CAD", payment.CurrencyCode);
            Assert.AreEqual<string>("0002", payment.ItemNumber);
            Assert.AreEqual<string>("CA", payment.ResidenceCountry);
            Assert.AreEqual<string>("Default", payment.ShippingMethod);
            Assert.AreEqual<string>("0.00", payment.HandlingAmount);
            Assert.AreEqual<string>("One+Cent", payment.TransactionSubject);
            Assert.AreEqual<string>("0.00", payment.Shipping);
        }
        #endregion
    }
}