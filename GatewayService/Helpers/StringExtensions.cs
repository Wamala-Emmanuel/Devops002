using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GatewayService.Helpers
{
    public static class StringExtensions
    {
        public static string randomSequence = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string RemoveLastOccurrenceOf(this string source, string find)
        {
            var index = source.LastIndexOf(find, StringComparison.InvariantCulture);
            return source.Remove(index, find.Length).Insert(index, "");
        }

        public static byte[] EncryptWithSha1(this byte[] data)
        {
            byte[] encryptedData;
            using (var sha = new SHA1CryptoServiceProvider())
            {
                encryptedData = sha.ComputeHash(data);
            }
            return encryptedData;
        }


        public static byte[] EncryptWithSha1(this string data)
        {
            byte[] encryptedData;
            using (var sha = new SHA1CryptoServiceProvider())
            {
                var dataToEncrypt = Encoding.UTF8.GetBytes(data);
                encryptedData = sha.ComputeHash(dataToEncrypt);
            }
            return encryptedData;
        }

        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToUtf8(this string str)
        {
            var encodedDataAsBytes = Convert.FromBase64String(str);
            var returnValue = Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }


        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var converted = str.Split(" ").Select(ToPascal);
            return string.Join(' ', converted);
        }

        private static string ToPascal(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var a = str.ToCharArray();
            a[0] = char.ToLower(a[0]);

            return new string(a);
        }

        public static string ToLowerCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var a = str.ToCharArray();
            a[0] = char.ToLower(a[0]);

            return new string(a);
        }
        
        /// <summary>
        /// Returns a copy of the string with the first letter capitalized.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>A string with the first letter capitalized.</returns>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }

        /// <summary>
        /// Returns a string with randomized characaters based on the sequence  
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string GetRandomString(string sequence, int max)
        {
            Guard.ThrowIfNullOrEmpty(sequence, "Please insert a valid string");
            
            var sb = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < max; i++)
            {
                sb.Append(sequence[(random.Next(max))]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate the random string with a given size and case. 
        /// If the second parameter is true, the return string is lowercase
        /// </summary>
        /// <param name="size"></param>
        /// <param name="isLowerCase"></param>
        /// <returns></returns>
        public static string GetRandomString(int size, bool isLowerCase)
        {
            var sb = new StringBuilder();
            var random = new Random();
            char ch;
            
            foreach (var item in Enumerable.Range(0, size))
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                sb.Append(ch);
            }
            
            if (isLowerCase)
            {
                return sb.ToString().ToLower();
            }

            return sb.ToString();
        }

        public static string GetRandomSpecialCharacters(int length = 1)
        {
            var specialChars = "!@#$%^&*?_";
            Random random = new Random();
 
            char[] chars = new char[length];

            foreach (var item in Enumerable.Range(0, length))
            {
                chars[item] = specialChars[random.Next(0, specialChars.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Gets string bytes encoded in ASCII
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetAsciiBytes(string content)
        {
            return Encoding.ASCII.GetBytes(content);
        }

        /// <summary>
        /// converts a string into a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GetByteArrayFromString(this string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// Masks the initial characters in a string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="charsToShow"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string MaskInitialChars(this string str, int charsToShow = 4, char mask = '*')
        {
            Guard.ThrowIfNullOrEmpty(str, "Please insert a valid string");

            Guard.ThrowIfSubstringLengthIsGreaterThanStringLength(str, subStringLength: charsToShow, 
                "The characters to show are greater than string length");

            return str.Substring(str.Length - charsToShow).PadLeft(str.Length, mask);
        }

        /// <summary>
        /// Masks the last characters in a string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="charsToShow"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string MaskLastChars(this string str, int charsToShow = 4, char mask = '*')
        {
            Guard.ThrowIfNullOrEmpty(str, "Please a valid string");

            Guard.ThrowIfSubstringLengthIsGreaterThanStringLength(str, subStringLength: charsToShow,
                "The characters to show are greater than string length");

            return str.Substring(0, charsToShow).PadRight(str.Length, mask);
        }

        public static string Randomize(this string str)
        {
            var random = new Random();
            return new string(str.ToCharArray().OrderBy(s => (random.Next(2)%2) == 0).ToArray());
        }
    }
}