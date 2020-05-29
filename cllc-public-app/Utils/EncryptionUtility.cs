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
        const int AES_256_KEY_SIZE_BYTES = 32;
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
                aes.Padding = PaddingMode.PKCS7;

                // AES-256 uses the same block size as AES-128 (the default) however the key size is larger (32 bytes).

                byte[] key = Encoding.UTF8.GetBytes(keyString.Substring(0, AES_256_KEY_SIZE_BYTES));

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

        public static string EncryptStringHex(string text, string keyString)
        {
            string result = null;

            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                byte[] key = Encoding.UTF8.GetBytes(keyString.Substring(0, AES_256_KEY_SIZE_BYTES));

                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);                                
                            }                            
                        }
                        

                        var iv = aes.IV;

                        byte[] data = msEncrypt.ToArray();

                        byte[] byteResult = new byte[iv.Length + data.Length];

                        Buffer.BlockCopy(iv, 0, byteResult, 0, iv.Length);
                        Buffer.BlockCopy(data, 0, byteResult, iv.Length, data.Length);

                        result = BitConverter.ToString(byteResult).Replace("-", ""); 
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
                    aesAlg.Padding = PaddingMode.PKCS7;
                    var key = Encoding.UTF8.GetBytes(keyString.Substring(0, AES_256_KEY_SIZE_BYTES));
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


        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string DecryptStringHex(string cipherText, string keyString)
        {
            
            string result = null;
            try
            {
                cipherText = HttpUtility.UrlDecode(cipherText);
                int NumberChars = cipherText.Length;
                byte[] fullCipher = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    fullCipher[i / 2] = Convert.ToByte(cipherText.Substring(i, 2), 16);
                
                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Padding = PaddingMode.PKCS7;
                    var key = Encoding.UTF8.GetBytes(keyString.Substring(0, AES_256_KEY_SIZE_BYTES));
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
