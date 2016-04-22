// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='MessageDisplay.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Message for Display
    /// </summary>
    [DataContract]
    [Serializable]
    public class MessageDisplay : Message, IIdentifier<Guid>
    {
        #region Properties
        /// <summary>
        /// Identifier
        /// </summary>
        [DataMember]
        public Guid Identifier
        {
            get;
            set;
        }
        #endregion
    }
}