// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='MessageHandler.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Logging
{
    using System;
    using System.Threading;
    using Abc.Collections;
    using Abc.Logging.Datum;

    /// <summary>
    /// Message Handler
    /// </summary>
    internal class MessageHandler : IDisposable
    {
        #region Members
        /// <summary>
        /// Errors
        /// </summary>
        private readonly FirstInFirstOut<ErrorItem> errors = new FirstInFirstOut<ErrorItem>();

        /// <summary>
        /// Messages
        /// </summary>
        private readonly FirstInFirstOut<Message> messages = new FirstInFirstOut<Message>();

        /// <summary>
        /// Occurences
        /// </summary>
        private readonly FirstInFirstOut<Occurrence> ocurrences = new FirstInFirstOut<Occurrence>();

        /// <summary>
        /// Event Log Entries
        /// </summary>
        private readonly FirstInFirstOut<EventLogItem> eventLogEntries = new FirstInFirstOut<EventLogItem>();

        /// <summary>
        /// Server Statistic Sets
        /// </summary>
        private readonly FirstInFirstOut<ServerStatisticSet> serverStatisticSets = new FirstInFirstOut<ServerStatisticSet>();

        /// <summary>
        /// Message Handler
        /// </summary>
        /// <remarks>
        /// Singleton
        /// </remarks>
        private static readonly MessageHandler handler = new MessageHandler();

        /// <summary>
        /// Logging Timer
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Disposed
        /// </summary>
        private bool disposed = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the MessageHandler class from being created.
        /// </summary>
        private MessageHandler()
        {
            this.timer = new Timer(this.Save, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Instance
        /// </summary>
        internal static MessageHandler Instance
        {
            get
            {
                return handler;
            }
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
        /// Queue Error Item
        /// </summary>
        /// <param name="error">Error Item</param>
        internal void Queue(ErrorItem error)
        {
            if (null != error)
            {
                this.errors.Enqueue(error);
            }
        }

        /// <summary>
        /// Queue Event Log Item
        /// </summary>
        /// <param name="item">Event Log Item</param>
        internal void Queue(EventLogItem item)
        {
            if (null != item)
            {
                this.eventLogEntries.Enqueue(item);
            }
        }

        /// <summary>
        /// Queue Message
        /// </summary>
        /// <param name="message">Message</param>
        internal void Queue(Message message)
        {
            if (null != message)
            {
                this.messages.Enqueue(message);
            }
        }

        /// <summary>
        /// Queue Occurrence
        /// </summary>
        /// <param name="occurence">Occurrence</param>
        internal void Queue(Occurrence occurence)
        {
            if (null != occurence)
            {
                this.ocurrences.Enqueue(occurence);
            }
        }

        /// <summary>
        /// Queue Server Statistic Set
        /// </summary>
        /// <param name="serverSet">Server Statistic Set</param>
        internal void Queue(ServerStatisticSet serverSet)
        {
            if (null != serverSet)
            {
                this.serverStatisticSets.Enqueue(serverSet);
            }
        }

        /// <summary>
        /// Flush Buffer
        /// </summary>
        internal void Flush()
        {
            while (0 < this.ocurrences.Count ||
                0 < this.messages.Count ||
                0 < this.errors.Count ||
                0 < this.eventLogEntries.Count ||
                0 < this.serverStatisticSets.Count)
            {
                this.Save(null);
            }
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
                    this.Flush();

                    if (null != this.timer)
                    {
                        this.timer.Dispose();
                    }

                    this.timer = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="info">Null</param>
        private void Save(object info)
        {
            var error = this.errors.Dequeue();
            var message = this.messages.Dequeue();
            var occurence = this.ocurrences.Dequeue();
            var eventItem = this.eventLogEntries.Dequeue();
            var serverSet = this.serverStatisticSets.Dequeue();

            if (null != error
                || null != message
                || null != occurence
                || null != eventItem
                || null != serverSet)
            {
                using (var proxy = new Proxy())
                {
                    if (null != error)
                    {
                        proxy.Log(error);
                    }

                    if (null != message)
                    {
                        proxy.Log(message);
                    }

                    if (null != occurence)
                    {
                        proxy.Log(occurence);
                    }

                    if (null != eventItem)
                    {
                        proxy.Log(eventItem);
                    }

                    if (null != serverSet)
                    {
                        proxy.Log(serverSet);
                    }
                }
            }
        }
        #endregion
    }
}