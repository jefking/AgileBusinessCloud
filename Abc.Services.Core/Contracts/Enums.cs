// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Enums.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    #region Event Type
    /// <summary>
    /// Event Type
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Data Contract")]
    [Flags]
    [DataContract]
    public enum EventTypes
    {
        /// <summary>
        /// Unknown event, Event Type not specified
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Fatal error or application crash.
        /// </summary>
        [EnumMember]
        Critical = 1,

        /// <summary>
        /// Recoverable error.
        /// </summary>
        [EnumMember]
        Error = 2,

        /// <summary>
        /// Noncritical problem.
        /// </summary>
        [EnumMember]
        Warning = 4,

        /// <summary>
        /// Informational message.
        /// </summary>
        [EnumMember]
        Information = 8,

        /// <summary>
        /// Debugging trace.
        /// </summary>
        [EnumMember]
        Verbose = 16,

        /// <summary>
        /// Starting of a logical operation.
        /// </summary>
        [EnumMember]
        Start = 256,

        /// <summary>
        /// Stopping of a logical operation.
        /// </summary>
        [EnumMember]
        Stop = 512,

        /// <summary>
        /// Suspension of a logical operation.
        /// </summary>
        [EnumMember]
        Suspend = 1024,

        /// <summary>
        /// Resumption of a logical operation.
        /// </summary>
        [EnumMember]
        Resume = 2048,

        /// <summary>
        /// Changing of correlation identity.
        /// </summary>
        [EnumMember]
        Transfer = 4096
    }
    #endregion

    #region Event Log Entry Type
    /// <summary>
    /// Event Log Entry Type, for Windows Event Logs
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Data Contract")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Data Contract")]
    [Flags]
    [DataContract]
    public enum EventLogEntryType
    {
        /// <summary>
        /// Default value, not set
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// An error event. This indicates a significant problem the user should know about; usually a loss of functionality or data.
        /// </summary>
        [EnumMember]
        Error = 1,

        /// <summary>
        /// A warning event. This indicates a problem that is not immediately significant, but that may signify conditions that could cause future problems.
        /// </summary>
        [EnumMember]
        Warning = 2,

        /// <summary>
        /// An information event. This indicates a significant, successful operation.
        /// </summary>
        [EnumMember]
        Information = 4,

        /// <summary>
        /// A success audit event. This indicates a security event that occurs when an audited access attempt is successful; for example, logging on successfully.
        /// </summary>
        [EnumMember]
        SuccessAudit = 8,

        /// <summary>
        /// A failure audit event. This indicates a security event that occurs when an audited access attempt fails; for example, a failed attempt to open a file.
        /// </summary>
        [EnumMember]
        FailureAudit = 16,
    }
    #endregion
}
