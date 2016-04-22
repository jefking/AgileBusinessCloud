// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Convert.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Convert
    /// </summary>
    public static class Convert
    {
        #region Methods
        /// <summary>
        /// From String
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">Data</param>
        /// <returns>Converted from string</returns>
        public static T FromString<T>(string data)
        {
            return Convert.FromString<T>(data, default(T));
        }

        /// <summary>
        /// From String
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Converted from string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Hard to flatten into a simpler way.")]
        public static T FromString<T>(string data, T defaultValue)
        {
            object temp = null;
            var type = typeof(T);
            if (type == typeof(string))
            {
                temp = data;
            }
            else if (type == typeof(bool))
            {
                bool v;
                temp = bool.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(int))
            {
                int v;
                temp = int.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(Guid))
            {
                Guid guid;
                temp = Guid.TryParse(data, out guid) ? (object)guid : defaultValue;
            }
            else if (type == typeof(DateTime))
            {
                DateTime v;
                temp = DateTime.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(long))
            {
                long v;
                temp = long.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(decimal))
            {
                decimal v;
                temp = decimal.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(double))
            {
                double v;
                temp = double.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(byte))
            {
                byte v;
                temp = byte.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(char))
            {
                char v;
                temp = char.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(short))
            {
                short v;
                temp = short.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(uint))
            {
                uint v;
                temp = uint.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(ushort))
            {
                ushort v;
                temp = ushort.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type == typeof(float))
            {
                float v;
                temp = float.TryParse(data, out v) ? (object)v : defaultValue;
            }
            else if (type.IsEnum)
            {
                try
                {
                    temp = (T)Enum.Parse(type, data);
                }
                catch
                {
                    temp = defaultValue;
                }
            }
            else
            {
                return (T)System.Convert.ChangeType(data, typeof(T), CultureInfo.InvariantCulture);
            }

            return (T)temp;
        }
        #endregion
    }
}