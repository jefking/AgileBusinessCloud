// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PartitionKeyComparer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Partition Key Comparer
    /// </summary>
    internal class PartitionKeyComparer : IEqualityComparer<TableServiceEntity>
    {
        #region MEthods
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">Entity X</param>
        /// <param name="y">Entity Y</param>
        /// <returns>Are Equal</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code Contract")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contract")]
        public bool Equals(TableServiceEntity x, TableServiceEntity y)
        {
            if (null == x)
            {
                throw new ArgumentNullException("x");
            }
            else if (null == y)
            {
                throw new ArgumentNullException("y");
            }

            return string.Compare(x.PartitionKey, y.PartitionKey, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Hash</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contract")]
        public int GetHashCode(TableServiceEntity obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.PartitionKey.GetHashCode();
        }
        #endregion
    }
}