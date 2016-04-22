// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Validator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Validator
    /// </summary>
    /// <typeparam name="T">Validation Class</typeparam>
    public class Validator<T>
        where T : IValidate<T>
    {
        #region Properties
        /// <summary>
        /// Gets Null Object Error Message
        /// </summary>
        private static string NullObjectErrorMessage
        {
            get
            {
                return "{0} is null.".FormatWithCulture(typeof(T));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// All Messages
        /// </summary>
        /// <param name="validate">To Validate</param>
        /// <returns>Messatges</returns>
        public string AllMessages(T validate)
        {
            var sb = new StringBuilder();
            foreach (var error in this.Validate(validate))
            {
                sb.AppendFormat("{0}{1}", error, Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="validate">To Validate</param>
        /// <param name="testForNull">Test for Null</param>
        /// <returns>Validation Messages</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Justification = "name is fine")]
        public IEnumerable<string> Validate(T validate, bool testForNull)
        {
            return testForNull && null == validate ? new string[] { NullObjectErrorMessage } : this.Validate(validate);
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="validate">To Validate</param>
        /// <returns>Validation Messages</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Justification = "name is fine")]
        public IEnumerable<string> Validate(T validate)
        {
            return this.Items(validate).Select(r => r.Message);
        }

        /// <summary>
        /// Checks whether is valid
        /// </summary>
        /// <param name="validate">To Validate</param>
        /// <param name="testForNull">Test for Null</param>
        /// <returns>Is Valid</returns>
        public bool IsValid(T validate, bool testForNull)
        {
            return testForNull && null == validate ? false : this.IsValid(validate);
        }

        /// <summary>
        /// Checks whether is valid
        /// </summary>
        /// <param name="validate">To Validate</param>
        /// <returns>Is Valid</returns>
        public bool IsValid(T validate)
        {
            return !this.Items(validate).Any();
        }

        /// <summary>
        /// Items
        /// </summary>
        /// <param name="t">To Validate</param>
        /// <returns>Rules</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        private IEnumerable<Rule<T>> Items(T t)
        {
            if (null == t)
            {
                throw new ArgumentNullException("t");
            }
            else
            {
                return t.Rules.Where(r => !r.Test(t));
            }
        }
        #endregion
    }
}