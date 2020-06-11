using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Kms.Core;
using Kms.Core.Mock;
using Kms.Core.Utils.Extensions;
using Kms.Crypto.Services;
using Kms.KeyMngr.KeyManager;
using Kms.KeyMngr.Utils.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Client.Dispatcher
{
    /// <summary>
    /// KeyDispatcher
    /// </summary>
    public class KeyDispatcher : IKeyDispatcher
    {
        private const bool SUCCESS = true;
        private readonly ILogger logger = null;
        private readonly KeyManagerResolver keyManagerResolver = null;
        private readonly KeyVaulter.KeyVaulterClient keyVaulterClient = null;

        #region Constructor

        public KeyDispatcher(
             ILogger<KeyDispatcher> logger,
            KeyManagerResolver keyManagerResolver,
            KeyVaulter.KeyVaulterClient keyVaulterClient)
        {
            this.logger = logger;
            this.keyManagerResolver = keyManagerResolver;
            this.keyVaulterClient = keyVaulterClient;
        }
        #endregion

        #region Create Symmetric Key

        /// <summary>
        /// Create Symmetric Key
        /// </summary>
        /// <returns>True/False</returns>
        public async Task<bool> CreateSymmetricKeyAsync()
        {
            const string successMsg = "Successfully creating symmetric key from KMS.";
            const string failMsg = "Fails to create symmetric key from KMS!";

            this.logger.CustomLogDebug("Start creating symmetric key from KMS...");

            // Create Symmetric key
            CipherKey symmetricKey = await this.keyVaulterClient.CreateSymmentricKeyAsync(new KeyRequest { Client = MockClients.Me });

            if (symmetricKey != null)
            {
                var tripleDesKeyManager = this.keyManagerResolver(nameof(TripleDesKeyManager)) as TripleDesKeyManager;
                await tripleDesKeyManager.SaveKeyAsync(symmetricKey);
                this.logger.CustomLogDebug($"{successMsg} {symmetricKey.ToString()}");
            }
            else
            {
                this.logger.CustomLogError($"{failMsg}");
            }

            return SUCCESS;
        }
        #endregion

        #region Create SharedSecrets

        /// <summary>
        /// Create SharedSecrets
        /// </summary>
        /// <returns>True/False</returns>
        public async Task<bool> CreateSharedSecretsAsync()
        {
            const string successMsg = "Successfully creating shared secrets from KMS.";
            const string failMsg = "Fails to create shared secrets from KMS!";

            this.logger.CustomLogDebug("Start creating shared secrets from KMS...");

            using var stream = this.keyVaulterClient.CreateSharedSectets(new KeyRequest { Client = MockClients.Me });

            var responseProcessing = Task.Run(async () =>
            {
                var tripleDesKeyManager = this.keyManagerResolver(nameof(TripleDesKeyManager)) as TripleDesKeyManager;
                var sharedSecretManager = this.keyManagerResolver(nameof(SharedSecretKeyManager)) as SharedSecretKeyManager;

                // Get decrption key
                var decryptKey = await tripleDesKeyManager.GetKeyAsync(KeyTypeEnum.TripleDes);

                try
                {
                    await foreach (var encryptReply in stream.ResponseStream.ReadAllAsync())
                    {
                        using (var es = new TripleDesService())
                        {
                            var encryptedData = encryptReply.Cipher;
                            var decryptedData = es.Decrypt(decryptKey.Key1, encryptedData);
                            var sharedSecret = JsonConvert.DeserializeObject<CipherKey>(decryptedData);
                            await sharedSecretManager.UpdateKeysAsync(CipherKey.Types.KeyTypeEnum.SharedSecret, sharedSecret);

                            this.logger.CustomLogDebug($"{successMsg}");
                        }
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
                {
                    this.logger.LogError(ex, "Stream cancelled.");
                    throw;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error reading response.");
                    throw;
                }
            });

            try
            {
                await responseProcessing;
                return SUCCESS;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, failMsg);
                return !SUCCESS;
            }
        }
        #endregion

        #region CreateAsymmetricKeyAsync

        /// <summary>
        /// Create Asymmetric Key
        /// </summary>
        /// <returns>True/False</returns>
        public async Task<bool> CreateAsymmetricKeyAsync()
        {
            const string successMsg = "Successfully creating asymmetric key from KMS.";
            const string failMsg = "Fails to create asymmetric key from KMS!";

            this.logger.CustomLogDebug("Start creating asymmetric key from KMS...");

            // Create Asymmetric key
            EncryptedData encryptedData = await this.keyVaulterClient.CreateAsymmetricKeysAsync(new KeyRequest { Client = MockClients.Me });

            try
            {
                if (!(encryptedData == null || string.IsNullOrEmpty(encryptedData.Cipher)))
                {
                    var tripleDesKeyManager = this.keyManagerResolver(nameof(TripleDesKeyManager)) as TripleDesKeyManager;
                    var rsaKeyManager = this.keyManagerResolver(nameof(RsaKeyManager)) as RsaKeyManager;
                    var decryptKey = await tripleDesKeyManager.GetKeyAsync(KeyTypeEnum.TripleDes);

                    if (decryptKey == null)
                        throw new NullReferenceException("No available TripleDES key to decrypt.");

                    using var es = new TripleDesService();
                    var decryptedJson = es.Decrypt(decryptKey.Key1, encryptedData.Cipher);
                    var asymmetricKey = JsonConvert.DeserializeObject<CipherKey>(decryptedJson);
                    await rsaKeyManager.UpdateKeysAsync(CipherKey.Types.KeyTypeEnum.Rsa, asymmetricKey);

                    this.logger.CustomLogDebug($"{successMsg} {asymmetricKey.ToString()}");
                }
                else
                    throw new NullReferenceException("gRPC repondes null EncryptedData.");

                return SUCCESS;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, failMsg);
                return !SUCCESS;
            }
        }
        #endregion

        #region Audit Working Keys

        /// <summary>
        /// Audit Working Keys
        /// </summary>
        public async Task AuditWorkingKeysAsync()
        {
            const string successMsg = "Successfully auditing working keys to KMS.";

            this.logger.CustomLogDebug("Start audit working keys...");

            using var stream = keyVaulterClient.AuditWorkingKeys();

            try
            {
                var workingKeys = await this.getWorkingKeys();
                foreach (CipherKey wk in workingKeys)
                {
                    var encryptedData = await this.encryptKey(wk);
                    this.logger.CustomLogDebug($"Requesting audit for {wk.KeyType.ToString()} ({wk.Id})...");
                    await stream.RequestStream.WriteAsync(encryptedData);
                    await Task.Delay(2500); // Simulate delay
                }
            }
            catch (Exception ex)
            {
                this.logger.CustomLogError(ex.Message);
            }

            // Wait for request steaming done
            this.logger.CustomLogDebug("Completing request stream...");
            await stream.RequestStream.CompleteAsync();
            this.logger.CustomLogDebug("Request stream completed.");

            // Get the response
            var auditReports = await stream.ResponseAsync;

            if (auditReports?.Reports != null)
            {
                auditReports.Reports.ToList().ForEach(r =>
                {
                    if (r.IsMatched)
                        this.logger.CustomLogInfo($"Audit OK with {r.Key.KeyType.ToString()} key ({r.Key.Id})");
                    else
                        this.logger.CustomLogWarn(
                            $"Audit NG with {r.Key.KeyType.ToString()} key, ({r.Key.Id} != {r.KmsKeyId}");
                });
            }

            this.logger.CustomLogDebug($"{successMsg}");
        }
        #endregion

        #region Audit Working Keys (Bidirection)

        /// <summary>
        /// Audit Working Keys (Bidirection)
        /// </summary>
        public async Task AuditWorkingKeysBidAsync()
        {
            const string successMsg = "Successfully reporting working keys to KMS.";

            this.logger.CustomLogDebug("Start audit working keys...");

            using var stream = keyVaulterClient.AuditWorkingKeysBid();

            var responseProcessing = Task.Run(async () =>
            {

                try
                {
                    await foreach (var auditResult in stream.ResponseStream.ReadAllAsync())
                    {
                        if (auditResult.IsMatched)
                            this.logger.CustomLogInfo($"Audit OK with {auditResult.Key.KeyType.ToString()} key ({auditResult.Key.Id})");
                        else
                            this.logger.CustomLogWarn(
                                $"Audit NG with {auditResult.Key.KeyType.ToString()} key, ({auditResult.Key.Id} != {auditResult.KmsKeyId}");
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    this.logger.LogError(ex, "Stream cancelled.");
                    throw;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error reading response: " + ex);
                    throw;
                }
            });


            try
            {
                var workingKeys = await this.getWorkingKeys();
                foreach (CipherKey wk in workingKeys)
                {
                    var encryptedData = await this.encryptKey(wk);
                    this.logger.CustomLogDebug($"Requesting audit for {wk.KeyType.ToString()} ({wk.Id})...");
                    await stream.RequestStream.WriteAsync(encryptedData);
                    await Task.Delay(2000); // Simulate delay
                }
            }
            catch (Exception ex)
            {
                this.logger.CustomLogError(ex.Message);
            }

            // Wait for request steaming done
            this.logger.CustomLogDebug("Completing request stream...");
            await stream.RequestStream.CompleteAsync();
            this.logger.CustomLogDebug("Request stream completed.");

            // Wait for response steaming done
            this.logger.CustomLogDebug("Completing response stream...");
            await responseProcessing;
            this.logger.CustomLogDebug("Response stream completed.");

            this.logger.CustomLogDebug($"{successMsg}");
        }
        #endregion

        private async Task<EncryptedData> encryptKey(CipherKey key)
        {
            var encryptedData = new EncryptedData()
            {
                Client = MockClients.Me,
                Cipher = string.Empty
            };

            var tripleDesKeyManager = this.keyManagerResolver(nameof(TripleDesKeyManager)) as TripleDesKeyManager;

            #region Get decrption key
            var decryptKey = await tripleDesKeyManager.GetKeyAsync(KeyTypeEnum.TripleDes);

            if (decryptKey == null)
            {
                this.logger.CustomLogError("No available TripleDes key to decrypt!");
                return encryptedData;
            }
            #endregion


            #region Encrypt the key
            string cipher = string.Empty;
            using (var es = new TripleDesService())
            {
                string json = JsonConvert.SerializeObject(key);
                cipher = await es.EncryptAsync(decryptKey.Key1, json);
                encryptedData.Cipher = cipher;
            }
            #endregion

            return encryptedData;
        }

        private async Task<List<CipherKey>> getWorkingKeys()
        {
            var tripleDesKeyManager = this.keyManagerResolver(nameof(TripleDesKeyManager)) as TripleDesKeyManager;
            var sharedSecretManager = this.keyManagerResolver(nameof(SharedSecretKeyManager)) as SharedSecretKeyManager;
            var rsaKeyManager = this.keyManagerResolver(nameof(RsaKeyManager)) as RsaKeyManager;

            #region Get all working keys
            var workingKeys = new List<CipherKey>();

            // Symmetric key
            var tripleDesKey = await tripleDesKeyManager.GetKeyAsync(KeyTypeEnum.TripleDes);
            workingKeys.Add(tripleDesKey);

            // Shared secret
            var sharedSecrets = await sharedSecretManager.GetKeysAsync(KeyTypeEnum.SharedSecret);
            if (sharedSecrets != null && sharedSecrets.Count > 0)
            {
                sharedSecrets.ToList().ForEach(k => workingKeys.Add(k));
            }

            // RSA
            var rsaKeys = await rsaKeyManager.GetKeysAsync(KeyTypeEnum.Rsa);
            if (rsaKeys != null && rsaKeys.Count > 0)
            {
                rsaKeys.ToList().ForEach(k => workingKeys.Add(k));
            }

            #endregion

            return workingKeys;
        }
    }
}
