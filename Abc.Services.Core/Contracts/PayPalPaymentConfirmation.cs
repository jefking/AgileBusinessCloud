// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PayPalPaymentConfirmation.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Abc.Services.Data;
    using Abc.Services.Validation;

    /// <summary>
    /// PayPal Payment Confirmation
    /// </summary>
    public class PayPalPaymentConfirmation : IUser, IConvert<PayPalPaymentConfirmationRow>, IApplication, IValidate<PayPalPaymentConfirmation>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Application
        /// </summary>
        public Application Application
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets User
        /// </summary>
        public User User
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
        [DataMember]
        public string Currency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Custom
        /// </summary>
        [DataMember]
        public string Custom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets MCGross
        /// </summary>
        [DataMember]
        public string MCGross
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Protection Eligibility
        /// </summary>
        [DataMember]
        public string ProtectionEligibility
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Status
        /// </summary>
        [DataMember]
        public string AddressStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payer Id
        /// </summary>
        [DataMember]
        public string PayerId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Tax
        /// </summary>
        [DataMember]
        public string Tax
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Street
        /// </summary>
        [DataMember]
        public string AddressStreet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payment Date
        /// </summary>
        [DataMember]
        public string PaymentDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Charset
        /// </summary>
        [DataMember]
        public string Charset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Zip
        /// </summary>
        [DataMember]
        public string AddressZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets MC Fee
        /// </summary>
        [DataMember]
        public string MCFee
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Country Code
        /// </summary>
        [DataMember]
        public string AddressCountryCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Name
        /// </summary>
        [DataMember]
        public string AddressName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payer Status
        /// </summary>
        [DataMember]
        public string PayerStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Business
        /// </summary>
        [DataMember]
        public string Business
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address Country
        /// </summary>
        [DataMember]
        public string AddressCountry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address City
        /// </summary>
        [DataMember]
        public string AddressCity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Quantity
        /// </summary>
        [DataMember]
        public string Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets TXN Id
        /// </summary>
        [DataMember]
        public string TXNId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payment Type
        /// </summary>
        [DataMember]
        public string PaymentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Button Id
        /// </summary>
        [DataMember]
        public string BTNId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Address State
        /// </summary>
        [DataMember]
        public string AddressState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Receiver Email
        /// </summary>
        [DataMember]
        public string ReceiverEmail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Shipping
        /// </summary>
        [DataMember]
        public string Shipping
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Payment Fee
        /// </summary>
        [DataMember]
        public string PaymentFee
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Shipping Discount
        /// </summary>
        [DataMember]
        public string ShippingDiscount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Insurance Amount
        /// </summary>
        [DataMember]
        public string InsuranceAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Receiver Id
        /// </summary>
        [DataMember]
        public string ReceiverId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Taxation Type
        /// </summary>
        [DataMember]
        public string TXNType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Item Name
        /// </summary>
        [DataMember]
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Discount
        /// </summary>
        [DataMember]
        public string Discount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Item Number
        /// </summary>
        [DataMember]
        public string ItemNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Residence Country
        /// </summary>
        [DataMember]
        public string ResidenceCountry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Shipping Method
        /// </summary>
        [DataMember]
        public string ShippingMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Handling Amount
        /// </summary>
        [DataMember]
        public string HandlingAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Transaction Subject
        /// </summary>
        [DataMember]
        public string TransactionSubject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Rules for Validation
        /// </summary>
        [ScriptIgnore]
        [IgnoreDataMember]
        public IEnumerable<Rule<PayPalPaymentConfirmation>> Rules
        {
            get
            {
                return new Rule<PayPalPaymentConfirmation>[]
                {
                    new Rule<PayPalPaymentConfirmation>(p => !string.IsNullOrWhiteSpace(p.TransactionId), "Transaction Id is not present."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to PayPal Payment Confirmation Row
        /// </summary>
        /// <returns>Converted</returns>
        [CLSCompliant(false)]
        public PayPalPaymentConfirmationRow Convert()
        {
            return new PayPalPaymentConfirmationRow(this.Application.Identifier)
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
                UserId = this.User.Identifier,
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
            };
        }
        #endregion
    }
}