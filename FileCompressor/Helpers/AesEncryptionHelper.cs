using System.Security.Cryptography;
using System.Text;

namespace FileCompressorApp.Helpers
{
    public static class AesEncryptionHelper
    {
        public static byte[] Encrypt(byte[] plainBytes, string password)
        {
            byte[] key = DeriveKey(password);
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); 

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length); 

            using var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            return ms.ToArray();
        }

        public static byte[] Decrypt(byte[] encryptedData, string password, out bool success)
        {
            success = false;

            try
            {
                byte[] key = DeriveKey(password);

                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[16];
                Array.Copy(encryptedData, 0, iv, 0, 16);
                aes.IV = iv;

                using var ms = new MemoryStream();
                using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(encryptedData, 16, encryptedData.Length - 16);
                cryptoStream.FlushFinalBlock();

                success = true;
                return ms.ToArray();
            }
            catch
            {
                success = false;
                return Array.Empty<byte>();
            }
        }
        private static byte[] DeriveKey(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}