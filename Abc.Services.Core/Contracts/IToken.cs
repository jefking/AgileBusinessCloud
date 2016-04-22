// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IToken.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    /// <summary>
    /// Token Interface
    /// </summary>
    public interface IToken
    {
        #region Properties
        /// <summary>
        /// Gets the Token
        /// </summary>
        Token Token
        {
            get;
        }
        #endregion
    }
}