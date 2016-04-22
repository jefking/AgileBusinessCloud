// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Ascii85.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Ascii85 encoding, decoding class
    /// </summary>
    public class Ascii85
    {
        #region Members
        /// <summary>
        /// Offset
        /// </summary>
        private const int Offset = 33;

        /// <summary>
        /// Powers of 85
        /// </summary>
        private static readonly uint[] powers = { 85 * 85 * 85 * 85, 85 * 85 * 85, 85 * 85, 85, 1 };

        /// <summary>
        /// Encoded Block
        /// </summary>
        private byte[] encodedBlock = new byte[5];

        /// <summary>
        /// Decoded block
        /// </summary>
        private byte[] decodedBlock = new byte[4];

        /// <summary>
        /// Row
        /// </summary>
        private uint row = 0;
        #endregion

        #region Methods
        /// <summary>
        /// Decodes an ASCII85 encoded string into the original binary data
        /// </summary>
        /// <param name="encoded">ASCII85 encoded string</param>
        /// <returns>byte array of decoded binary data</returns>
        public byte[] Decode(string encoded)
        {
            if (string.IsNullOrWhiteSpace(encoded))
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                var count = 0;

                foreach (char c in encoded)
                {
                    switch (c)
                    {
                        case 'z':
                            if (count != 0)
                            {
                                throw new FormatException("The character 'z' is invalid inside an ASCII85 block.");
                            }

                            this.decodedBlock[0] = 0;
                            this.decodedBlock[1] = 0;
                            this.decodedBlock[2] = 0;
                            this.decodedBlock[3] = 0;
                            stream.Write(this.decodedBlock, 0, this.decodedBlock.Length);
                            continue;
                        case '\n':
                        case '\r':
                        case '\t':
                        case '\0':
                        case '\f':
                        case '\b':
                            continue;
                        default:
                            if (c < '!' || c > 'u')
                            {
                                throw new FormatException("Bad character '{0}' found. ASCII85 only allows characters '!' to 'u'.".FormatWithCulture(c));
                            }

                            this.row += (uint)(c - Offset) * powers[count];
                            count++;
                            if (count == this.encodedBlock.Length)
                            {
                                this.DecodeBlock(this.decodedBlock.Length);
                                stream.Write(this.decodedBlock, 0, this.decodedBlock.Length);
                                this.row = 0;
                                count = 0;
                            }

                            break;
                    }
                }

                if (count != 0)
                {
                    if (count == 1)
                    {
                        throw new InvalidDataException("The last block of ASCII85 data cannot be a single byte.");
                    }

                    count--;
                    this.row += powers[count];
                    this.DecodeBlock(count);
                    for (int i = 0; i < count; i++)
                    {
                        stream.WriteByte(this.decodedBlock[i]);
                    }
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Encodes binary data into a plaintext ASCII85 format string
        /// </summary>
        /// <param name="binary">binary data to encode</param>
        /// <returns>ASCII85 encoded string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated in Code Contract.")]
        public string Encode(byte[] binary)
        {
            Contract.Requires(null != binary);

            var encoded = new StringBuilder((int)(binary.Length * (this.encodedBlock.Length / this.decodedBlock.Length)));

            var count = 0;
            this.row = 0;
            foreach (byte b in binary)
            {
                if (count >= this.decodedBlock.Length - 1)
                {
                    this.row |= b;
                    if (this.row == 0)
                    {
                        encoded.Append('z');
                    }
                    else
                    {
                        this.EncodeBlock(this.encodedBlock.Length, encoded);
                    }

                    this.row = 0;
                    count = 0;
                }
                else
                {
                    this.row |= (uint)(b << (24 - (count * 8)));
                    count++;
                }
            }

            if (count > 0)
            {
                this.EncodeBlock(count + 1, encoded);
            }

            return encoded.ToString();
        }

        /// <summary>
        /// Encode Text Block
        /// </summary>
        /// <param name="count">Count</param>
        /// <param name="encoded">String Builder</param>
        private void EncodeBlock(int count, StringBuilder encoded)
        {
            for (var i = this.encodedBlock.Length - 1; i >= 0; i--)
            {
                this.encodedBlock[i] = (byte)((this.row % 85) + Offset);
                this.row /= 85;
            }

            for (var i = 0; i < count; i++)
            {
                encoded.Append((char)this.encodedBlock[i]);
            }
        }

        /// <summary>
        /// Decode Block
        /// </summary>
        /// <param name="bytes">Bytes</param>
        private void DecodeBlock(int bytes)
        {
            for (var i = 0; i < bytes; i++)
            {
                this.decodedBlock[i] = (byte)(this.row >> (24 - (i * 8)));
            }
        }

        /// <summary>
        /// Invariant Method
        /// </summary>
        [ContractInvariantMethod]
        private void InvariantMethod()
        {
            Contract.Invariant(5 == powers.Length);
            Contract.Invariant(0 <= this.row);
            Contract.Invariant(null != this.encodedBlock);
            Contract.Invariant(null != this.decodedBlock);
        }
        #endregion
    }
}