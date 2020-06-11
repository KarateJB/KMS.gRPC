using System;

namespace Kms.Crypto.Factory
{
    /// <summary>
    /// Secret key factory
    /// </summary>
    public class SecretFactory
    {
        /// <summary>
        /// Create a secret
        /// </summary>
        /// <returns>Secret</returns>
        public static string Create()
        {
            var secret = $"{Guid.NewGuid().ToString()}-{DateTime.Now.Ticks}";
            var bytesEncode = System.Text.Encoding.UTF8.GetBytes(secret);
            return Convert.ToBase64String(bytesEncode);
        }
    }
}
