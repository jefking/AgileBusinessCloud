// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethods.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region Methods
        /// <summary>
        /// Is Valid Table
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Is Valid</returns>
        public static bool IsValidTable(this string value)
        {
            return Regex.IsMatch(value, "^[A-Za-z][A-Za-z0-9]*");
        }

        /// <summary>
        /// Is Valid Blob Container
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Is Valid</returns>
        public static bool IsValidBlobContainer(this string value)
        {
            return Regex.IsMatch(value, @"^[a-z0-9](([a-z0-9\-[^\-])){1,61}[a-z0-9]$");
        }
        #endregion
    }
}