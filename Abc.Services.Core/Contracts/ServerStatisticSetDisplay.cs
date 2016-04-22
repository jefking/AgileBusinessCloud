// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerStatisticSetDisplay.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using Abc.Azure;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Server Statistic Set Display
    /// </summary>
    [Serializable]
    [DataContract]
    public class ServerStatisticSetDisplay : ServerStatisticSet, IIdentifier<Guid>
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
        /// Gets or sets the Server Statistic Set Display Range
        /// </summary>
        [DataMember]
        public IEnumerable<ServerStatisticSetDisplay> Range
        {
            get;
            set;
        }
        #endregion
    }
}