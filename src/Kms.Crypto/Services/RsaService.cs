using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using Kms.Crypto.Utils;
using Kms.Crypto.Utils.Extensions;
using XC.RSAUtil;
using static Kms.Core.CipherKey.Types;

namespace Kms.Crypto.Services
{
    /// <summary>
    /// RSA Service
    /// </summary>
    /// <remarks>Use RSA2 in default</remarks>
    public class RsaService : IAsymCryptoService, IDisposable
    {
        private readonly int keySize;
        private readonly int maxDataSize;
        private readonly HashAlgorithmName hashAlgorithm;
        private readonly RSASignaturePadding signaturePadding;
        private readonly RSAEncryptionPadding encryptionPadding;
        private readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Constructor using default RSA2 options
        /// </summary>
        public RsaService()
        {
            this.keySize = 2048;
            this.maxDataSize = this.keySize / 8; // RSA is only able to encrypt data with max size = key size
            this.hashAlgorithm = HashAlgorithmName.SHA256;
            this.signaturePadding = RSASignaturePadding.Pss;
            this.encryptionPadding = RSAEncryptionPadding.OaepSHA256;
            this.encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Create RSA Key pair as [private key, public key]
        /// </summary>
        /// <returns>Key list as [private key, public key]</returns>
        public IList<string> CreateKeyPair()
        {
            var keys = RsaKeyGenerator.Pkcs8Key(2048, false);
            return keys;
        }

        /// <summary>
        /// Create RSA key
        /// </summary>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        public CipherKey CreateKeyPair(KeyMetadata meta)
        {
            var keyList = this.CreateKeyPair();
            var privateKey = keyList[0];
            var publicKey = keyList[1];
            var key = CipherKeyUtils.Create(KeyTypeEnum.Rsa, publicKey, privateKey, meta);
            return key;
        }

        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public string SignSignature(CipherKey key, string data)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, key.Key1, key.Key2, this.keySize))
            {
                return rsaUtil.SignData(data, HashAlgorithmName.SHA256, this.signaturePadding);
            }
        }

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        public bool VerifySignature(CipherKey key, string data, string signature)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, key.Key1, key.Key2, this.keySize))
            {
                return rsaUtil.VerifyData(data, signature, this.hashAlgorithm, this.signaturePadding);
            }
        }

        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public string SignSignature(string privateKey, string data)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, string.Empty, privateKey, this.keySize))
            {
                return rsaUtil.SignData(data, HashAlgorithmName.SHA256, this.signaturePadding);
            }
        }

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        public bool VerifySignature(string publicKey, string data, string signature)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, publicKey, string.Empty, this.keySize))
            {
                return rsaUtil.VerifyData(data, signature, this.hashAlgorithm, this.signaturePadding);
            }
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        public string Encrypt(CipherKey key, string data)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, key.Key1, key.Key2))
            {
                return rsaUtil.EncryptByDataSize(this.maxDataSize, data, this.encryptionPadding);
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        public string Decrypt(CipherKey key, string cipherData)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, key.Key1, key.Key2))
            {
                return rsaUtil.DecryptByDataSize(this.maxDataSize, cipherData, this.encryptionPadding);
            }
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        public string Encrypt(string publicKey, string data)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, publicKey))
            {
                return rsaUtil.EncryptByDataSize(this.maxDataSize, data, this.encryptionPadding);
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        public string Decrypt(string privateKey, string cipherData)
        {
            using (var rsaUtil = new RsaPkcs8Util(this.encoding, string.Empty, privateKey))
            {
                return rsaUtil.DecryptByDataSize(this.maxDataSize, cipherData, this.encryptionPadding);
            }
        }

        /// <summary>
        /// Create RSA Key pair as [private key, public key]
        /// </summary>
        /// <returns>Key list as [private key, public key]</returns>
        public async Task<IList<string>> CreateKeyPairAsync()
        {
            var keys = await Task.Run(() => this.CreateKeyPair());
            return keys;
        }

        /// <summary>
        /// Create RSA key
        /// </summary>
        /// <param name="meta">Key's metadata</param>
        /// <returns>Cipherkey object</returns>
        public async Task<CipherKey> CreateKeyPairAsync(KeyMetadata meta)
        {
            var key = await Task.Run(() => this.CreateKeyPair(meta));
            return key;
        }

        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public async Task<string> SignSignatureAsync(CipherKey key, string data)
        {
            var signedData = await Task.Run(() => this.SignSignature(key, data));
            return signedData;
        }

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        public async Task<bool> VerifySignatureAsync(CipherKey key, string data, string signature)
        {
            var isVerifyOk = await Task.Run(() => this.VerifySignature(key, data, signature));
            return isVerifyOk;
        }

        /// <summary>
        /// Sign with private key
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public async Task<string> SignSignatureAsync(string privateKey, string data)
        {
            var signedData = await Task.Run(() => this.SignSignature(privateKey, data));
            return signedData;
        }

        /// <summary>
        /// Verify signature with public key
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="data">Original data</param>
        /// <param name="signature">Signed data (Signature)</param>
        /// <returns>True(Verify OK)/False(Verify NG)</returns>
        public async Task<bool> VerifySignatureAsync(string publicKey, string data, string signature)
        {
            var isVerifyOk = await Task.Run(() => this.VerifySignature(publicKey, data, signature));
            return isVerifyOk;
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        public async Task<string> EncryptAsync(CipherKey key, string data)
        {
            var encryptedData = await Task.Run(() => this.Encrypt(key, data));
            return encryptedData;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="key">CipherKey object</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        public async Task<string> DecryptAsync(CipherKey key, string cipherData)
        {
            var decryptedData = await Task.Run(() => this.Decrypt(key, cipherData));
            return decryptedData;
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted data</returns>
        public async Task<string> EncryptAsync(string publicKey, string data)
        {
            var encryptedData = await Task.Run(() => this.Encrypt(publicKey, data));
            return encryptedData;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="cipherData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        public async Task<string> DecryptAsync(string privateKey, string cipherData)
        {
            var decrypedData = await Task.Run(() => this.Decrypt(privateKey, cipherData));
            return decrypedData;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}