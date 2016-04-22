// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='DataCostCalculator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Data Cost Calculator
    /// </summary>
    public static class DataCostCalculator
    {
        #region Members
        /// <summary>
        /// Object Overhead
        /// </summary>
        /// <remarks>
        /// Holds the calculated cost of an objects property names
        /// </remarks>
        private static readonly IDictionary<Type, int> objectOverhead = new Dictionary<Type, int>();

        /// <summary>
        /// Overhead Lock
        /// </summary>
        private static readonly object overheadLock = new object();
        #endregion

        #region Methods
        /// <summary>
        /// Calculate Length of Object
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Calculated Length of Data</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated")]
        [CLSCompliant(false)]
        public static int Calculate(object data)
        {
            Contract.Requires<ArgumentNullException>(null != data);

            var type = data.GetType();
            int overheadCost;
            lock (overheadLock)
            {
                if (objectOverhead.ContainsKey(type))
                {
                    overheadCost = objectOverhead[type];
                }
                else
                {
                    overheadCost = Overhead(data.GetType());
                    objectOverhead.Add(type, overheadCost);
                }
            }

            return overheadCost + RawDataLength(data);
        }

        /// <summary>
        /// Calculate a Collection
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <returns>Calculate</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public static int Calculate(ICollection collection)
        {
            Contract.Requires<ArgumentNullException>(null != collection);

            int value = 0;
            foreach (var item in collection)
            {
                if (null != item)
                {
                    value += Calculate(item);
                }
            }

            return value;
        }

        /// <summary>
        /// Calculate Overhead
        /// </summary>
        /// <param name="type">Entity Object</param>
        /// <returns>Calculated Length of Property Names</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is validated.")]
        public static int Overhead(Type type)
        {
            Contract.Requires<ArgumentNullException>(null != type);

            int calculatedCost = 0;

            foreach (var property in type.GetProperties())
            {
                calculatedCost += Length(property.Name);
            }

            return calculatedCost;
        }

        /// <summary>
        /// Calculate Storage Cost
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Calculated Length of Data</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is validated.")]
        [CLSCompliant(false)]
        public static int RawDataLength(object data)
        {
            Contract.Requires<ArgumentNullException>(null != data);

            int calculatedCost = 0;

            var type = data.GetType();
            object propertyValue;

            foreach (var property in type.GetProperties())
            {
                try
                {
                    propertyValue = property.GetValue(data, null);
                    if (null != propertyValue)
                    {
                        calculatedCost += Length(propertyValue);
                    }
                }
                catch
                {
                }
            }

            return calculatedCost;
        }

        /// <summary>
        /// Objects Length in Bytes
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Length</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "have to check twice")]
        private static int Length(object obj)
        {
            var type = obj.GetType();
            if (type.IsEnum)
            {
                return sizeof(int);
            }
            else if (type.IsValueType)
            {
                return ((ValueType)obj).SizeOf();
            }
            else if (obj is string)
            {
                return ((string)obj).Length * sizeof(char);
            }
            else if (type.IsArray)
            {
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    return (int)ms.Length;
                }
            }
            else
            {
                return RawDataLength(obj);
            }
        }
        #endregion
    }
}