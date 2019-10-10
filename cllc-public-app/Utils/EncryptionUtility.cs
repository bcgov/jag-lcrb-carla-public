using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Gov.Lclb.Cllb.Public.Utility
{
    /// <summary>
    /// Helper methods for working with encryption
    /// </summary>
    public static class EncryptionUtility
    {

        /// <summary>
        /// Encrypt a string using AES
        /// </summary>
        /// <param name="text">The string to encrypt</param>
        /// <param name="keyString">The secret key</param>
        /// <returns></returns>
        public static string EncryptString(string text, string keyString)
        {
            string result = null;

            using (Aes aes = Aes.Create())
            {
                byte[] key = Encoding.UTF8.GetBytes(keyString.Substring(0, aes.Key.Length));

                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aes.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        byte[] byteResult = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, byteResult, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, byteResult, iv.Length, decryptedContent.Length);

                        result = Convert.ToBase64String(byteResult);
                    }
                }
            }
            return result;
        }

        public static string DecryptString(string cipherText, string keyString)
        {
            string result = null;
            try
            {
                cipherText = HttpUtility.UrlDecode(cipherText);
                cipherText = cipherText.Replace(" ", "+");
                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

                using (var aesAlg = Aes.Create())
                {
                    var key = Encoding.UTF8.GetBytes(keyString.Substring(0, aesAlg.Key.Length));
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // invalid code. Return a null value 
                result = null;
            }
            return result;
        }
    }
}
