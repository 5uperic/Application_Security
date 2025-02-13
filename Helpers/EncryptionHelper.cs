using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourSecretKey123"); // 16, 24, or 32 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("YourIV12345678901"); // 16 bytes


        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.GenerateIV(); // Generate a random IV

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Prepend IV to the ciphertext
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }


        public static string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length); // Extract the IV
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(fullCipher, 16, fullCipher.Length - 16))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}