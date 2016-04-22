// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='CommunicationFailureException.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Custom exception to be thrown when remote communication fails.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "Simple Serialization"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Passing Datum Fault"), Serializable]
    public class CommunicationFailureException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CommunicationFailureException class.
        /// </summary>
        public CommunicationFailureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommunicationFailureException class.
        /// </summary>
        /// <param name="message">Exception error message.</param>
        /// <param name="fault">Datum Fault</param>
        public CommunicationFailureException(string message, DatumFault fault)
            : base(message)
        {
            Contract.Requires(1001 <= (int)fault && 1200 >= (int)fault);

            this.FaultCodeValue = (int)fault;
        }

        /// <summary>
        /// Initializes a new instance of the CommunicationFailureException class.
        /// </summary>
        /// <param name="message">Exception error message.</param>
        /// <param name="innerException">Inner exception.</param>
        /// <param name="fault">Datum Fault</param>
        public CommunicationFailureException(string message, Exception innerException, DatumFault fault)
            : base(message, innerException)
        {
            Contract.Requires(1001 <= (int)fault && 1200 >= (int)fault);

            this.FaultCodeValue = (int)fault;
        }

        /// <summary>
        /// Initializes a new instance of the CommunicationFailureException class.
        /// </summary>
        /// <param name="serialization">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected CommunicationFailureException(SerializationInfo serialization, StreamingContext context)
            : base(serialization, context)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Fault Code Value
        /// </summary>
        public int FaultCodeValue
        {
            get;
            private set;
        }
        #endregion
    }
}