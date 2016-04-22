// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PayPalPaymentConfirmationRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// PayPal Payment Confirmation
    /// </summary>
    [AzureDataStore("PayPalPaymentConfirmation")]
    [CLSCompliant(false)]
    public class PayPalPaymentConfirmationRow : ApplicationData, IConvert<PayPalPaymentConfirmation>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PayPalPaymentConfirmationRow class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public PayPalPaymentConfirmationRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PayPalPaymentConfirmationRow class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public PayPalPaymentConfirmationRow(Guid applicationId)
            : base(applicationId)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets User
        /// </summary>
        public Guid UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Successful
        /// </summary>
        public bool Successful
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Transaction Id
        /// </summary>
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Response
        /// </summary>
        public string Response
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payment Status
        /// </summary>
        public string PaymentStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Amount
        /// </summary>
        public string Amount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Currency Code
        /// </summary>
        public string CurrencyCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets First Name
        /// </summary>
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Name
        /// </summary>
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payement Status From PayPal
        /// </summary>
        public string PaymentStatusFromPayPal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Gross
        /// </summary>
        public string Gross
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Currency
        /// </summary>
        public string Currency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Custom
        /// </summary>
        public string Custom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets MC Gross
        /// </summary>
        public string MCGross
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Protection Eligibility
        /// </summary>
        public string ProtectionEligibility
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Status
        /// </summary>
        public string AddressStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payer Id
        /// </summary>
        public string PayerId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Tax
        /// </summary>
        public string Tax
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Street
        /// </summary>
        public string AddressStreet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payment Date
        /// </summary>
        public string PaymentDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Charset
        /// </summary>
        public string Charset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Zip
        /// </summary>
        public string AddressZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets MC Fee
        /// </summary>
        public string MCFee
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Country Code
        /// </summary>
        public string AddressCountryCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Name
        /// </summary>
        public string AddressName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payer Status
        /// </summary>
        public string PayerStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Business
        /// </summary>
        public string Business
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Country
        /// </summary>
        public string AddressCountry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address City
        /// </summary>
        public string AddressCity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Quantity
        /// </summary>
        public string Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Taxation Id
        /// </summary>
        public string TXNId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payment Type
        /// </summary>
        public string PaymentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Button Id
        /// </summary>
        public string BTNId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address State
        /// </summary>
        public string AddressState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Receiver Email
        /// </summary>
        public string ReceiverEmail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Shipping
        /// </summary>
        public string Shipping
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payment Fee
        /// </summary>
        public string PaymentFee
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Shipping Discount
        /// </summary>
        public string ShippingDiscount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Insurance Amount
        /// </summary>
        public string InsuranceAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Receiver Id
        /// </summary>
        public string ReceiverId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Taxation Type
        /// </summary>
        public string TXNType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Item Name
        /// </summary>
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Discount
        /// </summary>
        public string Discount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Item Number
        /// </summary>
        public string ItemNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Residence Country
        /// </summary>
        public string ResidenceCountry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Shipping Method
        /// </summary>
        public string ShippingMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Handling Amount
        /// </summary>
        public string HandlingAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Transaction Subject
        /// </summary>
        public string TransactionSubject
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to PayPal Payment Confirmation
        /// </summary>
        /// <returns>Converted</returns>
        public PayPalPaymentConfirmation Convert()
        {
            return new PayPalPaymentConfirmation()
            {
                Amount = this.Amount,
                Currency = this.Currency,
                CurrencyCode = this.CurrencyCode,
                Custom = this.Custom,
                Email = this.Email,
                FirstName = this.FirstName,
                Gross = this.Gross,
                LastName = this.LastName,
                PaymentStatus = this.PaymentStatus,
                PaymentStatusFromPayPal = this.PaymentStatusFromPayPal,
                Successful = this.Successful,
                TransactionId = this.TransactionId,
                Response = this.Response,
                MCGross = this.MCGross,
                ProtectionEligibility = this.ProtectionEligibility,
                AddressStatus = this.AddressStatus,
                PayerId = this.PayerId,
                Tax = this.Tax,
                AddressStreet = this.AddressStreet,
                PaymentDate = this.PaymentDate,
                Charset = this.Charset,
                AddressZip = this.AddressZip,
                MCFee = this.MCFee,
                AddressCountryCode = this.AddressCountryCode,
                AddressName = this.AddressName,
                PayerStatus = this.PayerStatus,
                Business = this.Business,
                AddressCountry = this.AddressCountry,
                Quantity = this.Quantity,
                TXNId = this.TXNId,
                PaymentType = this.PaymentType,
                BTNId = this.BTNId,
                AddressState = this.AddressState,
                ReceiverEmail = this.ReceiverEmail,
                Shipping = this.Shipping,
                PaymentFee = this.PaymentFee,
                ShippingDiscount = this.ShippingDiscount,
                InsuranceAmount = this.InsuranceAmount,
                ReceiverId = this.ReceiverId,
                TXNType = this.TXNType,
                ItemName = this.ItemName,
                Discount = this.Discount,
                ItemNumber = this.ItemNumber,
                ResidenceCountry = this.ResidenceCountry,
                ShippingMethod = this.ShippingMethod,
                HandlingAmount = this.HandlingAmount,
                TransactionSubject = this.TransactionSubject,
                User = new User()
                {
                    Identifier = this.UserId,
                },
                Application = new Application()
                {
                    Identifier = this.ApplicationId,
                },
            };
        }
        #endregion
    }
}