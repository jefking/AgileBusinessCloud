// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStored.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Bytes Stored
    /// </summary>
    [DataContract]
    public class BytesStored
    {
        #region Properties
        /// <summary>
        /// Gets or sets Application Identifier
        /// </summary>
        [DataMember]
        public Guid ApplicationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Data Cost Type
        /// </summary>
        [DataMember]
        public int DataCostType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Occurred On
        /// </summary>
        [DataMember]
        public DateTime OccurredOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Bytes
        /// </summary>
        [DataMember]
        public int Bytes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Object Type
        /// </summary>
        [DataMember]
        public string ObjectType
        {
            get;
            set;
        }
        #endregion
    }
}