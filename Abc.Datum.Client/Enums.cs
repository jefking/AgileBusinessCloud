// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Enums.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Logging
{
    using System;

    #region Event Types
    /// <summary>
    /// Event Types
    /// </summary>
    [Flags]
    public enum EventTypes
    {
        /// <summary>
        /// Default, Initialization value
        /// </summary>
        None = 0,

        /// <summary>
        /// Fatal error or application crash.
        /// </summary>
        Critical = 1,

        /// <summary>
        /// Recoverable error.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Noncritical problem.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Informational message.
        /// </summary>
        Information = 8,

        /// <summary>
        /// Debugging trace.
        /// </summary>
        Verbose = 16,

        /// <summary>
        /// Starting of a logical operation.
        /// </summary>
        Start = 256,

        /// <summary>
        /// Stopping of a logical operation.
        /// </summary>
        Stop = 512,

        /// <summary>
        /// Suspension of a logical operation.
        /// </summary>
        Suspend = 1024,

        /// <summary>
        /// Resumption of a logical operation.
        /// </summary>
        Resume = 2048,

        /// <summary>
        /// Changing of correlation identity.
        /// </summary>
        Transfer = 4096
    }
    #endregion
}