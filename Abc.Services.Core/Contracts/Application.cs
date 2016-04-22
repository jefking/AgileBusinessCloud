// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Application.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using Abc.Configuration;
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Application
    /// </summary>
    [DataContract]
    public class Application : IIdentifier<Guid>
    {
        #region Properties
        /// <summary>
        /// Gets the Current Application
        /// </summary>
        public static Application Current
        {
            get
            {
                Contract.Ensures(Contract.Result<Application>() != null);

                return new Application()
                {
                    Identifier = ServerConfiguration.ApplicationIdentifier,
                };
            }
        }

        /// <summary>
        /// Gets the Default Application
        /// </summary>
        public static Application Default
        {
            get
            {
                Contract.Ensures(Contract.Result<Application>() != null);

                return new Application()
                {
                    Identifier = new Guid("3BD8FBF6-E89A-4FE0-8369-A314519D1F6F"),
                };
            }
        }

        /// <summary>
        /// Gets or sets Identifier
        /// </summary>
        [DataMember]
        public Guid Identifier
        {
            get;
            set;
        }
        #endregion
    }
}