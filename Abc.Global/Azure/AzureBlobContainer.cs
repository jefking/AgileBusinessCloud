// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AzureBlobContainer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Azure Blob Container
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public abstract class AzureBlobContainer : IAzureStorage
    {
        #region Members
        /// <summary>
        /// Container
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Correct")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Correct visibility")]
        protected readonly CloudBlobContainer Container;

        /// <summary>
        /// Account
        /// </summary>
        private readonly CloudStorageAccount account;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AzureBlobContainer class
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="containerName">Container Name</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Containers must be lower case")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code Contracts")]
        protected AzureBlobContainer(CloudStorageAccount account, string containerName)
        {
            Contract.Requires<ArgumentNullException>(null != account);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(containerName));

            this.account = account;

            var client = this.account.CreateCloudBlobClient();
            client.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(5));

            this.Container = client.GetContainerReference(containerName.ToLowerInvariant());
        }
        #endregion

        #region Methods
        /// <summary>
        /// Container Exists
        /// </summary>
        /// <returns>Exists</returns>
        public bool Exists()
        {
            return this.Container.Exists();
        }

        /// <summary>
        /// Ensure Exists
        /// </summary>
        public void EnsureExist()
        {
            this.Container.CreateIfNotExist();
        }

        /// <summary>
        /// Ensure Exists
        /// </summary>
        /// <param name="publicContainer">Public Container</param>
        public void EnsureExist(bool publicContainer)
        {
            this.EnsureExist();

            var permissions = new BlobContainerPermissions();

            if (publicContainer)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            }

            this.Container.SetPermissions(permissions);
        }

        /// <summary>
        /// Delete If Exists
        /// </summary>
        public void DeleteIfExist()
        {
            this.Container.Delete();
        }

        /// <summary>
        /// Get File
        /// </summary>
        /// <param name="objId">Object Id</param>
        /// <returns>Stream</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "If we dispose the stream the clien won't be able to use it")]
        public Stream GetStream(string objId)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(objId));

            var stream = new MemoryStream();
            var blob = this.Container.GetBlobReference(objId);
            blob.DownloadToStream(stream);
            stream.Seek(0, 0);
            return stream;
        }

        /// <summary>
        /// Get Bytes
        /// </summary>
        /// <param name="objId">Object Id</param>
        /// <returns>Bytes</returns>
        public byte[] GetBytes(string objId)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(objId));

            using (var stream = this.GetStream(objId) as MemoryStream)
            {
                try
                {
                    return stream.ToArray();
                }
                catch (StorageClientException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="objId">Object Id</param>
        public void Delete(string objId)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(objId));

            var blob = this.Container.GetBlobReference(objId);
            blob.DeleteIfExists();
        }

        /// <summary>
        /// Contract Invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(null != this.account);
            Contract.Invariant(null != this.Container);
        }
        #endregion
    }
}