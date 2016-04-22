// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerConfiguration.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System;
    using System.Diagnostics.Contracts;
    using Ms = Microsoft.WindowsAzure;

    /// <summary>
    /// Server Configuration
    /// </summary>
    public static class ServerConfiguration
    {
        #region Members
        /// <summary>
        /// Data Connection String
        /// </summary>
        public const string DataConnectionStringKey = "DataConnectionString";

        /// <summary>
        /// CodeStorm Data Connection String
        /// </summary>
        public const string CodeStormDataConnectionStringKey = "CodeStorm.DataConnectionString";

        /// <summary>
        /// GitHub Client Id Key
        /// </summary>
        public const string GitHubClientIdKey = "GitHubKey";

        /// <summary>
        /// GitHub Secret Key
        /// </summary>
        public const string GitHubSecretKey = "GitHubSecret";

        /// <summary>
        /// Test Key
        /// </summary>
        public const string TestKey = "95FD7F7F-439B-4D97-95C1-2782B9CE1743";

        /// <summary>
        /// Application Identifier Key
        /// </summary>
        public const string ApplicationIdentifierKey = "ApplicationIdentifier";

        /// <summary>
        /// Content Distrobution Url
        /// </summary>
        public const string ContentDistributionUrlKey = "Abc.CdnUrl";

        /// <summary>
        /// Blob Url
        /// </summary>
        public const string BlobUrlKey = "Abc.BlobUrl";

        /// <summary>
        /// PayPay Payment Data Transfer Key
        /// </summary>
        public const string PayPalPaymentDataTransferKey = "PayPal.PaymentDataTransfer";

        /// <summary>
        /// PayPay Payment Data Transfer Url Key
        /// </summary>
        public const string PayPalPaymentDataTransferUrlKey = "PayPal.PaymentDataTransferUrl";
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether Testing
        /// </summary>
        public static bool Testing
        {
            get
            {
                return Settings.Instance.Get<bool>(TestKey, false);
            }
        }

        /// <summary>
        /// Gets the PayPal Payment Data Transfer
        /// </summary>
        public static string PayPalPaymentDataTransfer
        {
            get
            {
                return Settings.Instance.Get<string>(PayPalPaymentDataTransferKey);
            }
        }

        /// <summary>
        /// Gets the PayPal Payment Data Transfer Url
        /// </summary>
        public static string PayPalPaymentDataTransferUrl
        {
            get
            {
                return Settings.Instance.Get<string>(PayPalPaymentDataTransferUrlKey, "https://www.paypal.com/cgi-bin/webscr");
            }
        }

        /// <summary>
        /// Gets Default Storage Account
        /// </summary>
        public static Ms.CloudStorageAccount Default
        {
            get
            {
                Contract.Ensures(Contract.Result<Ms.CloudStorageAccount>() != null);

                return Testing ? Ms.CloudStorageAccount.DevelopmentStorageAccount : Ms.CloudStorageAccount.Parse(Settings.Instance.Get(DataConnectionStringKey));
            }
        }

        /// <summary>
        /// Gets Default Storage Account
        /// </summary>
        public static Ms.CloudStorageAccount CodeStormData
        {
            get
            {
                Contract.Ensures(Contract.Result<Ms.CloudStorageAccount>() != null);

                return Testing ? Ms.CloudStorageAccount.DevelopmentStorageAccount : Ms.CloudStorageAccount.Parse(Settings.Instance.Get(CodeStormDataConnectionStringKey));
            }
        }

        /// <summary>
        /// Gets the Application Identifier
        /// </summary>
        public static Guid ApplicationIdentifier
        {
            get
            {
                return Settings.Instance.Get<Guid>(ApplicationIdentifierKey, Guid.Empty);
            }
        }

        /// <summary>
        /// Gets the CDN Content Distrobution Url
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Configuration Value")]
        public static string ContentDistributionUrl
        {
            get
            {
                return Settings.Instance.Get(ContentDistributionUrlKey, "http://cdn.agilebusinesscloud.com");
            }
        }

        /// <summary>
        /// Gets the Content Distrobution Url
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Configuration Value")]
        public static string BlobUrl
        {
            get
            {
                return Settings.Instance.Get(BlobUrlKey, "http://content.agilebusinesscloud.com");
            }
        }

        /// <summary>
        /// Gets the GitHub Client Id
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Configuration Value")]
        public static string GitHubClientId
        {
            get
            {
                return Settings.Instance[GitHubClientIdKey];
            }
        }

        /// <summary>
        /// Gets the GitHub Secret
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Configuration Value")]
        public static string GitHubSecret
        {
            get
            {
                return Settings.Instance[GitHubSecretKey];
            }
        }
        #endregion
    }
}