// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='FtpResponse.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// FTP Response
    /// </summary>
    public class FtpResponse
    {
        #region Properties
        /// <summary>
        /// Gets or sets Data
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Web Service Data"), DataMember]
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FailureCode
        /// </summary>
        [DataMember]
        public int FailureCode
        {
            get;
            set;
        }
        #endregion
    }
}