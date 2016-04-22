// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Mailgun.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// Mail Gun class for sending email through third party.
    /// </summary>
    public static class MailGun
    {
        #region Members
        /// <summary>
        /// Log
        /// </summary>
        private static readonly LogCore log = new LogCore();

        /// <summary>
        /// Sending Server Name
        /// </summary>
        private const string ServerName = "best";

        /// <summary>
        /// API Url
        /// </summary>
        private static readonly string apiUrl = Settings.Instance.Get(ConfigurationKeys.MailGunApiKey, "https://mailgun.net/api/");
        #endregion

        #region Methods
        /// <summary>
        /// Send plain-text message 
        /// </summary>
        /// <param name="sender">sender specification</param>
        /// <param name="recipients">comma- or semicolon-separated list of recipients specifications</param>
        /// <param name="subject">message subject</param>
        /// <param name="text">message text</param>
        public static void Send(string sender, string recipients, string subject, string text)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(sender));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(recipients));
            Contract.Requires<ArgumentNullException>(null != subject);
            Contract.Requires<ArgumentNullException>(null != text);

            var req = new NameValueCollection();
            req.Add("sender", sender);
            req.Add("recipients", recipients);
            req.Add("subject", subject);
            req.Add("body", text);

            byte[] data = FormData(req);
            HttpWebRequest wr = OpenRequest(MessageUrl("txt", ServerName), "POST");
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.ContentLength = data.Length;
            try
            {
                using (Stream rs = wr.GetRequestStream())
                {
                    rs.Write(data, 0, data.Length);
                }

                using (wr.GetResponse())
                {
                }
            }
            catch (WebException wex)
            {
                log.Log(wex, EventTypes.Error, (int)DatumFault.EmailSendFailure);
            }
        }

        /// <summary>
        /// Send raw mime message 
        /// </summary>
        /// <param name="sender">sender specification</param>
        /// <param name="recipients">comma- or semicolon-separated list of recipient specifications</param>
        /// <param name="rawMime">mime-encoded message body</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Code contracts")]
        public static void Send(string sender, string recipients, byte[] rawMime)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(sender));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(recipients));
            Contract.Requires<ArgumentNullException>(null != rawMime);
            Contract.Requires<ArgumentOutOfRangeException>(0 < rawMime.Length);

            var wr = OpenRequest(MessageUrl("mime", ServerName), "POST");
            var req = Encoding.UTF8.GetBytes("{0}\n{1}\n\n".FormatWithCulture(sender, recipients));
            wr.ContentLength = req.Length + rawMime.Length;
            wr.ContentType = "text/plain";
            try
            {
                using (var rs = wr.GetRequestStream())
                {
                    rs.Write(req, 0, req.Length);
                    rs.Write(rawMime, 0, rawMime.Length);
                }

                using (wr.GetResponse())
                {
                }
            }
            catch (WebException wex)
            {
                log.Log(wex, EventTypes.Error, (int)DatumFault.EmailSendFailure);
            }
        }

        /// <summary>
        /// Formatted Message Url
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="servername">Server Name</param>
        /// <returns>Message Url</returns>
        private static string MessageUrl(string format, string servername)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(format));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(servername));
            
            return "{0}messages.{1}?servername={2}".FormatWithCulture(new Uri(apiUrl), format, servername);
        }

        /// <summary>
        /// Get WWW Form Data
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>Ascii Encoded</returns>
        private static byte[] FormData(NameValueCollection parameters)
        {
            Contract.Requires(null != parameters);

            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Count; ++i)
            {
                sb.AppendFormat("{0}={1}&", parameters.GetKey(i), HttpUtility.UrlEncode(parameters.Get(i), Encoding.UTF8));
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Open Request
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="method">Method</param>
        /// <returns>HTTP Web Request</returns>
        private static HttpWebRequest OpenRequest(string url, string method)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(url));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(method));

            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = method;
            wr.Proxy = null;

            var credentials = new CredentialCache();
            credentials.Add(new Uri(apiUrl), "Basic", new NetworkCredential("api_key", Settings.Instance.Get(ConfigurationKeys.MailGunApiKeyKey)));
            wr.Credentials = credentials;

            return wr;
        }
        #endregion
    }
}