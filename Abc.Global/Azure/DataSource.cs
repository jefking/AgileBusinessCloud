// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='DataSource.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Abc.Configuration;
    using Microsoft.WindowsAzure.StorageClient;
    using Ms = Microsoft.WindowsAzure;

    /// <summary>
    /// Data Source
    /// </summary>
    public abstract class DataSource
    {
        #region Members
        /// <summary>
        /// Maximum String Length
        /// </summary>
        public const int MaximumStringLength = 32000;
        #endregion

        #region Methods
        /// <summary>
        /// Row, string is valid
        /// </summary>
        /// <remarks>
        /// Is Null, white space or empty
        /// Less than maximum length
        /// </remarks>
        /// <param name="value">Value</param>
        /// <returns>Row Is Valid</returns>
        public static bool RowIsValid(string value)
        {
            return string.IsNullOrWhiteSpace(value) || value.Length < MaximumStringLength;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="tables">Tables</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contract")]
        protected void Initialize(IEnumerable<string> tables)
        {
            Contract.Requires(null != tables);
            Contract.Requires(Contract.ForAll<string>(tables, s => !string.IsNullOrWhiteSpace(s)));

            var storageAccount = ServerConfiguration.Default;
            Parallel.ForEach(
                tables,
                (table, b) =>
                {
                    storageAccount.CreateCloudTableClient().CreateTableIfNotExist(table);
                });
        }
        #endregion
    }
}