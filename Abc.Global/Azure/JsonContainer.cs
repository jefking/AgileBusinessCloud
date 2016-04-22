// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='JsonContainer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Script.Serialization;
    using Microsoft.WindowsAzure;
    
    /// <summary>
    /// Json Container
    /// </summary>
    /// <typeparam name="T">ISerializable</typeparam>
    public class JsonContainer<T> : TextContainer<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the JsonContainer class
        /// </summary>
        /// <param name="account">Storage Account</param>
        public JsonContainer(CloudStorageAccount account)
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
        protected override string Serialize(T obj, out string contentType)
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.ValueAtReturn<string>(out contentType)));

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            else
            {
                contentType = "application/json";
                var serializer = new JavaScriptSerializer()
                {
                    MaxJsonLength = int.MaxValue,
                };

                return serializer.Serialize(obj);
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
            else
            {
                var serializer = new JavaScriptSerializer()
                {
                    MaxJsonLength = int.MaxValue,
                };

                return serializer.Deserialize<T>(serialized);
            }
        }
        #endregion
    }
}