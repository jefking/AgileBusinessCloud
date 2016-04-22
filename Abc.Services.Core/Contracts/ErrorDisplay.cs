// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ErrorDisplay.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Error Display
    /// </summary>
    [DataContract]
    [Serializable]
    public class ErrorDisplay : ErrorItem, IIdentifier<Guid>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Identifier
        /// </summary>
        [DataMember]
        public Guid Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Parent Identifier
        /// </summary>
        [DataMember]
        public Guid ParentIdentifier
        {
            get;
            set;
        }
        #endregion
    }
}