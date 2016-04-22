// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='WebResponse.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Abc.Web;

    /// <summary>
    /// WebResponse
    /// </summary>
    [DataContract]
    public class WebResponse
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WebResponse class
        /// </summary>
        public WebResponse()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebResponse class
        /// </summary>
        /// <param name="errors">Errors</param>
        public WebResponse(IEnumerable<Error> errors)
        {
            this.Successful = errors == null || errors.Count() == 0;
            this.Errors = errors;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether Successful
        /// </summary>
        [DataMember]
        public bool Successful
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Errors
        /// </summary>
        [DataMember]
        public IEnumerable<Error> Errors
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Bind
        /// </summary>
        /// <param name="code">Error Code</param>
        /// <param name="message">Message</param>
        /// <returns>Web Response</returns>
        public static WebResponse Bind(int code, string message)
        {
            var error = new Error()
            {
                Code = code,
                Message = message,
            };

            var errors = new List<Error>(1);
            errors.Add(error);
            return new WebResponse(errors);
        }
        #endregion
    }
}