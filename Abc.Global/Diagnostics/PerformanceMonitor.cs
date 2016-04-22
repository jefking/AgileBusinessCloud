// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceMonitor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Performance Monitor
    /// </summary>
    public abstract class PerformanceMonitor : IDisposable
    {
        #region Members
        /// <summary>
        /// Started On
        /// </summary>
        private readonly Stopwatch stopwatch = null;

        /// <summary>
        /// Method
        /// </summary>
        private readonly MethodBase method = null;

        /// <summary>
        /// Write Performance Monitor is Turned off.
        /// </summary>
        private static volatile bool writePerfTurnedOff = true;

        /// <summary>
        /// Message
        /// </summary>
        private StringBuilder content = null;

        /// <summary>
        /// Disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Session Identifier
        /// </summary>
        protected readonly Guid SessionIdentifier = Guid.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PerformanceMonitor class
        /// </summary>
        protected PerformanceMonitor()
        {
            var stackTrace = new StackTrace();
            foreach (var stackFrame in stackTrace.GetFrames())
            {
                var method = stackFrame.GetMethod();
                if (method.IsConstructor)
                {
                    continue;
                }
                else
                {
                    this.method = method;
                    break;
                }
            }

            stackTrace = null;

            this.stopwatch = Stopwatch.StartNew();
            this.SessionIdentifier = Session.GetSession();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Duration
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return this.stopwatch.Elapsed;
            }
        }

        /// <summary>
        /// Gets the Duration
        /// </summary>
        public virtual TimeSpan MinimumDuration
        {
            get
            {
                return new TimeSpan(0, 0, 0, 1);
            }
        }

        /// <summary>
        /// Gets Content
        /// </summary>
        public string Content
        {
            get
            {
                return this.content == null ? null : this.content.ToString();
            }
        }

        /// <summary>
        /// Gets MethodName
        /// </summary>
        protected string MethodName
        {
            get
            {
                return (null == this.method) ? null : this.method.ToString();
            }
        }

        /// <summary>
        /// Gets ClassName
        /// </summary>
        protected string ClassName
        {
            get
            {
                return (null == this.method.DeclaringType) ? null : this.method.DeclaringType.FullName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether To Log
        /// </summary>
        protected virtual bool ToLog
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Append Performance Info
        /// </summary>
        /// <param name="format">If data is populated, this should be message plus a format string, ie: "something happened Id={0} TimeStamp={1}"</param>
        /// <param name="data">Data Payload</param>
        public void Append(string format, params object[] data)
        {
            if (null != format && null != data)
            {
                this.content = this.content ?? new StringBuilder();
                this.content.AppendFormat(CultureInfo.InvariantCulture, format, data);
            }
        }

        /// <summary>
        /// Append Performance Info
        /// </summary>
        /// <param name="message">Message to Append</param>
        public void Append(string message)
        {
            if (null != message)
            {
                this.content = this.content ?? new StringBuilder();
                this.content.Append(message);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Log Occurence
        /// </summary>
        /// <param name="duration">Duration</param>
        protected abstract void LogOccurrence(TimeSpan duration);

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (null != this.stopwatch)
                    {
                        this.stopwatch.Stop();
                    }

                    if (Guid.Empty != this.SessionIdentifier)
                    {
                        Session.ReleaseSession();
                    }

                    if (this.ToLog)
                    {
                        if (this.MinimumDuration < this.Duration)
                        {
                            this.LogOccurrence(this.Duration);
                        }
                    }
                    else if (writePerfTurnedOff)
                    {
                        Trace.Write("Performance Data logging is turned off.");
                        writePerfTurnedOff = false;
                    }
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}