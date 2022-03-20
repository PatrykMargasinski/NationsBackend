using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NationsAPI.Utils
{
    public class SecurityUtils : ISecurityUtils
    {
        private readonly IConfiguration _config;
        public SecurityUtils(IConfiguration config)
        {
            _config = config;
        }

        private readonly byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        public string Decrypt(string text)
        {
            byte[] bytes = Convert.FromBase64String(text);
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            var key = _config.GetValue<string>("Security:EncryptKey");
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(key));
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[bytes.Length];
                    cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    return Encoding.Unicode.GetString(decryptedBytes);
                }
            }
        }

        public string Encrypt(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.BlockSize = 128;
            var key = _config.GetValue<string>("Security:EncryptKey");
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(key));
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public string Hash(string text)
        {
            byte[] salt = new byte[16];
            var cryptoService = new RNGCryptoServiceProvider();
            cryptoService.GetBytes(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(text, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
