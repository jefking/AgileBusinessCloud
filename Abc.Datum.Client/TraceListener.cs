// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TraceListener.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Diagnostics
{
    using System;
    using Abc.Logging;
    using Abc.Underpinning;
    using Logging.Datum;

    /// <summary>
    /// Trace Listener, to log to the ABC Datum web service
    /// </summary>
    public class TraceListener : System.Diagnostics.TraceListener
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();

        /// <summary>
        /// Null To Log
        /// </summary>
        private const string LogNull = "null";
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether Is Thread Safe
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Write To Log
        /// </summary>
        /// <param name="message">Message</param>
        public override void Write(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                if (null != application.Token)
                {
                    var msg = new Message()
                    {
                        Message = message,
                        OccurredOn = DateTime.UtcNow,
                        MachineName = Environment.MachineName,
                        Token = application.GetToken(),
                        DeploymentId = Abc.Azure.AzureEnvironment.DeploymentId,
                        SessionIdentifier = Session.InstantSession(),
                    };

                    MessageHandler.Instance.Queue(msg);
                }
            }
        }

        /// <summary>
        /// Write Line
        /// </summary>
        /// <param name="message">Message</param>
        public override void WriteLine(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                this.Write(message);
            }
        }

        /// <summary>
        /// Fail
        /// </summary>
        /// <param name="message">Message</param>
        public override void Fail(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                this.Write("Fail: {0}".FormatWithCulture(message));
            }
        }

        /// <summary>
        /// Fail
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="detailMessage">Detail Message</param>
        public override void Fail(string message, string detailMessage)
        {
            if (!string.IsNullOrWhiteSpace(message) || !string.IsNullOrWhiteSpace(detailMessage))
            {
                this.Write("Fail: {0}; Detail: {1}".FormatWithCulture(message ?? LogNull, detailMessage ?? LogNull));
            }
        }

        /// <summary>
        /// Flush Data
        /// </summary>
        /// <remarks>
        /// Posts all Data to Web Services
        /// </remarks>
        public override void Flush()
        {
            MessageHandler.Instance.Flush();

            base.Flush();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="category">Category</param>
        public override void Write(string message, string category)
        {
            if (!string.IsNullOrWhiteSpace(message) || !string.IsNullOrWhiteSpace(category))
            {
                this.Write("{1}: {0}".FormatWithCulture(message ?? LogNull, category ?? LogNull));
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="o">Object</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.TraceListener.Write(System.String)", Justification = "Tracing implementation.")]
        public override void Write(object o)
        {
            if (null != o)
            {
                string data = o == null ? LogNull : o.ToString();
                this.Write(data);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="o">Object</param>
        /// <param name="category">Category</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.TraceListener.Write(System.String,System.String)", Justification = "Tracing implementation.")]
        public override void Write(object o, string category)
        {
            if (null != o || !string.IsNullOrWhiteSpace(category))
            {
                string data = o == null ? LogNull : o.ToString();
                this.Write(data, category);
            }
        }

        /// <summary>
        /// Write Line
        /// </summary>
        /// <param name="o">Object</param>
        public override void WriteLine(object o)
        {
            if (null != o)
            {
                this.Write(o);
            }
        }

        /// <summary>
        /// Write Line
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="category">Category</param>
        public override void WriteLine(string message, string category)
        {
            if (!string.IsNullOrWhiteSpace(message) || !string.IsNullOrWhiteSpace(category))
            {
                this.Write(message, category);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="o">Object</param>
        /// <param name="category">Category</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.TraceListener.Write(System.String,System.String)", Justification = "Tracing implementation.")]
        public override void WriteLine(object o, string category)
        {
            if (null != o || !string.IsNullOrWhiteSpace(category))
            {
                string data = o == null ? LogNull : o.ToString();
                this.Write(data, category);
            }
        }
        #endregion
    }
}