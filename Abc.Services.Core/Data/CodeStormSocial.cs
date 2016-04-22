// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='CodeStormSocial.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using Abc.Azure;
    using System;
    using System.Collections.Generic;

    [Serializable]
    [AzureDataStore("social")]
    public class CodeStormSocial
    {
        #region Properties
        /// <summary>
        /// Gets and sets ABC Handle
        /// </summary>
        public string AbcHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets Twitter Handle
        /// </summary>
        public string TwitterHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets GitHub Handle
        /// </summary>
        public string GitHubHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Twitter Links
        /// </summary>
        public IEnumerable<string> TwitterLinks
        {
            get;
            set;
        }

        /// <summary>
        /// Twitter Mentions
        /// </summary>
        public IEnumerable<Mention> TwitterMentions
        {
            get;
            set;
        }
        #endregion
    }
}