// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethods.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using System.Text;
    using Abc.Text;

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region System.String
        /// <summary>
        /// Format With Culture
        /// </summary>
        /// <param name="format">String To Be Formatted</param>
        /// <param name="data">Data</param>
        /// <returns>Formatted String</returns>
        public static string FormatWithCulture(this string format, params object[] data)
        {
            Contract.Requires(null != format);

            Contract.Ensures(Contract.Result<string>() != null);

            Contract.Assume(null != data);

            return string.Format(CultureInfo.CurrentCulture, format, data);
        }

        /// <summary>
        /// StartsWith With Culture
        /// </summary>
        /// <param name="startsWith">Starts With</param>
        /// <param name="value">Data</param>
        /// <returns>Formatted String</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Is Validated")]
        public static bool StartsWithWithCulture(this string startsWith, string value)
        {
            Contract.Requires(null != startsWith);
            Contract.Requires(null != value);

            return startsWith.StartsWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Converts this string to a Guid.
        /// </summary>
        /// <param name="convert">String to convert.</param>
        /// <returns>Resulting Guid</returns>
        public static Guid ToGuid(this string convert)
        {
            Contract.Requires(null != convert);

            try
            {
                return new Guid(convert);
            }
            catch (FormatException)
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Converts this string to a Guid which is encoded in Base64
        /// </summary>
        /// <param name="convert">String to convert</param>
        /// <returns>Resulting Guid</returns>
        public static Guid ToGuidBase64(this string convert)
        {
            Contract.Requires(null != convert);

            return new Guid(System.Convert.FromBase64String("{0}==".FormatWithCulture(convert)));
        }

        /// <summary>
        /// Converts this string to a Guid which is encoded in ASCII85
        /// </summary>
        /// <param name="convert">String to convert</param>
        /// <returns>Resulting Guid</returns>
        public static Guid ToGuidAscii85(this string convert)
        {
            Contract.Requires(null != convert);

            var ascii = new Ascii85();
            var bytes = ascii.Decode(convert);
            if (null != bytes
                && 16 == bytes.Length)
            {
                return new Guid(bytes);
            }
            else
            {
                throw new FormatException("Invalid string for decoding into Guid.");
            }
        }

        /// <summary>
        /// Convert String to Byte[]
        /// </summary>
        /// <param name="convert">String to Convert</param>
        /// <returns>Bytes</returns>
        public static byte[] ToBytes(this string convert)
        {
            Contract.Requires(null != convert);

            return Encoding.Default.GetBytes(convert);
        }

        /// <summary>
        /// Get Hex MD5
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>MD5 in Hexidecimal format</returns>
        public static string GetHexMD5(this string value)
        {
            var data = value.ToBytes();
            var md5 = data.MD5Hash();
            return md5.GetHexadecimal();
        }

        /// <summary>
        /// Converts String to Byte[]
        /// </summary>
        /// <param name="value">Hexidecimal</param>
        /// <returns>Bytes</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring.")]
        public static byte[] FromHex(this string value)
        {
            Contract.Requires<ArgumentException>(1 < value.Length);
            Contract.Requires<ArgumentException>(0 == value.Length % 2);

            var length = value.Length;
            var bytes = new char[length / 2][];
            var bits = value.ToCharArray();
            for (uint i = 0; i < length / 2; i++)
            {
                bytes[i] = new char[] { bits[i * 2], bits[(i * 2) + 1] };
            }

            var converted = new byte[bytes.LongLength];
            for (long i = 0; i < bytes.LongLength; i++)
            {
                converted[i] = byte.Parse(bytes[i][0].ToString() + bytes[i][1], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }

            return converted;
        }
        #endregion

        #region System.ValueType
        /// <summary>
        /// Size Of
        /// </summary>
        /// <param name="valueType">Value Type</param>
        /// <returns>Size</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "I am validating with a contract.")]
        public static int SizeOf(this ValueType valueType)
        {
            Contract.Requires(null != valueType);

            if (valueType is int)
            {
                return sizeof(int);
            }
            else if (valueType is char)
            {
                return sizeof(char);
            }
            else if (valueType is bool)
            {
                return sizeof(bool);
            }
            else if (valueType is DateTime)
            {
                return 8;
            }
            else if (valueType is TimeSpan)
            {
                return 8;
            }
            else if (valueType is Guid)
            {
                return 16;
            }
            else if (valueType is long)
            {
                return sizeof(long);
            }
            else if (valueType is double)
            {
                return sizeof(double);
            }
            else if (valueType is decimal)
            {
                return sizeof(decimal);
            }
            else if (valueType is float)
            {
                return sizeof(float);
            }
            else if (valueType is uint)
            {
                return sizeof(uint);
            }
            else if (valueType is ulong)
            {
                return sizeof(ulong);
            }
            else if (valueType is short)
            {
                return sizeof(short);
            }
            else if (valueType is ushort)
            {
                return sizeof(ushort);
            }
            else if (valueType is byte)
            {
                return sizeof(byte);
            }
            else if (valueType is sbyte)
            {
                return sizeof(sbyte);
            }
            else
            {
                throw new InvalidOperationException("Unknown Value Type '{0}'.".FormatWithCulture(valueType.GetType()));
            }
        }
        #endregion

        #region System.ICollection
        /// <summary>
        /// Bubble Sort
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <returns>Sorted Collection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Collection cannot be null.")]
        public static ICollection BubbleSort(this ICollection collection)
        {
            Contract.Requires(null != collection);

            Contract.Ensures(Contract.Result<ICollection>() != null);
            Contract.Ensures(Contract.Result<ICollection>().Count == collection.Count);

            if (0 == collection.Count)
            {
                return collection;
            }

            IComparable ptr;
            IComparable[] array = new IComparable[collection.Count];
            collection.CopyTo(array, 0);
            for (int i = 0; i < array.LongLength; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (0 > array[i].CompareTo(array[j]))
                    {
                        ptr = array[j];
                        array[j] = array[i];
                        array[i] = ptr;
                    }
                }
            }

            return array;
        }

        /// <summary>
        /// Quick Sort
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <returns>Sorted Collection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Collection cannot be null.")]
        public static ICollection QuickSort(this ICollection collection)
        {
            Contract.Requires(null != collection);

            Contract.Ensures(Contract.Result<ICollection>() != null);
            Contract.Ensures(Contract.Result<ICollection>().Count == collection.Count);

            if (0 == collection.Count)
            {
                return collection;
            }

            IComparable[] array = new IComparable[collection.Count];
            collection.CopyTo(array, 0);

            IComparable temp;
            Stack stack = new Stack();
            IComparable pivot;
            int pivotIndex = 0;
            int leftIndex = pivotIndex + 1;
            int rightIndex = collection.Count - 1;

            stack.Push(pivotIndex);
            stack.Push(rightIndex);

            int leftIndexOfSubSet, rightIndexOfSubset;

            while (stack.Count > 0)
            {
                rightIndexOfSubset = (int)stack.Pop();
                leftIndexOfSubSet = (int)stack.Pop();

                leftIndex = leftIndexOfSubSet + 1;
                pivotIndex = leftIndexOfSubSet;
                rightIndex = rightIndexOfSubset;

                pivot = array[pivotIndex];

                if (leftIndex > rightIndex)
                {
                    continue;
                }

                while (leftIndex < rightIndex)
                {
                    while ((leftIndex <= rightIndex) && (0 >= array[leftIndex].CompareTo(pivot)))
                    {
                        leftIndex++;
                    }

                    while ((leftIndex <= rightIndex) && (0 <= array[rightIndex].CompareTo(pivot)))
                    {
                        rightIndex--;
                    }

                    if (rightIndex >= leftIndex)
                    {
                        temp = array[leftIndex];
                        array[leftIndex] = array[rightIndex];
                        array[rightIndex] = temp;
                    }
                }

                if (pivotIndex <= rightIndex)
                {
                    if (0 > array[rightIndex].CompareTo(array[pivotIndex]))
                    {
                        temp = array[pivotIndex];
                        array[pivotIndex] = array[rightIndex];
                        array[rightIndex] = temp;
                    }
                }

                if (leftIndexOfSubSet < rightIndex)
                {
                    stack.Push(leftIndexOfSubSet);
                    stack.Push(rightIndex - 1);
                }

                if (rightIndexOfSubset > rightIndex)
                {
                    stack.Push(rightIndex + 1);
                    stack.Push(rightIndexOfSubset);
                }
            }

            return array;
        }

        /// <summary>
        /// Selection Sort
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <returns>Sorted Collection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Collection cannot be null.")]
        public static ICollection SelectionSort(this ICollection collection)
        {
            Contract.Requires(null != collection);

            Contract.Ensures(Contract.Result<ICollection>() != null);
            Contract.Ensures(Contract.Result<ICollection>().Count == collection.Count);

            if (0 == collection.Count)
            {
                return collection;
            }

            IComparable[] array = new IComparable[collection.Count];
            collection.CopyTo(array, 0);

            IComparable temp = null;
            int smallestLocation = 0;

            for (int j = 0; j < array.Length - 1; j++)
            {
                smallestLocation = j;

                for (int i = j; i < array.Length; i++)
                {
                    if (0 == array[i].CompareTo(temp))
                    {
                        smallestLocation = i;
                        break;
                    }
                    else if (0 > array[i].CompareTo(array[smallestLocation]))
                    {
                        smallestLocation = i;
                    }
                }

                if (smallestLocation != j)
                {
                    temp = array[smallestLocation];
                    array[smallestLocation] = array[j];
                    array[j] = temp;
                }
            }

            return array;
        }

        /// <summary>
        /// Shell Sort
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <returns>Sorted Collection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Collection cannot be null.")]
        public static ICollection ShellSort(this ICollection collection)
        {
            Contract.Requires(null != collection);

            Contract.Ensures(Contract.Result<ICollection>() != null);
            Contract.Ensures(Contract.Result<ICollection>().Count == collection.Count);

            if (0 == collection.Count)
            {
                return collection;
            }

            IComparable[] array = new IComparable[collection.Count];
            collection.CopyTo(array, 0);

            IComparable temp = null;
            int i, j, increment = 3;
            while (increment > 0)
            {
                for (i = 0; i < array.Length; i++)
                {
                    j = i;
                    temp = array[i];
                    while ((j >= increment)
                        && (0 > temp.CompareTo(array[j - increment])))
                    {
                        array[j] = array[j - increment];
                        j = j - increment;
                    }

                    array[j] = temp;
                }

                if (increment / 2 != 0)
                {
                    increment /= 2;
                }
                else if (increment == 1)
                {
                    break;
                }
                else
                {
                    increment = 1;
                }
            }

            return array;
        }
        #endregion

        #region System.Guid
        /// <summary>
        /// To Base 64 encoded string
        /// </summary>
        /// <param name="value">Guid</param>
        /// <returns>Base64 encoded</returns>
        public static string ToBase64(this Guid value)
        {
            Contract.Requires((0 + 22) <= System.Convert.ToBase64String(value.ToByteArray()).Length);

            return System.Convert.ToBase64String(value.ToByteArray()).Substring(0, 22);
        }

        /// <summary>
        /// To ASCII 85 encoded string
        /// </summary>
        /// <param name="value">Guid</param>
        /// <returns>ASCII85</returns>
        public static string ToAscii85(this Guid value)
        {
            var a = new Ascii85();
            return a.Encode(value.ToByteArray());
        }
        #endregion

        #region System.Array
        /// <summary>
        /// Merges current Array with objects to Merge
        /// </summary>
        /// <typeparam name="T">Type of Array</typeparam>
        /// <param name="primary">Current Array</param>
        /// <param name="add">To Merge With</param>
        /// <returns>Merged Array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code Contracts"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public static T[] Merge<T>(this T[] primary, T[] add) where T : class
        {
            Contract.Requires(null != primary);
            Contract.Requires<ArgumentNullException>(null != add);

            T[] returnObjects = new T[primary.Length + add.Length];
            primary.CopyTo(returnObjects, 0);
            add.CopyTo(returnObjects, primary.Length);
            return returnObjects;
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <typeparam name="T">Type of Array</typeparam>
        /// <param name="superset">Current Array</param>
        /// <param name="subset">Items to Remove</param>
        /// <returns>Clean Array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code Contracts"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public static T[] Remove<T>(this T[] superset, T[] subset) where T : class
        {
            Contract.Requires(null != superset);
            Contract.Requires<ArgumentNullException>(null != subset);

            bool removeItem = false;
            List<T> returnObjects = new List<T>();
            foreach (T main in superset)
            {
                foreach (T item in subset)
                {
                    if (main == item)
                    {
                        removeItem = true;
                        break;
                    }
                }

                if (!removeItem)
                {
                    returnObjects.Add(main);
                }

                removeItem = false;
            }

            return returnObjects.ToArray();
        }

        /// <summary>
        /// Check if the current array contains a sub-array
        /// </summary>
        /// <typeparam name="T">Type of Array</typeparam>
        /// <param name="primary">Current Array</param>
        /// <param name="target">Target Array</param>
        /// <returns>True if it contains the target, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public static bool Contains<T>(this T[] primary, T[] target)
        {
            Contract.Requires(null != primary);

            if (null == target || target.Length > primary.Length)
            {
                return false;
            }

            for (int i = 0; i < primary.Length - target.Length; i++)
            {
                bool found = true;

                for (int j = 0; j < target.Length; j++)
                {
                    if (!primary[i + j].Equals(target[j]))
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Create a new sub-array from a range of elements from an array starting at a specified source index up to a specified length.
        /// </summary>
        /// <typeparam name="T">Type of Array</typeparam>
        /// <param name="primary">Current Array</param>
        /// <param name="index">Source Index</param>
        /// <param name="length">Length of Sub-Array</param>
        /// <returns>Sub-Array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public static T[] SubArray<T>(this T[] primary, int index, int length)
        {
            Contract.Requires(null != primary);
            Contract.Requires(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(index + length <= primary.GetLowerBound(0) + primary.Length);

            T[] subArray = new T[length];
            Array.Copy(primary, index, subArray, 0, length);
            return subArray;
        }

        /// <summary>
        /// Content Equals
        /// </summary>
        /// <typeparam name="T">Type, IComparable</typeparam>
        /// <param name="primary">Primary</param>
        /// <param name="target">Target</param>
        /// <returns>Equals</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code Contracts")]
        public static bool ContentEquals<T>(this T[] primary, T[] target) where T : IComparable
        {
            Contract.Requires(null != primary);

            if (null == target || primary.Length != target.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < primary.Length; i++)
                {
                    if (null == primary[i] && null == target[i])
                    {
                        continue;
                    }
                    else if (null != primary[i] && null != target[i])
                    {
                        if (!primary[i].Equals(target[i]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region System.Reflection.PropertyInfo
        /// <summary>
        /// Get Custom Attributes
        /// </summary>
        /// <typeparam name="T">Attribute Type</typeparam>
        /// <param name="memberInfo">Member Info</param>
        /// <param name="inherit">Inherit</param>
        /// <returns>Custom Attributes</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contract")]
        public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            Contract.Requires(null != memberInfo);

            return memberInfo.GetCustomAttributes(typeof(T), inherit) as T[];
        }

        /// <summary>
        /// Get Custom Attribute
        /// </summary>
        /// <typeparam name="T">Attribute Type</typeparam>
        /// <param name="memberInfo">Member Info</param>
        /// <param name="inherit">Inherit</param>
        /// <returns>Custom Attribute</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contract")]
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            Contract.Requires(null != memberInfo);

            var attributes = memberInfo.GetCustomAttributes(typeof(T), inherit) as T[];
            return (null != attributes && 0 < attributes.Length) ? attributes[0] : null;
        }
        #endregion

        #region System.Int64
        /// <summary>
        /// Remove Traling Zeros
        /// </summary>
        /// <param name="number">Number</param>
        /// <returns>Number Shortened</returns>
        public static long RemoveTrailingZeros(this long number)
        {
            while (number % 10 == 0)
            {
                number /= 10;
            }

            return number;
        }
        #endregion

        #region System.DateTime
        /// <summary>
        /// Date Time, Shorten By
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <param name="by">Shorten By</param>
        /// <returns>Shortened Date Time</returns>
        public static DateTime Shorten(this DateTime dateTime, long by)
        {
            Contract.Requires((dateTime.Ticks - (dateTime.Ticks % by)) <= 3155378975999999999);

            return new DateTime(dateTime.Ticks - (dateTime.Ticks % by), dateTime.Kind);
        }

        /// <summary>
        /// Add Business Days
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="days">Days</param>
        /// <returns>Date Time</returns>
        public static DateTime AddBusinessDays(this DateTime date, int days)
        {
            Contract.Requires<ArgumentException>(0 <= days);

            if (days > 0)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        date = date.AddDays(2);
                        days -= 1;
                        goto default;
                    case DayOfWeek.Sunday:
                        date = date.AddDays(1);
                        days -= 1;
                        goto default;
                    default:
                        date = date.AddDays((days / 5) * 7);
                        var extraDays = days % 5;
                        if ((int)date.DayOfWeek + extraDays > 5)
                        {
                            extraDays += 2;
                        }

                        date = date.AddDays(extraDays);
                        break;
                }
            }

            return date;
        }
        #endregion

        #region System.Byte
        /// <summary>
        /// Compute MD5 Hash
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>MD5 Hash</returns>
        public static byte[] MD5Hash(this byte[] value)
        {
            Contract.Requires<ArgumentNullException>(null != value);

            using (var hash = MD5.Create())
            {
                return hash.ComputeHash(value);
            }
        }

        /// <summary>
        /// Convert Byte[] to Hexadecimal String
        /// </summary>
        /// <param name="value">byte[] to Convert</param>
        /// <returns>Hexidecimal MD5</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts.")]
        public static string GetHexadecimal(this byte[] value)
        {
            Contract.Requires(null != value);

            var sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                sb.Append(value[i].ToString("x2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Deserialized Bytes
        /// </summary>
        /// <typeparam name="T">Serialized Object Type</typeparam>
        /// <param name="value">Bytes</param>
        /// <returns>object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring.")]
        public static T Deserialize<T>(this byte[] value)
        {
            Contract.Requires<ArgumentException>(0 < value.LongLength);

            T returnValue;
            using (var ms = new MemoryStream())
            {
                ms.Write(value, 0, value.Length);
                ms.Position = 0;
                var bf = new BinaryFormatter();
                returnValue = (T)bf.Deserialize(ms);
            }

            return returnValue;
        }
        #endregion

        #region System.Object
        /// <summary>
        /// Serializes an Object
        /// </summary>
        /// <param name="value">object to Serialize</param>
        /// <returns>bytes</returns>
        public static byte[] Serialize(this object value)
        {
            byte[] bytes;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, value);
                bytes = ms.ToArray();
            }

            return bytes;
        }
        #endregion
    }
}