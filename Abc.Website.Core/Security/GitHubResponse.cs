// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='GitHubResponse.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Security
{
    using Abc.Text;
    using System;

    /// <summary>
    /// GitHub Response
    /// </summary>
    public class GitHubResponse
    {
        #region Properties
        /// <summary>
        /// Gets and sets Access Token
        /// </summary>
        public string AccessToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets Token Type
        /// </summary>
        public string TokenType
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse Response from Data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Response</returns>
        public static GitHubResponse Parse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentException("data");
            }
            else
            {
                var matches = RegexStatement.GitHubAuthenticationResponse.Match(data);
                return new GitHubResponse()
                {
                    AccessToken = matches.Groups["accessToken"].Value,
                    TokenType = matches.Groups["tokenType"].Value,
                };
            }
        }
        #endregion
    }
}