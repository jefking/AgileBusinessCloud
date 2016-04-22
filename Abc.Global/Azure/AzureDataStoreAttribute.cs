// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AzureDataStoreAttribute.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Azure Data Store
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AzureDataStoreAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AzureDataStoreAttribute class.
        /// </summary>
        /// <param name="name">Name</param>
        public AzureDataStoreAttribute(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            this.Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
        #endregion
    }
}