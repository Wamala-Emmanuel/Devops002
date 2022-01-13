using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Serilog;

namespace GatewayService.Helpers.Nira
{
#nullable disable
    public class NiraUtils
    {
        public static string specialCharacters = "!@$?_-";

        /// <summary>
        /// converts a string into a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] GetByteArrayFromString(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        private static byte[] CalculateNonce()
        {
            var byteArray = new byte[16];

            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(byteArray);

            return byteArray;
        }

        /// <summary>
        /// Base64 encode and then return a string
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string ConvertToBase64(byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }
        
        /// <summary>
        /// Returns a date in the format "yyyy-MM-dd'T'HH:mm:ss.fffzzz"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static string GetDateString(DateTime date)
        {
            return date.ToString($"yyyy-MM-dd'T'HH:mm:ss.fffzzz");
        }

        /// <summary>
        /// sets the nira usernametoken values
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="offset"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static UsernameToken SetUsernameToken(string username, string password, double offset, string culture)
        {
            Guard.ThrowIfNullOrEmpty(username, "Please enter the username");
            Guard.ThrowIfNullOrEmpty(password, "Please enter the password");

            var nonceBytes = CalculateNonce();
            var nonce = nonceBytes.ToBase64();

            var dateNow = DateTime.UtcNow.AddHours(offset);
            var dateCreated = dateNow.ToString($"yyyy-MM-dd'T'HH:mm:ss.fff", new CultureInfo(culture)) + "+03:00";
            var createdDigest = dateCreated.RemoveLastOccurrenceOf(":");
            var createdDigestBytes = Encoding.UTF8.GetBytes(createdDigest); 

            var passwordHashBytes = password.EncryptWithSha1();

            var combined = CombineBytes(nonceBytes, createdDigestBytes, passwordHashBytes);

            var encodedPasswordDigest = combined.EncryptWithSha1();
            var passwordDigest = encodedPasswordDigest.ToBase64();
            
            var token = new UsernameToken
            {
                Username = username, 
                Created = dateCreated, 
                Nonce = nonce, 
                Password = passwordDigest
            };

            return token;
        }

        /// <summary>
        ///  Combines nonce bytes, created bytes and Sha1 hashed passowrd bytes into a byte array
        /// </summary>
        /// <param name="nonceBytes"></param>
        /// <param name="createdDigestBytes"></param>
        /// <param name="passwordHashBytes"></param>
        /// <returns></returns>
        private static byte[] CombineBytes( byte[] nonceBytes,byte[] createdDigestBytes, byte[] passwordHashBytes)
        {
            var combined = new byte[createdDigestBytes.Length + nonceBytes.Length + passwordHashBytes.Length];

            Buffer.BlockCopy(nonceBytes, 0, combined, 0, nonceBytes.Length);
            Buffer.BlockCopy(createdDigestBytes, 0, combined, nonceBytes.Length, createdDigestBytes.Length);
            Buffer.BlockCopy(passwordHashBytes, 0, combined, nonceBytes.Length + createdDigestBytes.Length,
                passwordHashBytes.Length);
            return combined;
        }

        /// <summary>
        /// Gets a X509Certificate2 object of the certificate
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetEncryptionCertificate(string certificatePath)
        {
            Guard.ThrowIfNullOrEmpty(certificatePath, "Please enter the certificate path");

            try
            {
                return new X509Certificate2(certificatePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex,$"{certificatePath} not found");
                throw new ClientFriendlyException($"{certificatePath} not found");
            }
        }

        /// <summary>
        /// Encrypts data with an RSA public key 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncryptWithRSA(X509Certificate2 certificate, string data)
        {
            Guard.ThrowIfNullOrEmpty(data, "Please enter the data to be encrypted");
            
            using var publicKey = certificate.GetRSAPublicKey();
            var dataToEncrypt = GetByteArrayFromString(data);
            return publicKey.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>
        /// Get Gender value from nin
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        public static string GetGenderFromNin(string nin)
        {
            Guard.ThrowIfNullOrEmpty(nin, "Please enter the nin.");

            return nin.Substring(1, 1);
        }
        
        public static string MaskCardDetails(string data, int charsToShow)
        {
            Guard.ThrowIfNullOrEmpty(data, "Please enter a valid string.");

            return data.MaskLastChars(charsToShow: charsToShow);
        }

        /// <summary>
        /// Nira username token details
        /// </summary>
        public class UsernameToken
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Nonce { get; set; }
            public string Created { get; set; }
        }

        /// <summary>
        /// Returns a password based on the NIRA password policy
        /// </summary>
        /// <returns></returns>
        public static string GetNiraPassword()
        {
            var lowerCase = StringExtensions.GetRandomString(3, true);
            var upperCase = StringExtensions.GetRandomString(3, false);
            var specialChar = StringExtensions.GetRandomSpecialCharacters(1);
            var digit = GetRandomNumber(0, 9);

            var newPassword = new StringBuilder
            {
                Capacity = 6
            };

            newPassword.Insert(0, lowerCase);
            newPassword.Insert(3, specialChar);
            newPassword.Insert(4, upperCase);
            newPassword.Insert(7, digit);

            return newPassword.ToString().Randomize();
        }

        public static int GetRandomNumber(int min, int max)
        {
            Guard.ThrowIfNullOrEmpty(min, "Please insert a valid minimum integer");
            Guard.ThrowIfNullOrEmpty(max, "Please insert a valid maximum integer");

            var random = new Random();

            return random.Next(min, max);
        }
    }
}
