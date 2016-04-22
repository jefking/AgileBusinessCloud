// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BinaryContainer.cs'>
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
    /// Binary Container
    /// </summary>
    public class BinaryContainer : AzureBlobContainer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BinaryContainer class
        /// </summary>
        /// <param name="account">Storage Account</param>
        /// <param name="containerName">Container Name</param>
        public BinaryContainer(CloudStorageAccount account, string containerName)
            : base(account, containerName)
        {
            Contract.Requires<ArgumentNullException>(null != account);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(containerName));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="objId">Oject Id</param>
        /// <param name="content">Content</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Uri</returns>
        public Uri Save(string objId, byte[] content, string contentType)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(objId));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(contentType));

            Contract.Assume(TimeSpan.Zero < this.Container.ServiceClient.Timeout);
            Contract.Assume(TimeSpan.MaxValue > this.Container.ServiceClient.Timeout);

            return this.Save(objId, content, contentType, this.Container.ServiceClient.Timeout);
        }

        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="objId">Oject Id</param>
        /// <param name="content">Content</param>
        /// <param name="contentType">Content Type</param>
        /// <param name="timeout">Time Out</param>
        /// <returns>Uri</returns>
        public Uri Save(string objId, byte[] content, string contentType, TimeSpan timeout)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(objId));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(contentType));
            Contract.Requires<ArgumentOutOfRangeException>(TimeSpan.Zero < timeout);
            Contract.Requires<ArgumentOutOfRangeException>(TimeSpan.MaxValue > timeout);

            var currentTimeOut = this.Container.ServiceClient.Timeout;
            this.Container.ServiceClient.Timeout = timeout;

            var blob = this.Container.GetBlobReference(objId);
            blob.UploadByteArray(content);

            blob.Properties.ContentType = contentType;
            blob.SetProperties();

            this.Container.ServiceClient.Timeout = currentTimeOut;
            return blob.Uri;
        }
        #endregion
    }
}