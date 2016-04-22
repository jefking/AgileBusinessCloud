// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationQuery.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Application Query
    /// </summary>
    [DataContract]
    public class ApplicationQuery : IApplicationIdentifier
    {
        #region Properties
        /// <summary>
        /// Gets or sets Application Identifier
        /// </summary>
        [DataMember]
        public Guid ApplicationIdentifier
        {
            get;
            set;
        }
        #endregion
    }
}