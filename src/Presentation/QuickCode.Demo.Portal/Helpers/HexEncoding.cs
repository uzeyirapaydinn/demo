using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QuickCode.Demo.Portal.Helpers
{
    /// <summary>
    /// HexEncoding ile ilgili işlemleri içerir
    /// </summary>
    public class HexEncoding
    {
        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            StringBuilder newString = new StringBuilder();
            char c;

            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                {
                    newString.Append(c);
                }
                else
                {
                    discarded++;
                }
            }

            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.SetString(newString.ToString().Substring(0, newString.Length - 1));
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new string(new char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }

            return bytes;
        }

        /// <summary>
        /// Converts byte data to String
        /// </summary>
        /// <param name="bytes">Byte data toString</param>
        /// <returns>String data</returns>
        public static string ToString(byte[] bytes)
        {
            string hexString = String.Empty;

            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }

            return hexString;
        }

        /// <summary>
        /// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>true if hex digit, false if not</returns>
        public static bool IsHexDigit(char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = char.ToUpper(c);
            numChar = Convert.ToInt32(c);

            if (numChar >= numA && numChar < (numA + 6))
            {
                return true;
            }

            if (numChar >= num1 && numChar < (num1 + 10))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>New byte number</returns>
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
            {
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            }

            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            return newByte;
        }
    }
}