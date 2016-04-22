// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContentBlobContext.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Content Blob Context
    /// </summary>
    public class ContentBlobContext
    {
        #region Members
        /// <summary>
        /// Binary Blob Container
        /// </summary>
        public const string BinaryBlobContainer = "binaryblobcontainer";

        /// <summary>
        /// XML Blob Container
        /// </summary>
        public const string XmlBlobContainer = "xmlblobcontainer";

        /// <summary>
        /// Text Blob Container
        /// </summary>
        public const string TextBlobContainer = "textblobcontainer";

        /// <summary>
        /// Containers
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "not a security issue here")]
        public static readonly IList<string> Containers = new ReadOnlyCollection<string>(new string[] { TextBlobContainer, XmlBlobContainer });

        /// <summary>
        /// Containers
        /// </summary>
        private IDictionary<string, CloudBlobContainer> containers = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets Xml
        /// </summary>
        public CloudBlobContainer Xml
        {
            get
            {
                return this.containers[XmlBlobContainer];
            }
        }

        /// <summary>
        /// Gets Text
        /// </summary>
        public CloudBlobContainer Text
        {
            get
            {
                return this.containers[TextBlobContainer];
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="account">Cloud Storage Account</param>
        public void Initialize(CloudStorageAccount account)
        {
            Contract.EnsuresOnThrow<ArgumentNullException>(null == account);

            if (null == this.containers)
            {
                CloudBlobContainer blobContainer;

                var client = account.CreateCloudBlobClient();

                this.containers = new Dictionary<string, CloudBlobContainer>(Containers.Count);

                foreach (string container in Containers)
                {
                    blobContainer = client.GetContainerReference(container);
                    blobContainer.CreateIfNotExist();
                    this.containers.Add(container, blobContainer);
                }
            }
        }
        #endregion
    }
}