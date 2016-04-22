// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TextContainer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Text Container
    /// </summary>
    /// <typeparam name="T">ISerializable</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors", Justification = "General settings")]
    public abstract class TextContainer<T> : AzureBlobContainer, ITextContainer<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TextContainer class
        /// </summary>
        /// <param name="account">Storage Account</param>
        protected TextContainer(CloudStorageAccount account)
            : base(account, ContainerName(typeof(T)))
        {
            Contract.Requires(null != account);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="objId">Oject Id</param>
        /// <param name="obj">Object</param>
        public void Save(string objId, T obj)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(objId));
            Contract.Requires<ArgumentNullException>(null != obj);

            var blob = this.Container.GetBlobReference(objId);
            string contentType;
            var serialized = this.Serialize(obj, out contentType);
            blob.Properties.ContentType = contentType;
            blob.UploadText(serialized);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="objId">Object Id</param>
        /// <returns>Object</returns>
        public T Get(string objId)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(objId));

            var blob = this.Container.GetBlobReference(objId);
            try
            {
                return this.Deserialize(blob.DownloadText());
            }
            catch (StorageClientException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Serialized Object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Simple")]
        protected abstract string Serialize(T obj, out string contentType);

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="serialized">Serialized</param>
        /// <returns>Object</returns>
        protected abstract T Deserialize(string serialized);

        /// <summary>
        /// Container Name
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Table Name</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Containers must be lower case.")]
        private static string ContainerName(Type type)
        {
            Contract.Requires<ArgumentNullException>(null != type);

            var store = type.GetCustomAttribute<AzureDataStoreAttribute>(false);
            return (null == store ? type.Name : store.Name).ToLowerInvariant();
        }
        #endregion
    }
}