// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Secured.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Secured Data Access
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class Secured : IToken
    {
        #region Properties
        /// <summary>
        /// Gets or sets Token
        /// </summary>
        [DataMember]
        public Token Token { get; set; }
        #endregion
    }
}