// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='XmlContainer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.WindowsAzure;

    /// <summary>
    /// Xml Container
    /// </summary>
    /// <typeparam name="T">ISerializable</typeparam>
    public class XmlContainer<T> : TextContainer<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the XmlContainer class
        /// </summary>
        /// <param name="account">Storage Account</param>
        public XmlContainer(CloudStorageAccount account)
            : base(account)
        {
            Contract.Requires(null != account);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Serialized Object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.IO.StringWriter.#ctor", Justification = "Default behavior")]
        protected override string Serialize(T obj, out string contentType)
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.ValueAtReturn<string>(out contentType)));

            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }

            contentType = "text/xml";
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="serialized">Serialized</param>
        /// <returns>Object</returns>
        protected override T Deserialize(string serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized))
            {
                throw new ArgumentNullException("serialized");
            }

            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(serialized))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
        #endregion
    }
}