// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Error.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Web
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Error
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "Data Contract")]
    [DataContract]
    public class Error
    {
        #region Properties
        /// <summary>
        /// Gets or sets Code
        /// </summary>
        [DataMember]
        public int Code
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        [DataMember]
        public string Message
        {
            get;
            set;
        }
        #endregion
    }
}