// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethodsTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Collections;
    using System.Security.Cryptography;
    using System.Text;
    using Abc.Azure;
    using Abc.Services;
    using Abc.Website;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Extension Methods Test
    /// </summary>
    [TestClass]
    public class ExtensionMethodsTest
    {
        #region Members
        /// <summary>
        /// Un-Ordered Data Set
        /// </summary>
        private readonly int[] unordered = new int[1000];

        /// <summary>
        /// Ordered Data Set
        /// </summary>
        private readonly int[] ordered = new int[1000];
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize Test
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            Random random = new Random();
            for (int index = 0; index < unordered.Length; index++)
            {
                unordered[index] = random.Next();
            }

            Array.Copy(unordered, ordered, unordered.Length);

            Array.Sort(ordered);
        }
        #endregion

        #region System.String
        [TestMethod]
        public void FormatWithCulture()
        {
            string format = "{0}:{1:ddMM}:{2}";
            Guid g = Guid.NewGuid();
            string testCultureFormat = format.FormatWithCulture(.99, DateTime.Now, g);
            string testFormat = string.Format(format, .99, DateTime.Now, g);

            Assert.AreEqual<string>(testFormat, testCultureFormat, "Strings should be the same.");
        }

        [TestMethod]
        public void StartsWithWithCulture()
        {
            string startsWith = "abcXXXX";
            string with = "abc";
            string withUpper = "ABC";

            Assert.IsTrue(startsWith.StartsWith(with), "Doesn't start with.");
            Assert.IsFalse(startsWith.StartsWith(withUpper), "Doesn't start with.");
            Assert.IsTrue(startsWith.StartsWithWithCulture(with), "With Culture");
            Assert.IsFalse(startsWith.StartsWithWithCulture(withUpper), "With Culture");
        }

        [TestMethod]
        public void ToGuid()
        {
            Guid original = Guid.NewGuid();
            Guid converted = original.ToString().ToGuid();
            Assert.AreEqual<Guid>(original, converted);
        }

        [TestMethod]
        public void ToGuidFormatException()
        {
            Assert.AreEqual<Guid>(Guid.Empty, "blah".ToGuid());
        }

        [TestMethod]
        public void ToBytes()
        {
            var data = StringHelper.ValidString();
            var bytes = data.ToBytes();
            var expected = new UTF8Encoding().GetBytes(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                Assert.AreEqual<byte>(expected[i], bytes[i]);
            }
        }

        [TestMethod]
        public void GetHexMD5()
        {
            var data = StringHelper.ValidString();
            var bytes = data.ToBytes();
            var md5 = bytes.MD5Hash();
            Assert.AreEqual<string>(md5.GetHexadecimal(), data.GetHexMD5());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromHexTooShort()
        {
            "w".FromHex();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromHexOdd()
        {
            "asd".FromHex();
        }

        [TestMethod]
        public void FromHex()
        {
            var company = new Abc.Services.Contracts.Company()
            {
                Active = true,
                CreatedBy = new Abc.Services.Contracts.User()
                {
                    UserName = StringHelper.ValidString(),
                    Identifier = Guid.NewGuid(),
                },
                CreatedOn = DateTime.UtcNow,
                Deleted = false,
                EditedOn = DateTime.UtcNow,
                Identifier = Guid.NewGuid(),
                Name = StringHelper.ValidString(),
            };

            var hex = company.Serialize().GetHexadecimal();
            Assert.IsNotNull(hex);
            var deserialized = hex.FromHex().Deserialize<Abc.Services.Contracts.Company>();
            Assert.AreEqual<string>(company.CreatedBy.UserName, deserialized.CreatedBy.UserName);
            Assert.AreEqual<string>(company.Name, deserialized.Name);
            Assert.AreEqual<bool>(company.Active, deserialized.Active);
            Assert.AreEqual<bool>(company.Deleted, deserialized.Deleted);
            Assert.AreEqual<DateTime>(company.CreatedOn, deserialized.CreatedOn);
            Assert.AreEqual<DateTime>(company.EditedOn, deserialized.EditedOn);
            Assert.AreEqual<Guid>(company.Identifier, deserialized.Identifier);
            Assert.AreEqual<Guid>(company.CreatedBy.Identifier, deserialized.CreatedBy.Identifier);
        }
        #endregion

        #region System.ValueType
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SizeOfInvalid()
        {
            var valType = new ArgIterator();

            valType.SizeOf();
        }

        [TestMethod]
        public void SizeOf()
        {
            Assert.AreEqual<int>(sizeof(int), 1.SizeOf(), "int");
            Assert.AreEqual<int>(sizeof(char), 'a'.SizeOf(), "char");
            Assert.AreEqual<int>(sizeof(bool), true.SizeOf(), "bool");
            Assert.AreEqual<int>(8, DateTime.Now.SizeOf(), "DateTime");
            Assert.AreEqual<int>(8, TimeSpan.Zero.SizeOf(), "TimeSpan");
            Assert.AreEqual<int>(16, Guid.Empty.SizeOf(), "Guid");
            long l = 12312;
            Assert.AreEqual<int>(sizeof(long), l.SizeOf(), "long");
            double d = 123421.123123;
            Assert.AreEqual<int>(sizeof(double), d.SizeOf(), "double");
            Assert.AreEqual<int>(sizeof(decimal), new decimal(123123.2312).SizeOf(), "decimal");
            float f = 123.123F;
            Assert.AreEqual<int>(sizeof(float), f.SizeOf(), "float");
            uint ui = 22;
            Assert.AreEqual<int>(sizeof(uint), ui.SizeOf(), "uint");
            ulong ul = 88;
            Assert.AreEqual<int>(sizeof(ulong), ul.SizeOf(), "ulong");
            short s = -1048;
            Assert.AreEqual<int>(sizeof(short), s.SizeOf(), "short");
            ushort us = 1048;
            Assert.AreEqual<int>(sizeof(ushort), us.SizeOf(), "ushort");
            byte b = 255;
            Assert.AreEqual<int>(sizeof(byte), b.SizeOf(), "byte");
            sbyte sb = -23;
            Assert.AreEqual<int>(sizeof(sbyte), sb.SizeOf(), "sbyte");
        }
        #endregion

        #region System.IEnumerable
        [TestMethod]
        public void BubbleSort()
        {
            ICollection sortedCol = unordered.BubbleSort();
            int[] sorted = new int[sortedCol.Count];
            sortedCol.CopyTo(sorted, 0);
            for (int index = 0; index < sorted.Length; index++)
            {
                if (ordered[index] != sorted[index])
                {
                    Assert.Fail("Sort order is not consistant.");
                }
            }
        }

        [TestMethod]
        public void QuickSort()
        {
            ICollection sortedCol = unordered.QuickSort();
            int[] sorted = new int[sortedCol.Count];
            sortedCol.CopyTo(sorted, 0);
            for (int index = 0; index < sorted.Length; index++)
            {
                if (ordered[index] != sorted[index])
                {
                    Assert.Fail("Sort order is not consistant.");
                }
            }
        }

        [TestMethod]
        public void SelectionSort()
        {
            ICollection sortedCol = unordered.SelectionSort();
            int[] sorted = new int[sortedCol.Count];
            sortedCol.CopyTo(sorted, 0);
            for (int index = 0; index < sorted.Length; index++)
            {
                if (ordered[index] != sorted[index])
                {
                    Assert.Fail("Sort order is not consistant.");
                }
            }
        }

        [TestMethod]
        public void ShellSort()
        {
            ICollection sortedCol = unordered.ShellSort();
            int[] sorted = new int[sortedCol.Count];
            sortedCol.CopyTo(sorted, 0);
            for (int index = 0; index < sorted.Length; index++)
            {
                if (ordered[index] != sorted[index])
                {
                    Assert.Fail("Sort order is not consistant.");
                }
            }
        }
        #endregion

        #region System.Guid
        /// <summary>
        /// To Base 64
        /// </summary>
        public void ToBase64()
        {
            Guid original = Guid.NewGuid();
            Guid converted = original.ToBase64().ToGuidBase64();
            Assert.AreEqual<Guid>(original, converted);
        }

        /// <summary>
        /// To Ascii 85
        /// </summary>
        public void ToAscii85()
        {
            Guid original = Guid.NewGuid();
            Guid converted = original.ToAscii85().ToGuidAscii85();
            Assert.AreEqual<Guid>(original, converted);
        }
        #endregion

        #region OccurrenceData
        /// <summary>
        /// A test for ExecutionTime
        /// </summary>
        [TestMethod]
        public void ExecutionTimeTest()
        {
            var ramdom = new Random();
            var number = ramdom.Next();
            var data = new OccurrenceData()
            {
                Duration = number
            };
            var expected = new TimeSpan(number);
            var actual = data.ExecutionTime();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region System.Array
        [TestMethod]
        public void ArrayMerge()
        {
            string[] one = { "1", "2", "3", "4", "5", "6" };
            string[] two = { "7", "8", "9", "10" };

            string[] three = one.Merge(two);

            if (three.Length != one.Length + two.Length)
            {
                Assert.Fail("Array Lengths don't match up");
            }

            int i = 0;
            foreach (string num in three)
            {
                if (i < one.Length)
                {
                    if (one[i] != num)
                    {
                        Assert.Fail("Initial array is not aligned with output array");
                    }
                }
                else
                {
                    if (two[i - one.Length] != num)
                    {
                        Assert.Fail("Secondary array was not properly merged with initial array");
                    }
                }

                i++;
            }
        }

        [TestMethod]
        public void ArrayRemove()
        {
            string[] full = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            string[] remove = { "8", "9", "4" };

            string[] output = full.Remove(remove);

            if (output.Length != full.Length - remove.Length)
            {
                Assert.Fail("Array Lengths don't match up");
            }

            foreach (string check in remove)
            {
                foreach (string item in output)
                {
                    if (check == item)
                    {
                        Assert.Fail("Item should have been removed.");
                    }
                }
            }
        }

        [TestMethod]
        public void ArrayContains()
        {
            string[] full = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            string[] target1 = { "3", "4", "5" };
            string[] target2 = { "11", "12" };
            string[] target3 = { "5", "4", "3" };

            Assert.IsTrue(full.Contains(target1));
            Assert.IsFalse(full.Contains(target2));
            Assert.IsFalse(full.Contains(target3));
        }

        [TestMethod]
        public void ArrayEquals()
        {
            string[] array1 = { "1", "2", "3", "4" };
            int[] array2 = { 1, 2, 3, 4 };

            Assert.IsTrue(array1.ContentEquals(new string[] { "1", "2", "3", "4" }));
            Assert.IsFalse(array1.ContentEquals(new string[] { "5", "6", "7", "8" }));
            Assert.IsTrue(array2.ContentEquals(new int[] { 1, 2, 3, 4 }));
            Assert.IsFalse(array2.ContentEquals(new int[] { 1, 2, 3 }));

            // check arrays with nulls
            string[] array3 = { "1", "2", null };
            Assert.IsTrue(array3.ContentEquals(new string[] { "1", "2", null }));
            Assert.IsFalse(array3.ContentEquals(new string[] { "1", null, null }));
        }

        [TestMethod]
        public void ArraySub()
        {
            int[] full = { 1, 2, 3, 4, 5, 6 };
            int[] target1 = full.SubArray<int>(2, 3);
            int[] target2 = full.SubArray<int>(4, 1);

            int[] expected1 = { 3, 4, 5 };
            int[] expected2 = { 5 };

            Assert.IsTrue(target1.ContentEquals(expected1));
            Assert.IsTrue(target2.ContentEquals(expected2));
        }
        #endregion

        #region System.Reflection.PropertyInfo
        [TestMethod]
        public void GetCustomAttributesNone()
        {
            var clsCompliant = typeof(object).GetCustomAttributes<CLSCompliantAttribute>(false);
            Assert.IsNotNull(clsCompliant);
            Assert.AreEqual<int>(0, clsCompliant.Length);
        }

        [TestMethod]
        public void GetCustomAttributeNone()
        {
            var clsCompliant = typeof(object).GetCustomAttribute<CLSCompliantAttribute>(false);
            Assert.IsNull(clsCompliant);
        }

        [TestMethod]
        public void GetCustomAttributes()
        {
            var dataStore = typeof(ErrorData).GetCustomAttributes<AzureDataStoreAttribute>(false);
            Assert.IsNotNull(dataStore);
        }

        [TestMethod]
        public void GetCustomAttribute()
        {
            var dataStore = typeof(ErrorData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.IsNotNull(dataStore);
        }
        #endregion

        #region System.Int64
        [TestMethod]
        public void RemoveTrailingZeros()
        {
            long number = 999000;
            Assert.AreEqual<long>(999, number.RemoveTrailingZeros());
        }
        #endregion

        #region System.DateTime
        [TestMethod]
        public void Shorten()
        {
            var now = DateTime.UtcNow;
            Assert.AreEqual<DateTime>(new DateTime(now.Ticks - (now.Ticks % 1000), now.Kind), now.Shorten(1000));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddBusinessDaysNegative()
        {
            var random = new Random();
            var now = DateTime.UtcNow;
            now.AddBusinessDays(random.Next() * -1);
        }

        [TestMethod]
        public void AddBusinessDays()
        {
            var now = DateTime.UtcNow;
            while (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
            {
                now = now.AddDays(1);
            }

            var data = now.AddBusinessDays(5);
            Assert.AreEqual<DateTime>(now.AddDays(7).Date, data.Date);
        }

        [TestMethod]
        public void AddBusinessDaysSaturday()
        {
            var now = DateTime.UtcNow;
            while (now.DayOfWeek != DayOfWeek.Saturday)
            {
                now = now.AddDays(1);
            }

            var data = now.AddBusinessDays(5);
            Assert.AreEqual<DateTime>(now.AddDays(6).Date, data.Date);
        }

        [TestMethod]
        public void AddBusinessDaysSunday()
        {
            var now = DateTime.UtcNow;
            while (now.DayOfWeek != DayOfWeek.Sunday)
            {
                now = now.AddDays(1);
            }

            var data = now.AddBusinessDays(5);
            Assert.AreEqual<DateTime>(now.AddDays(5).Date, data.Date);
        }
        #endregion

        #region System.Byte
        [TestMethod]
        public void MD5Hash()
        {
            var random = new Random();
            byte[] bytes = new byte[1024];
            random.NextBytes(bytes);
            var extensionMethod = bytes.MD5Hash();

            using (var hash = MD5.Create())
            {
               var md5 = hash.ComputeHash(bytes);
               Assert.IsTrue(extensionMethod.ContentEquals<byte>(md5));
            }
        }

        [TestMethod]
        public void GetHexidecimal()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);
            
            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }

            Assert.AreEqual<string>(sb.ToString(), bytes.GetHexadecimal());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeserializeEmpty()
        {
            var bytes = new byte[0];
            bytes.Deserialize<object>();
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            var data = new Abc.Services.Contracts.Contact()
            {
                Email = StringHelper.ValidString(),
                Identifier = Guid.NewGuid(),
                Owner = new Abc.Services.Contracts.User()
                {
                    Email = StringHelper.ValidString(),
                    Identifier = Guid.NewGuid(),
                    NameIdentifier = StringHelper.ValidString(),
                    UserName = StringHelper.ValidString(),
                },
            };

            var serialized = data.Serialize();

            Assert.IsNotNull(serialized);
            var deserialized = serialized.Deserialize<Abc.Services.Contracts.Contact>();
            Assert.AreEqual<string>(data.Email, deserialized.Email);
            Assert.AreEqual<string>(data.Owner.Email, deserialized.Owner.Email);
            Assert.AreEqual<string>(data.Owner.NameIdentifier, deserialized.Owner.NameIdentifier);
            Assert.AreEqual<string>(data.Owner.UserName, deserialized.Owner.UserName);
            Assert.AreEqual<Guid>(data.Identifier, deserialized.Identifier);
            Assert.AreEqual<Guid>(data.Owner.Identifier, deserialized.Owner.Identifier);
        }
        #endregion

        #region System.Object
        [TestMethod]
        public void Serialize()
        {
            var data = DateTime.UtcNow;
            Assert.IsNotNull(data.Serialize());
        }
        #endregion
    }
}