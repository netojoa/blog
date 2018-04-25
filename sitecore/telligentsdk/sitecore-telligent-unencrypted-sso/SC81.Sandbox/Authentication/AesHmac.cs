using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SC81.Sandbox.Authentication
{
    public class AesHmac : IDisposable
    {
        private readonly byte[] _hmacKey;
        private readonly AesCryptoServiceProvider _aes;
        private readonly RandomNumberGenerator _rng = new RNGCryptoServiceProvider();

        public AesHmac(string encryptionKey, string hmacKey)
        {

            var encryption = Convert.FromBase64String(encryptionKey);
            var _hmac = Convert.FromBase64String(hmacKey);

            _aes = new AesCryptoServiceProvider { Mode = CipherMode.CBC, Key = encryption };

        }

        public string SecureCookieForSSO(string cookieKeyNameForUsername, string userName,
                                         string cookieKeyNameForEmail, string email,
                                         string cookieKeyNameForDisplayName, string displayName)
        {
            var ssoData = string.Format("{0}={1}&{2}={3}&{4}={5}", cookieKeyNameForUsername, userName,
                                                                   cookieKeyNameForEmail, email,
                                                                   cookieKeyNameForDisplayName, displayName);
            return ssoData;
        }

        private string SecureCookieForSso(string ssoData)
        {
            var iv = new byte[_aes.IV.Length];
            _rng.GetBytes(iv);

            var ciphertext = GenerateCipherText(ssoData, iv);
            var hmac = ComputeHmac(ciphertext, iv);

            return Convert.ToBase64String(iv)
                   + "$" + Convert.ToBase64String(hmac)
                   + "$" + Convert.ToBase64String(ciphertext);
        }

        private byte[] GenerateCipherText(string plainText, byte[] iv)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var memoryStream = new MemoryStream())
            {
                using (var encryptor = _aes.CreateEncryptor(_aes.Key, iv))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private byte[] ComputeHmac(byte[] ciphertext, byte[] individualValue)
        {
            var messageToMac = new byte[individualValue.Length + ciphertext.Length];
            Array.Copy(individualValue, 0, messageToMac, 0, individualValue.Length);
            Array.Copy(ciphertext, 0, messageToMac, individualValue.Length, ciphertext.Length);

            using (var hmacsha256 = new HMACSHA256(_hmacKey))
            {
                return hmacsha256.ComputeHash(messageToMac);
            }
        }

        public void Dispose()
        {
            _aes.Dispose();
        }

    }
}