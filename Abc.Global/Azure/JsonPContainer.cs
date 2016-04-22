// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='JsonContainer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using Microsoft.WindowsAzure;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// JSONP Container
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class JsonPContainer<T> : JsonContainer<T>
    {
        #region Properties
        /// <summary>
        /// Json Format
        /// </summary>
        public const string JsonPFormat = "{0}({1})";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the JSON P Container class
        /// </summary>
        /// <param name="account">Storage Account</param>
        /// <param name="method"Method></param>
        public JsonPContainer(CloudStorageAccount account, string method)
            : base(account)
        {
            Contract.Requires<ArgumentNullException>(null != account);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(method));

            this.Method = method;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Method
        /// </summary>
        public string Method
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Serialized Object</returns>
        protected override string Serialize(T obj, out string contentType)
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.ValueAtReturn<string>(out contentType)));

            var serialized = base.Serialize(obj, out contentType);
            return string.Format(JsonPFormat, this.Method, serialized);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="serialized">Serialized</param>
        /// <returns>Object</returns>
        protected override T Deserialize(string serialized)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}