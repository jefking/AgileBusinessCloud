// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='WindowsAzureQueue.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Windows Azure Queue
    /// </summary>
    /// <typeparam name="T">Queued Object Type</typeparam>
    public class WindowsAzureQueue<T>
    {
        #region Members
        /// <summary>
        /// Cloud Queue Client
        /// </summary>
        private readonly CloudQueueClient client = null;

        /// <summary>
        /// Cloud Queue
        /// </summary>
        private readonly CloudQueue queue = null;

        /// <summary>
        /// Maximum Visibility Timeout
        /// </summary>
        public static readonly TimeSpan MaximumVisibilityTimeout = new TimeSpan(7, 0, 0, 0);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WindowsAzureQueue class
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="queueReference">Queue Reference</param>
        /// <param name="create">Create</param>
        public WindowsAzureQueue(CloudStorageAccount account, string queueReference, bool create = true)
        {
            Contract.Requires<ArgumentNullException>(null != account);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(queueReference));

            this.client = account.CreateCloudQueueClient();
            this.queue = this.client.GetQueueReference(queueReference);
            if (create)
            {
                var created = this.queue.CreateIfNotExist();
                if (created)
                {
                    Trace.Write("New Queue Created: '{0}'.".FormatWithCulture(queueReference));
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Retrieve Approximate Message Count
        /// </summary>
        public int RetrieveApproximateMessageCount
        {
            get
            {
                return this.queue.RetrieveApproximateMessageCount();
            }
        }

        /// <summary>
        /// Gets the Approximate Message Count
        /// </summary>
        public int ApproximateMessageCount
        {
            get
            {
                return this.queue.ApproximateMessageCount ?? 0;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Queue Object
        /// </summary>
        /// <param name="toQueue">To Queue</param>
        public void Queue(T toQueue)
        {
            Contract.Requires<ArgumentNullException>(null != toQueue);

            var message = new CloudQueueMessage(toQueue.Serialize());

            this.queue.AddMessage(message);
        }

        /// <summary>
        /// Peek next message
        /// </summary>
        /// <returns>Message</returns>
        public T Peek()
        {
            var message = this.queue.PeekMessage();
            return message.AsBytes.Deserialize<T>();
        }

        /// <summary>
        /// Get Message
        /// </summary>
        /// <returns>Message</returns>
        public T Get()
        {
            var message = this.queue.GetMessage();
            return message.AsBytes.Deserialize<T>();
        }

        /// <summary>
        /// Get Messages
        /// </summary>
        /// <param name="messageCount">Message Count</param>
        /// <returns>Messages</returns>
        public IEnumerable<T> Get(int messageCount = 1)
        {
            Contract.Requires<ArgumentException>(0 < messageCount);

            return this.Get(messageCount, MaximumVisibilityTimeout.Subtract(new TimeSpan(0, 0, 0, 0, 1)));
        }

        /// <summary>
        /// Get Messages
        /// </summary>
        /// <param name="messageCount">Message Count</param>
        /// <param name="visibilityTimeout">Visibility Timeout</param>
        /// <returns>Messages</returns>
        public IEnumerable<T> Get(int messageCount, TimeSpan visibilityTimeout)
        {
            Contract.Requires<ArgumentException>(0 < messageCount);
            Contract.Requires<ArgumentException>(32 >= messageCount);
            Contract.Requires<ArgumentException>(new TimeSpan(0, 0, 1) <= visibilityTimeout);
            Contract.Requires<ArgumentException>(new TimeSpan(7, 0, 0, 0) >= visibilityTimeout);

            var message = this.queue.GetMessages(messageCount, visibilityTimeout);
            return message.Select(i => i.AsBytes.Deserialize<T>());
        }

        /// <summary>
        /// Invariant Contract
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Contract Invariant")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invariant Contract")]
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(null != this.client);
            Contract.Invariant(null != this.queue);
        }
        #endregion
    }
}