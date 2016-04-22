// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethods.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Text;
    using LinqToTwitter;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region System.Net.WebException
        /// <summary>
        /// Convert an FTP-based WebException into our custom DatumFault enum
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>DatumFault Error Code</returns>
        internal static DatumFault ErrorCodeValue(this WebException ex)
        {
            Contract.Requires(null != ex);
            Contract.Ensures((int)Contract.Result<DatumFault>() >= 1001 && (int)Contract.Result<DatumFault>() <= 1200);

            switch (ex.Status)
            {
                case WebExceptionStatus.NameResolutionFailure:
                    return DatumFault.FtpInvalidUrl;

                case WebExceptionStatus.ProxyNameResolutionFailure:
                    return DatumFault.FtpProxyNameResolutionFailure;

                case WebExceptionStatus.ConnectFailure:
                    return DatumFault.FtpConnectFailure;

                case WebExceptionStatus.ReceiveFailure:
                    return DatumFault.FtpReceiveResponseFailure;

                case WebExceptionStatus.SendFailure:
                case WebExceptionStatus.RequestProhibitedByCachePolicy:
                    return DatumFault.FtpSendRequestFailure;

                case WebExceptionStatus.RequestCanceled:
                    return DatumFault.FtpRequestCanceled;

                case WebExceptionStatus.RequestProhibitedByProxy:
                    return DatumFault.FtpRequestProhibitedByProxy;

                case WebExceptionStatus.ProtocolError:
                    return DatumFault.FtpProtocolError;

                case WebExceptionStatus.ConnectionClosed:
                case WebExceptionStatus.PipelineFailure:
                case WebExceptionStatus.KeepAliveFailure:
                    return DatumFault.FtpConnectionClosed;

                case WebExceptionStatus.TrustFailure:
                    return DatumFault.FtpTrustFailure;

                case WebExceptionStatus.SecureChannelFailure:
                    return DatumFault.FtpSecureChannelFailure;

                case WebExceptionStatus.ServerProtocolViolation:
                    return DatumFault.FtpServerProtocolViolation;

                case WebExceptionStatus.Timeout:
                    return DatumFault.FtpTimeout;

                case WebExceptionStatus.MessageLengthLimitExceeded:
                    return DatumFault.FtpMessageLengthLimitExceeded;

                case WebExceptionStatus.CacheEntryNotFound:
                    return DatumFault.FtpCacheEntryNotFound;

                default:
                    return DatumFault.FtpUnknownError;
            }
        }
        #endregion

        #region Abc.Azure.AzureTable<T>
        /// <summary>
        /// Query Table for Log Data
        /// </summary>
        /// <typeparam name="T">Log Data Type</typeparam>
        /// <param name="table">Table</param>
        /// <param name="query">Query</param>
        /// <returns>Results</returns>
        [CLSCompliant(false)]
        public static IQueryable<T> Query<T>(this AzureTable<T> table, LogQuery query)
            where T : LogData, new()
        {
            Contract.Requires<ArgumentNullException>(null != query);
            Contract.Requires<ArgumentNullException>(null != query.From);
            Contract.Requires<ArgumentNullException>(null != query.To);
            Contract.Requires<ArgumentNullException>(null != query.Top);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(query.PartitionKey));

            return table.QueryBy(d => d.PartitionKey == query.PartitionKey && d.Timestamp > query.From.Value && d.Timestamp < query.To.Value, query.Top.Value);
        }
        #endregion

        #region Abc.Azure.BinaryBlob<T>
        /// <summary>
        /// Get Digest History
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="blob">Blob</param>
        /// <param name="objectId">Object Identifier</param>
        /// <returns>Log History</returns>
        public static T GetDigest<T>(this BinaryBlob<T> blob, string objectId)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(objectId));

            try
            {
                using (var perf = new PerformanceMonitor())
                {
                    perf.Append(objectId);
                    return blob.Get(objectId);
                }
            }
            catch (Exception ex)
            {
                var log = new LogCore();
                log.Log(ex, EventTypes.Warning, (int)ServiceFault.NotInDigest);
            }

            return default(T);
        }
        #endregion

        #region LinqToTwitter.Status
        /// <summary>
        /// Render HTML
        /// </summary>
        /// <param name="status">Status</param>
        /// <returns>Render HTML</returns>
        [CLSCompliant(false)]
        public static IEnumerable<string> ParseLinks(this Status status)
        {
            var data = new List<string>();
            if (!string.IsNullOrWhiteSpace(status.Text))
            {
                var sb = new StringBuilder(status.Text);
                foreach (Match match in RegexStatement.Url.Matches(sb.ToString()))
                {
                    data.Add(match.Groups["Url"].Value);
                }
            }

            return data;
        }

        /// <summary>
        /// Parse Mentions
        /// </summary>
        /// <param name="status">Status</param>
        /// <returns>Mentions</returns>
        [CLSCompliant(false)]
        public static IEnumerable<string> ParseMentions(this Status status)
        {
            var data = new List<string>();
            if (!string.IsNullOrWhiteSpace(status.Text))
            {
                var sb = new StringBuilder(status.Text);
                foreach (Match match in RegexStatement.TwitterFollower.Matches(sb.ToString()))
                {
                    data.Add(match.Groups["Value"].Value);
                }
            }

            return data;
        }
        #endregion
    }
}