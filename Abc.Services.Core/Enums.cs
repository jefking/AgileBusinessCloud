// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Enums.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    #region Error Codes
    /// <summary>
    /// Fault Codes
    /// </summary>
    /// <remarks>
    /// Range from 1001 to 2000
    /// </remarks>
    public enum DatumFault
    {
        /// <summary>
        /// Empty
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 1001,

        /// <summary>
        /// Log Exception Failure
        /// </summary>
        LogException = 1002,

        /// <summary>
        /// Log Message Failure
        /// </summary>
        LogMessage = 1003,

        /// <summary>
        /// Log Performance Failure
        /// </summary>
        LogPerformance = 1004,

        /// <summary>
        /// No Uri Specified
        /// </summary>
        NoUriSpecified = 1005,

        /// <summary>
        /// Log Server Statistic Set
        /// </summary>
        LogServerStatisticSet = 1006,

        /// <summary>
        /// Log Event Item
        /// </summary>
        LogEventItem = 1007,

        // FTP-Based Error Codes
        // From 1101 to 1200

        /// <summary>
        /// FTP Unknown Error
        /// </summary>
        FtpUnknownError = 1101,

        /// <summary>
        /// FTP Invalid URL
        /// </summary>
        FtpInvalidUrl = 1102,

        /// <summary>
        /// FTP Permission Denied
        /// </summary>
        FtpPermissionDenied = 1103,

        /// <summary>
        /// FTP Recieve Response Failure
        /// </summary>
        FtpReceiveResponseFailure = 1104,

        /// <summary>
        /// FTP Connect Failure
        /// </summary>
        FtpConnectFailure = 1105,

        /// <summary>
        /// FTP Send Request Failure
        /// </summary>
        FtpSendRequestFailure = 1106,

        /// <summary>
        /// FTP Request Canceled
        /// </summary>
        FtpRequestCanceled = 1107,

        /// <summary>
        /// FTP Protocol Error
        /// </summary>
        FtpProtocolError = 1108,

        /// <summary>
        /// FTP Connection Closed
        /// </summary>
        FtpConnectionClosed = 1109,

        /// <summary>
        /// FTP Trust Failure
        /// </summary>
        FtpTrustFailure = 1110,

        /// <summary>
        /// FTP Secure Channel Failure
        /// </summary>
        FtpSecureChannelFailure = 1111,

        /// <summary>
        /// FTP Server Protocol Violation
        /// </summary>
        FtpServerProtocolViolation = 1112,

        /// <summary>
        /// FTP Proxy Name Resolution Failure
        /// </summary>
        FtpProxyNameResolutionFailure = 1113,

        /// <summary>
        /// FTP Message Length Limit Exceeded
        /// </summary>
        FtpMessageLengthLimitExceeded = 1114,

        /// <summary>
        /// FTP Request Prohibited by Proxy
        /// </summary>
        FtpRequestProhibitedByProxy = 1115,

        /// <summary>
        /// FTP Time out
        /// </summary>
        FtpTimeout = 1116,

        /// <summary>
        /// FTP Cache Entry not Found
        /// </summary>
        FtpCacheEntryNotFound = 1117,

        /// <summary>
        /// FTP Communication Failure
        /// </summary>
        FtpCommunicationFailure = 1119,

        /// <summary>
        /// FTP Data Conversion Failure
        /// </summary>
        FtpDataConversionFailure = 1120,

        // Twitter-Based Error Codes
        // From 1201 to 1250

        /// <summary>
        /// General Twitter Failure
        /// </summary>
        GeneralTwitterFailure = 1202,

        // Email-Based Error Codes
        // From 1251 to 1300

        /// <summary>
        /// Binary Email Unknown Error
        /// </summary>
        BinaryEmailUnknownError = 1251,

        /// <summary>
        /// Plain Text Email Unknown Error
        /// </summary>
        PlaintextEmailUnknownError = 1252,

        /// <summary>
        /// Email Send Failure
        /// </summary>
        EmailSendFailure = 1253,

        // Configuration-Based Error Codes
        // From 1301 to 1350

        /// <summary>
        /// Get Configuration Unknown Error
        /// </summary>
        GetConfigurationUnknownError = 1301,

        // Data-Based Error Codes
        // From 1351 to 1450

        /// <summary>
        /// Data Exists Failure
        /// </summary>
        DataExistsFailure = 1351,

        /// <summary>
        /// Log Metric
        /// </summary>
        LogMetric = 1352,

        /// <summary>
        /// Analytics
        /// </summary>
        Analytics = 1353,
    }

    /// <summary>
    /// Fault Codes
    /// </summary>
    /// <remarks>
    /// Range from 2001 to 3000
    /// </remarks>
    public enum ContentFault
    {
        /// <summary>
        /// Empty
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 2001,

        /// <summary>
        /// Get Text Failure
        /// </summary>
        GetText = 2002,

        /// <summary>
        /// Save Text Failure
        /// </summary>
        SaveText = 2003,

        /// <summary>
        /// Get XML Failure
        /// </summary>
        GetXml = 2004,

        /// <summary>
        /// Save XML Failure
        /// </summary>
        SaveXml = 2005,

        /// <summary>
        /// XML Content not valid XML
        /// </summary>
        ContentInvalidXml = 2005,

        /// <summary>
        /// Get Text Validation Failure
        /// </summary>
        GetTextValidation = 2006,

        /// <summary>
        /// Save Text Validation Failure
        /// </summary>
        SaveTextValidation = 2007,

        /// <summary>
        /// Get Binary Failure
        /// </summary>
        GetBinary = 2002,
    }

    /// <summary>
    /// Fault Codes
    /// </summary>
    /// <remarks>
    /// Range from 2001 to 3000
    /// </remarks>
    public enum ServiceFault
    {
        /// <summary>
        /// Empty
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 2001,

        /// <summary>
        /// Message Digest Failure
        /// </summary>
        MessageDigest = 2002,

        /// <summary>
        /// Messages Not In Digest
        /// </summary>
        MessagesNotInDigest = 2003,

        /// <summary>
        /// Performance Digest Failure
        /// </summary>
        PerformanceDigest = 2003,

        /// <summary>
        /// Blob Digest Failure
        /// </summary>
        BlobDigest = 2004,

        /// <summary>
        /// Not In Digest
        /// </summary>
        NotInDigest = 2005,

        /// <summary>
        /// Profile Doesn't exist
        /// </summary>
        ProfileDoesntExist = 2006,

        /// <summary>
        /// Cannot Create Profile
        /// </summary>
        CannotCreateProfile = 2007,
    }
    #endregion

    #region Data Cost Type
    /// <summary>
    /// Data Cost Type
    /// </summary>
    public enum DataCostType
    {
        /// <summary>
        /// Stored
        /// </summary>
        Stored = 0,

        /// <summary>
        /// Egress
        /// </summary>
        Egress = 1,

        /// <summary>
        /// Ingress
        /// </summary>
        Ingress = 2,
    }
    #endregion

    #region Role Type
    /// <summary>
    /// Role Type
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Client
        /// </summary>
        Client = 0,

        /// <summary>
        /// Manager
        /// </summary>
        Manager = 1
    }
    #endregion
}