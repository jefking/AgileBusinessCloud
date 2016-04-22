// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContentSource.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Configuration;

    /// <summary>
    /// Content Source
    /// </summary>
    public class ContentSource : DataSource
    {
        #region Members
        /// <summary>
        /// XML Data Table
        /// </summary>
        private readonly AzureTable<XmlData> xmlTable;

        /// <summary>
        /// Test Data Table
        /// </summary>
        private readonly AzureTable<TextData> textTable;

        /// <summary>
        /// Content Management System Blob Context
        /// </summary>
        private readonly ContentBlobContext blobContext = new ContentBlobContext();

        /// <summary>
        /// Binary Container
        /// </summary>
        private readonly BinaryContainer binaryContainer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ContentSource class.
        /// </summary>
        public ContentSource()
        {
            this.blobContext.Initialize(ServerConfiguration.Default);

            this.binaryContainer = new BinaryContainer(ServerConfiguration.Default, ContentBlobContext.BinaryBlobContainer);
            this.binaryContainer.EnsureExist();

            this.xmlTable = new AzureTable<XmlData>(ServerConfiguration.Default);
            this.xmlTable.EnsureExist();

            this.textTable = new AzureTable<TextData>(ServerConfiguration.Default);
            this.textTable.EnsureExist();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Insert Binary Data
        /// </summary>
        /// <param name="binary">Binary</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Identifier</returns>
        public Guid InsertBinary(byte[] binary, string contentType)
        {
            Contract.Requires<ArgumentNullException>(null != binary);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(contentType));
            Contract.Ensures(Contract.Result<Guid>() != Guid.Empty);

            Guid id = Guid.NewGuid();
            this.binaryContainer.Save(id.ToString(), binary, contentType);

            return id;
        }

        /// <summary>
        /// Insert XML
        /// </summary>
        /// <param name="xml">XML Content</param>
        /// <returns>XML Content Identifier</returns>
        public Guid InsertXml(string xml)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(xml));

            Guid id = Guid.NewGuid();
            var store = this.blobContext.Xml;
            var blob = store.GetBlobReference(id.ToString());
            blob.Properties.ContentType = "text/xml";
            blob.UploadText(xml);

            return id;
        }

        /// <summary>
        /// Insert Text into blob
        /// </summary>
        /// <param name="text">Text Content</param>
        /// <returns>Text Content Identifier</returns>
        public Guid InsertText(string text)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(text));

            var id = Guid.NewGuid();
            var store = this.blobContext.Text;
            var blob = store.GetBlobReference(id.ToString());
            blob.Properties.ContentType = "text";
            blob.UploadText(text);

            return id;
        }

        /// <summary>
        /// Update Xml
        /// </summary>
        /// <param name="id">Xml Identifier</param>
        /// <param name="xml">New Xml Content</param>
        public void UpdateXml(Guid id, string xml)
        {
            Contract.Requires(Guid.Empty != id);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(xml));

            var store = this.blobContext.Xml;
            var blob = store.GetBlobReference(id.ToString());
            if (blob.Exists())
            {
                blob.DeleteIfExists();
            }

            blob = store.GetBlobReference(id.ToString());
            blob.UploadText(xml);
        }

        /// <summary>
        /// Update Text
        /// </summary>
        /// <param name="id">Text Identifier</param>
        /// <param name="text">New Text Content</param>
        public void UpdateText(Guid id, string text)
        {
            Contract.Requires(Guid.Empty != id);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(text));

            var store = this.blobContext.Text;
            var blob = store.GetBlobReference(id.ToString());
            if (blob.Exists())
            {
                blob.DeleteIfExists();
            }

            blob = store.GetBlobReference(id.ToString());
            blob.UploadText(text);
        }

        /// <summary>
        /// Select Binary Data
        /// </summary>
        /// <param name="id">Binary Id</param>
        /// <returns>Binary Data</returns>
        public byte[] SelectBinary(Guid id)
        {
            Contract.Requires<ArgumentOutOfRangeException>(id != Guid.Empty);

            return this.binaryContainer.GetBytes(id.ToString());
        }

        /// <summary>
        /// Select Xml Data
        /// </summary>
        /// <param name="id">Xml Id</param>
        /// <returns>Xml Data</returns>
        public string SelectXml(Guid id)
        {
            Contract.Requires<ArgumentOutOfRangeException>(id != Guid.Empty);
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var binStore = this.blobContext.Xml;
            var blob = binStore.GetBlobReference(id.ToString());

            if (blob.Exists())
            {
                return blob.DownloadText();
            }
            else
            {
                throw new InvalidOperationException("Unknown identifier.");
            }
        }

        /// <summary>
        /// Select Text Data
        /// </summary>
        /// <param name="id">Text Id</param>
        /// <returns>Text Data</returns>
        public string SelectText(Guid id)
        {
            Contract.Requires<ArgumentOutOfRangeException>(id != Guid.Empty);
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var binStore = this.blobContext.Text;
            var blob = binStore.GetBlobReference(id.ToString());

            if (blob.Exists())
            {
                return blob.DownloadText();
            }
            else
            {
                throw new InvalidOperationException("Unknown identifier.");
            }
        }

        /// <summary>
        /// Insert Text Data
        /// </summary>
        /// <param name="data">Text Data</param>
        /// <returns>Id</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is validated."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Not Intended API"), CLSCompliant(false)]
        public Guid Insert(TextData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(Guid.Empty != data.Id, "Id is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != data.ApplicationId, "Application Id is empty.");

            this.textTable.AddEntity(data);

            return data.Id;
        }

        /// <summary>
        /// Update Text Data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Guid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is validated."), CLSCompliant(false)]
        public Guid Update(TextData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(data.RowKey), "Row Key is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != data.Id, "Id is empty.");
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(data.PartitionKey), "Patition Key is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != data.ApplicationId, "Application Id is empty.");

            this.textTable.AddOrUpdateEntity(data);

            return data.Id;
        }

        /// <summary>
        /// Select Text Data
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <param name="id">Id</param>
        /// <returns>Text</returns>
        [CLSCompliant(false)]
        public TextData SelectText(Guid applicationId, Guid id)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId, "Application Id is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != id, "Id is empty.");

            string partitionKey = applicationId.ToString();
            string rowKey = id.ToString();
            
            return this.textTable.QueryBy(partitionKey, rowKey);
        }

        /// <summary>
        /// Insert XML Data
        /// </summary>
        /// <param name="data">XML Data</param>
        /// <returns>Id</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is validated."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Not Intended API"), CLSCompliant(false)]
        public Guid Insert(XmlData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(data.RowKey), "Row Key is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != data.Id, "Id is empty.");
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(data.PartitionKey), "Patition Key is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != data.ApplicationId, "Application Id is empty.");

            this.xmlTable.AddEntity(data);

            return data.Id;
        }

        /// <summary>
        /// Update XML Data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Guid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is validated."), CLSCompliant(false)]
        public Guid Update(XmlData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(data.RowKey), "Row Key is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != data.Id, "Id is empty.");
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(data.PartitionKey), "Patition Key is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != data.ApplicationId, "Application Id is empty.");

            this.xmlTable.AddOrUpdateEntity(data);

            return data.Id;
        }

        /// <summary>
        /// Select XML Data
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <param name="id">Id</param>
        /// <returns>Text</returns>
        [CLSCompliant(false)]
        public XmlData SelectXml(Guid applicationId, Guid id)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId, "Application Id is empty.");
            Contract.Requires<ArgumentException>(Guid.Empty != id, "Id is empty.");

            string partitionKey = applicationId.ToString();
            string rowKey = id.ToString();
            
            return this.xmlTable.QueryBy(partitionKey, rowKey);
        }

        /// <summary>
        /// Contract Invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(null != this.textTable);
            Contract.Invariant(null != this.xmlTable);
            Contract.Invariant(null != this.blobContext);
            Contract.Invariant(null != this.binaryContainer);
        }
        #endregion
    }
}