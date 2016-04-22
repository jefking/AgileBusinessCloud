// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IValidate.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Validation
{
    using System.Collections.Generic;

    /// <summary>
    /// Validation Interface
    /// </summary>
    /// <typeparam name="T">Validation Class</typeparam>
    public interface IValidate<T>
    {
        #region Properties
        /// <summary>
        /// Gets Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Inline rule set")]
        IEnumerable<Rule<T>> Rules { get; }
        #endregion
    }
}