// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='SafeTimer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Threading
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    /// Safe Timer
    /// </summary>
    public class SafeTimer : IDisposable
    {
        #region Methods
        /// <summary>
        /// On Error
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Event needs to be visible")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "namespace", Justification = "Re Use")]
        public EventHandler<EventArgs<Exception>> OnError;

        /// <summary>
        /// Timer
        /// </summary>
        private Timer timer = null;

        /// <summary>
        /// Timer Callback
        /// </summary>
        private TimerCallback callback;

        /// <summary>
        /// Disposed
        /// </summary>
        private bool disposed = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SafeTimer class
        /// </summary>
        /// <param name="callback">Callback</param>
        /// <param name="state">State</param>
        /// <param name="dueTime">Due Time</param>
        /// <param name="period">Period</param>
        public SafeTimer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
            : this(callback, null, state, dueTime, period)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SafeTimer class
        /// </summary>
        /// <param name="callback">Callback</param>
        /// <param name="onError">On Error</param>
        /// <param name="state">State</param>
        /// <param name="dueTime">Due Time</param>
        /// <param name="period">Period</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "namespace", Justification = "Re Use")]
        public SafeTimer(TimerCallback callback, EventHandler<EventArgs<Exception>> onError, object state, TimeSpan dueTime, TimeSpan period)
        {
            this.callback = callback;
            this.OnError = onError;
            this.timer = new Timer(this.SafeCallback, state, dueTime, period);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

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
                    if (null != this.timer)
                    {
                        this.timer.Dispose();
                    }

                    this.timer = null;
                    this.OnError = null;
                    this.disposed = true;
                }
            }
        }

        /// <summary>
        /// Safe Callbak
        /// </summary>
        /// <param name="state">State</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        private void SafeCallback(object state)
        {
            try
            {
                var handler = this.callback;
                if (null != handler)
                {
                    handler(state);
                }
            }
            catch (Exception ex)
            {
                var handler = this.OnError;
                if (null != handler)
                {
                    try
                    {
                        handler(this, new EventArgs<Exception>(ex));
                    }
                    catch
                    {
                        // Safety
                    }
                }
            }
        }
        #endregion
    }
}