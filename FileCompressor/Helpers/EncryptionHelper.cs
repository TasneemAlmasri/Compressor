using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileCompressorApp.Helpers
{
    public static class EncryptionHelper
    {
        // Derives AES key from password
        private static byte[] DeriveKey(string password, int keySize = 32)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password)).Take(keySize).ToArray();
            }
        }

        // Encrypts plain data with AES and random IV (returns IV + encrypted data)
        public static byte[] Encrypt(byte[] data, string password)
        {
            byte[] key = DeriveKey(password);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV(); // Random IV
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length); // Prepend IV

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }

        // Decrypts AES-encrypted data that has IV prepended
        public static byte[] Decrypt(byte[] encryptedData, string password)
        {
            byte[] key = DeriveKey(password);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length), decryptor, CryptoStreamMode.Read))
                {
                    cs.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
