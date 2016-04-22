// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PaymentCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using System;
    using System.Data.Services.Client;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Abc.Text;

    /// <summary>
    /// Payment Core
    /// </summary>
    public class PaymentCore
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        private static readonly LogCore log = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// Process PayPal Response
        /// </summary>
        /// <param name="payment">Payment</param>
        /// <returns>PayPal Payment Confirmation</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring.")]
        public static PayPalPaymentConfirmation ProcessResponse(PayPalPaymentConfirmation payment)
        {
            Contract.Requires<ArgumentNullException>(null != payment);

            if (string.IsNullOrWhiteSpace(payment.Response))
            {
                log.Log("Empty response returned from PayPal.");
            }
            else if (payment.Response.StartsWith("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
            {
                payment.Successful = true;

                payment = PaymentCore.ProcessSuccessfulResponse(payment);

                var preferenceTable = new AzureTable<UserPreferenceRow>(ServerConfiguration.Default);
                var preference = preferenceTable.QueryBy(payment.Application.Identifier.ToString(), payment.User.Identifier.ToString());

                preference = preference ?? new UserPreferenceRow(payment.Application.Identifier, payment.User.Identifier);

                preference.MaxiumAllowedApplications = preference.MaxiumAllowedApplications.HasValue ? preference.MaxiumAllowedApplications + 1 : 1;
                preferenceTable.AddOrUpdateEntity(preference);
            }
            else
            {
                payment.Successful = false;

                log.Log("Error Accepting payment response.");
            }

            var table = new AzureTable<PayPalPaymentConfirmationRow>(ServerConfiguration.Default);
            table.AddEntity(payment.Convert());

            return payment;
        }

        /// <summary>
        /// Process Successful Response
        /// </summary>
        /// <param name="payment">Payment</param>
        /// <returns>PayPal Payment Confirmation</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring.")]
        public static PayPalPaymentConfirmation ProcessSuccessfulResponse(PayPalPaymentConfirmation payment)
        {
            Contract.Requires<ArgumentNullException>(null != payment);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(payment.Response));

            foreach (Match match in RegexStatement.PayPalResponse.Matches(payment.Response))
            {
                var value = match.Groups["value"].Value;
                switch (match.Groups["key"].Value)
                {
                    case "first_name":
                        payment.FirstName = value;
                        continue;
                    case "last_name":
                        payment.LastName = value;
                        continue;
                    case "payment_status":
                        payment.PaymentStatus = value;
                        continue;
                    case "payer_email":
                        payment.Email = value;
                        continue;
                    case "payment_gross":
                        payment.Gross = value;
                        continue;
                    case "mc_currency":
                        payment.CurrencyCode = value;
                        continue;
                    case "custom":
                        payment.Custom = value;
                        continue;
                    case "mc_gross":
                        payment.MCGross = value;
                        continue;
                    case "protection_eligibility":
                        payment.ProtectionEligibility = value;
                        continue;
                    case "address_status":
                        payment.AddressStatus = value;
                        continue;
                    case "payer_id":
                        payment.PayerId = value;
                        continue;
                    case "tax":
                        payment.Tax = value;
                        continue;
                    case "address_street":
                        payment.AddressStreet = value;
                        continue;
                    case "payment_date":
                        payment.PaymentDate = value;
                        continue;
                    case "charset":
                        payment.Charset = value;
                        continue;
                    case "address_zip":
                        payment.AddressZip = value;
                        continue;
                    case "mc_fee":
                        payment.MCFee = value;
                        continue;
                    case "address_country_code":
                        payment.AddressCountryCode = value;
                        continue;
                    case "address_name":
                        payment.AddressName = value;
                        continue;
                    case "payer_status":
                        payment.PayerStatus = value;
                        continue;
                    case "business":
                        payment.Business = value;
                        continue;
                    case "address_country":
                        payment.AddressCountry = value;
                        continue;
                    case "address_city":
                        payment.AddressCity = value;
                        continue;
                    case "quantity":
                        payment.Quantity = value;
                        continue;
                    case "txn_id":
                        payment.TXNId = value;
                        continue;
                    case "payment_type":
                        payment.PaymentType = value;
                        continue;
                    case "btn_id":
                        payment.BTNId = value;
                        continue;
                    case "address_state":
                        payment.AddressState = value;
                        continue;
                    case "receiver_email":
                        payment.ReceiverEmail = value;
                        continue;
                    case "payment_fee":
                        payment.PaymentFee = value;
                        continue;
                    case "shipping_discount":
                        payment.ShippingDiscount = value;
                        continue;
                    case "insurance_amount":
                        payment.InsuranceAmount = value;
                        continue;
                    case "receiver_id":
                        payment.ReceiverId = value;
                        continue;
                    case "txn_type":
                        payment.TXNType = value;
                        continue;
                    case "item_name":
                        payment.ItemName = value;
                        continue;
                    case "discount":
                        payment.Discount = value;
                        continue;
                    case "item_number":
                        payment.ItemNumber = value;
                        continue;
                    case "residence_country":
                        payment.ResidenceCountry = value;
                        continue;
                    case "shipping_method":
                        payment.ShippingMethod = value;
                        continue;
                    case "handling_amount":
                        payment.HandlingAmount = value;
                        continue;
                    case "transaction_subject":
                        payment.TransactionSubject = value;
                        continue;
                    case "shipping":
                        payment.Shipping = value;
                        continue;
                    default:
                        log.Log("Unknown response from PayPal: '{0}'; '{1}'.".FormatWithCulture(match.Groups["key"].Value, value));
                        continue;
                }
            }

            return payment;
        }

        /// <summary>
        /// Validate Purchase
        /// </summary>
        /// <param name="payment">Payment</param>
        /// <returns>Sucessful</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring.")]
        public bool ValidatePurchase(PayPalPaymentConfirmation payment)
        {
            Contract.Requires<ArgumentNullException>(null != payment);

            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(payment.TransactionId));

            payment.Successful = false;

            var url = ServerConfiguration.PayPalPaymentDataTransferUrl;
            var query = "cmd=_notify-synch&tx={0}&at={1}".FormatWithCulture(payment.TransactionId, ServerConfiguration.PayPalPaymentDataTransfer);
            var req = (HttpWebRequest)WebRequest.Create(url);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = query.Length;

            // Write the request back IPN strings
            using (var output = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {
                output.Write(query);

                // Do the request to PayPal and get the response
                var input = new StreamReader(req.GetResponse().GetResponseStream());
                
                payment.Response = input.ReadToEnd();

                payment = PaymentCore.ProcessResponse(payment);
            }

            return payment.Successful;
        }
        #endregion
    }
}