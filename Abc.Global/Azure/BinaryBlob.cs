// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BinaryBlob.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Microsoft.WindowsAzure;

    /// <summary>
    /// Binary Blob
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public class BinaryBlob<T>
    {
        #region Members
        /// <summary>
        /// Container
        /// </summary>
        private readonly BinaryContainer container;
        #endregion

        #region Constructors
        /// <summary>
        /// Binary Blob
        /// </summary>
        /// <param name="account">Account</param>
        public BinaryBlob(CloudStorageAccount account)
        {
            Contract.Requires<ArgumentNullException>(null != account);

            var dataStore = typeof(T).GetCustomAttribute<AzureDataStoreAttribute>(false);
            var containerName = null == dataStore ? typeof(T).Name : dataStore.Name;

            container = new BinaryContainer(account, containerName);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Save Object in Binary
        /// </summary>
        /// <param name="objectId">Object Identifier</param>
        /// <param name="item">Item</param>
        public void Save(string objectId, T item)
        {
            Contract.Requires<ArgumentNullException>(null != item);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(objectId));

            var formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            using (var memStream = new MemoryStream())
            {
                formatter.Serialize(memStream, item);
                memStream.Seek(0, SeekOrigin.Begin);

                container.Save(objectId, memStream.ToArray(), "application/bin");
            }
        }

        /// <summary>
        /// Get Object
        /// </summary>
        /// <param name="objectId">Object Identifier</param>
        /// <returns>Object</returns>
        public T Get(string objectId)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(objectId));

            var raw = container.GetBytes(objectId);
            var formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            using (var memStream = new MemoryStream(raw))
            {
                return (T)formatter.Deserialize(memStream);
            }
        }
        #endregion
    }
}