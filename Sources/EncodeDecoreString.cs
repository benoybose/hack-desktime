using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DeskTime.Sources
{
    internal class EncodeDecodeString
    {
        public static class Global
        {
            public const string strPermutation = "jipijipijee";

            public const int bytePermutation1 = 25;

            public const int bytePermutation2 = 89;

            public const int bytePermutation3 = 23;

            public const int bytePermutation4 = 65;
        }

        public static bool IsBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0 || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
            {
                return false;
            }
            try
            {
                Decrypt(Convert.FromBase64String(base64String));
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static string Encrypt(string strData)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(strData)));
        }

        public static string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(strData)));
        }

        public static byte[] Encrypt(byte[] strData)
        {
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes("jipijipijee", new byte[4] { 25, 89, 23, 65 });
            MemoryStream memoryStream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passwordDeriveBytes.GetBytes(aes.KeySize / 8);
            aes.IV = passwordDeriveBytes.GetBytes(aes.BlockSize / 8);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(strData, 0, strData.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        public static byte[] Decrypt(byte[] strData)
        {
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes("jipijipijee", new byte[4] { 25, 89, 23, 65 });
            MemoryStream memoryStream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passwordDeriveBytes.GetBytes(aes.KeySize / 8);
            aes.IV = passwordDeriveBytes.GetBytes(aes.BlockSize / 8);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(strData, 0, strData.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }
    }
}
