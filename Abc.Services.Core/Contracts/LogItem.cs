// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogItem.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Log Item
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class LogItem : Secured
    {
        #region Properties
        /// <summary>
        /// Gets or sets Occurred On
        /// </summary>
        [DataMember]
        public DateTime OccurredOn { get; set; }

        /// <summary>
        /// Gets or sets Machine Name
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets Instance Id
        /// </summary>
        [DataMember]
        public string DeploymentId { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        #endregion
    }
}