// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='StringHelper.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System;
    using System.Text;

    /// <summary>
    /// String Helper
    /// </summary>
    public static class StringHelper
    {
        #region Methods
        /// <summary>
        /// String Longer than the maximum row length
        /// </summary>
        /// <returns>Long String</returns>
        public static string LongerThanMaximumRowLength()
        {
            return FixedLength(DataSource.MaximumStringLength + 1);
        }

        /// <summary>
        /// Valid Random String
        /// </summary>
        /// <returns>String</returns>
        public static string ValidString()
        {
            return ValidString(256);
        }

        /// <summary>
        /// Valid Random String
        /// </summary>
        /// <param name="maxLength">Maximum length of String</param>
        /// <returns>String</returns>
        public static string ValidString(int maxLength)
        {
            var random = new Random();
            return FixedLength(random.Next(1, maxLength));
        }

        /// <summary>
        /// Null Or Empty Or White Space
        /// </summary>
        /// <returns>String</returns>
        public static string NullEmptyWhiteSpace()
        {
            var random = new Random();
            return InvalidString(random.Next(2));
        }

        /// <summary>
        /// Null Or Empty
        /// </summary>
        /// <returns>String</returns>
        public static string NullEmpty()
        {
            var random = new Random();
            return InvalidString(random.Next(1));
        }

        /// <summary>
        /// Fixed Length String
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Fixed Length</returns>
        public static string FixedLength(int length)
        {
            var contents = new StringBuilder(length);
            while (contents.Length < length)
            {
                contents.Append(Guid.NewGuid().ToString());
            }

            return contents.ToString().Substring(0, length);
        }

        /// <summary>
        /// Invalid String
        /// </summary>
        /// <param name="index">Return Item</param>
        /// <returns>String</returns>
        private static string InvalidString(int index)
        {
            switch (index)
            {
                case 0:
                    return null;
                case 1:
                    return string.Empty;
                case 2:
                    var random = new Random();
                    var max = random.Next(50);
                    var sb = new StringBuilder();
                    for (int i = 0; i < max; i++)
                    {
                        sb.Append(' ');
                    }

                    return sb.ToString();
                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion
    }
}