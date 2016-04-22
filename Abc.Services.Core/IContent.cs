// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IContent.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System.Diagnostics.Contracts;
    using System.ServiceModel;
    using Abc.Services.Contracts;

    /// <summary>
    /// Content Interface
    /// </summary>
    [ServiceContract]// (ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IContent
    {
        #region Methods
        /// <summary>
        /// Get Binary
        /// </summary>
        /// <param name="data">Binary Content</param>
        /// <returns>Binary Content Data</returns>
        [OperationContract]
        BinaryContent GetBinary(BinaryContent data);

        /// <summary>
        /// Save Binary Content
        /// </summary>
        /// <param name="data">Binary Content</param>
        /// <returns>Binary Content Id</returns>
        [OperationContract]
        BinaryContent SaveBinary(BinaryContent data);

        /// <summary>
        /// Get Text
        /// </summary>
        /// <param name="data">Text Data</param>
        /// <returns>Text Data Filled</returns>
        [OperationContract]
        TextContent GetText(TextContent data);

        /// <summary>
        /// Save Text
        /// </summary>
        /// <param name="data">Text Data</param>
        /// <returns>Text Data Id</returns>
        [OperationContract]
        TextContent SaveText(TextContent data);

        /// <summary>
        /// Get XML
        /// </summary>
        /// <param name="data">XML Data</param>
        /// <returns>XML Data Filled</returns>
        [OperationContract]
        XmlContent GetXml(XmlContent data);

        /// <summary>
        /// Save XML
        /// </summary>
        /// <param name="data">XML Data</param>
        /// <returns>XML Data Id</returns>
        [OperationContract]
        XmlContent SaveXml(XmlContent data);
        #endregion
    }
}