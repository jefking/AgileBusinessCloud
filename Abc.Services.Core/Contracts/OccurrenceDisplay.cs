// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='OccurrenceDisplay.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Abc.Azure;

    /// <summary>
    /// Occurrence Display
    /// </summary>
    [DataContract]
    [Serializable]
    public class OccurrenceDisplay: Occurrence, IIdentifier<Guid>
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