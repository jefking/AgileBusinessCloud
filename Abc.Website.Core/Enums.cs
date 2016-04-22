// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Enums.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website
{
    /// <summary>
    /// Website Fault enum for logging
    /// </summary>
    public enum Fault
    {
        /// <summary>
        /// None, Default Value
        /// </summary>
        None = 0,

        /// <summary>
        /// Unknown Fault
        /// </summary>
        Unknown = 1001,

        /// <summary>
        /// Account Controller Change Password
        /// </summary>
        AccountControllerChangePassword = 1002,

        /// <summary>
        /// Multitenant Update Sites
        /// </summary>
        MultitenantUpdateSites = 1003,

        /// <summary>
        /// Multitenant Sync Forever
        /// </summary>
        MultitenantSynchronizeRepeatedly = 1004,

        /// <summary>
        /// Multitenant Site Certificate
        /// </summary>
        MultitenantSiteCertificate = 1005,

        /// <summary>
        /// Multitenant Site Certificate
        /// </summary>
        UnableToOpenMultitenantSiteCertificate = 1006,

        /// <summary>
        /// Open Id Protocol Exception
        /// </summary>
        OpenIdProtocolException = 1007,

        /// <summary>
        /// Open Id Failure during third party authentication
        /// </summary>
        OpenIdThirdPartyFailure = 1008,

        /// <summary>
        /// Invalid Application Identifier
        /// </summary>
        InvalidApplicationIdentifier = 1009,

        /// <summary>
        /// Invalid User Type
        /// </summary>
        InvalidUserType = 1010,

        /// <summary>
        /// Data Not Specified
        /// </summary>
        DataNotSpecified = 1011,

        /// <summary>
        /// Unknown User
        /// </summary>
        UnknownUser = 1012,

        /// <summary>
        /// Certificate Not Specified
        /// </summary>
        CertificateNotSpecified = 1013,

        /// <summary>
        /// Twitter Failure
        /// </summary>
        TwitterFailure = 1014,

        /// <summary>
        /// Invalid Identifier
        /// </summary>
        InvalidIdentifier = 1015
    }

    /// <summary>
    /// Website Notification enum for logging
    /// </summary>
    public enum Notification
    {
        /// <summary>
        /// None, Default Value
        /// </summary>
        None = 0,

        /// <summary>
        /// Unknown Notification
        /// </summary>
        Unknown = 10001
    }
}