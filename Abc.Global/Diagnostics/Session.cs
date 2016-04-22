// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Session.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Session
    /// </summary>
    public sealed class Session
    {
        #region Members
        /// <summary>
        /// Sessions
        /// </summary>
        private static readonly IDictionary<int, Session> sessions = new Dictionary<int, Session>();

        /// <summary>
        /// Lock Object
        /// </summary>
        private static readonly object lockObject = new object();
        #endregion

        #region Session
        /// <summary>
        /// Initializes a new instance of the Session class
        /// </summary>
        public Session()
        {
            this.Count = 1;
            this.Identifier = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets and sets the Count
        /// </summary>
        private int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the Identifier
        /// </summary>
        public Guid Identifier
        {
            get;
            private set;
        }
        #endregion

        #region Members
        /// <summary>
        /// Get Session
        /// </summary>
        /// <returns>Session Identifier</returns>
        public static Guid GetSession()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObject)
            {
                if (sessions.ContainsKey(threadId))
                {
                    var session = sessions[threadId];
                    session.Count++;
                }
                else
                {
                    sessions.Add(threadId, new Session());
                }
            }

            return sessions[threadId].Identifier;
        }

        /// <summary>
        /// Release Session
        /// </summary>
        public static void ReleaseSession()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObject)
            {
                if (sessions.ContainsKey(threadId))
                {
                    var session = sessions[threadId];
                    session.Count--;
                    if (0 == sessions.Count)
                    {
                        sessions.Remove(threadId);
                    }
                }
            }
        }

        /// <summary>
        /// Instant Session
        /// </summary>
        /// <returns>Session Identifier</returns>
        public static Guid InstantSession()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObject)
            {
                return sessions.ContainsKey(threadId) ? sessions[threadId].Identifier : Guid.NewGuid();
            }
        }
        #endregion
    }
}