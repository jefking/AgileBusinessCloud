// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredQuery.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Bytes Stored Query
    /// </summary>
    [DataContract]
    public class BytesStoredQuery : ApplicationQuery
    {
        #region Properties
        /// <summary>
        /// Gets or sets Top
        /// </summary>
        [DataMember]
        public int? Top
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets To
        /// </summary>
        [DataMember]
        public DateTime? To
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets From
        /// </summary>
        [DataMember]
        public DateTime? From
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets DataCostType
        /// </summary>
        [DataMember]
        public DataCostType? DataCostType
        {
            get;
            set;
        }
        #endregion
    }
}