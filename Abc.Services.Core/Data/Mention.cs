// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Mention.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;

    /// <summary>
    /// Mention
    /// </summary>
    [Serializable]
    public class Mention
    {
        #region Properties
        /// <summary>
        /// Gets and sets Twitter Handle
        /// </summary>
        public string TwitterHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets ABC Handle
        /// </summary>
        public string AbcHandle
        {
            get;
            set;
        }
        #endregion
    }
}