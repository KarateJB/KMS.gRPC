using System;
using System.Linq;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Crypto.Models.DTO;
using Kms.Crypto.Services;
using Kms.gRPC.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Services.gRPC
{
    public partial class KeyVaultService : KeyVaulter.KeyVaulterBase
    {
        /// <summary>
        /// Get encrypted shared secret
        /// </summary>
        /// <param name="master">Master: who want to get the shared secret</param>
        /// <param name="client">Client: the owner of the shared secret</param>
        /// <returns>Cipher</returns>
        private async Task<string> getEncryptedAsymmetricKey(string client)
        {
            #region Get the Encrypt key for master
            var encryptKey =
                (await this.keyVault.FindAsync(
                    KeyEventHubUtils.ExpressAvailableClientKeys(
                        targetClientName: client.ToString(),
                        targetKeyType: KeyTypeEnum.TripleDes).Compile()))
                        .OrderByDescending(x => x.ActiveOn).FirstOrDefault();

            if (encryptKey == null)
            {
                this.logger.LogWarning($"No available encrypt key for client: {client.ToString()}!");
                return null;
            }
            #endregion

            #region Get the exist asymmetric key or create one

            var finalRsaKey = default(CipherKey);
            var oldRsaKey = (await this.keyVault.FindAsync(x => x.KeyType.Equals(KeyTypeEnum.Rsa) && x.Owner.Name.Equals(client))).FirstOrDefault();
            if (oldRsaKey != null && !KeyEventHubUtils.CheckIfDeprecateKey(oldRsaKey))
            {
                // The old key is good, use it
                finalRsaKey = oldRsaKey;
            }
            else
            {
                #region Revoke the expired shared secrets
                if (oldRsaKey != null)
                {
                    await this.keyVault.RevokeAsync(oldRsaKey.Id);
                    await this.keyVault.BackupAsync(oldRsaKey);
                    await this.keyVault.RemoveAsync(oldRsaKey.Id);
                }
                #endregion

                #region Start creating shared secrets
                var keyMeta = new KeyMetadata()
                {
                    Purpose = $"Create Asymmetric Key pair by {client}",
                    Owner = new CipherKeyOwner
                    {
                        Name = client,
                        Host = string.Empty
                    }
                };

                finalRsaKey = await this.keyVault.CreateRsaAsync(keyMeta);
                #endregion
            }

            #endregion

            #region Encrypt the asymmetric key

            using var es = new TripleDesService();
            string originData = JsonConvert.SerializeObject(finalRsaKey);
            var cipher = await es.EncryptAsync(encryptKey.Key1, originData);
            return cipher;
            #endregion
        }

        /// <summary>
        /// Get encrypted shared secret
        /// </summary>
        /// <param name="client">Client: who want to get the shared secret</param>
        /// <param name="owner">Owner: the owner of the shared secret</param>
        /// <returns>Cipher</returns>
        private async Task<string> getEncryptedShareSecret(string client, string owner)
        {
            #region Get the Encrypt key for master
            var encryptKey =
                (await this.keyVault.FindAsync(
                    KeyEventHubUtils.ExpressAvailableClientKeys(
                        targetClientName: client.ToString(),
                        targetKeyType: KeyTypeEnum.TripleDes).Compile()))
                        .OrderByDescending(x => x.ActiveOn).FirstOrDefault();

            if (encryptKey == null)
            {
                this.logger.LogWarning($"No available encrypt key for client: {owner.ToString()}!");
                return null;
            }
            #endregion

            #region Get the exist Shared secret or create them

            var finalSharedSecret = default(CipherKey);
            var oldSharedSecret = (await this.keyVault.FindAsync(x => x.KeyType.Equals(KeyTypeEnum.SharedSecret) && x.Owner.Name.Equals(owner))).FirstOrDefault();
            if (oldSharedSecret != null && !KeyEventHubUtils.CheckIfDeprecateKey(oldSharedSecret))
            {
                // The old Shared secret is good, use them
                finalSharedSecret = oldSharedSecret;
            }
            else
            {
                #region Revoke the expired shared secrets
                if (oldSharedSecret != null)
                {
                    await this.keyVault.RevokeAsync(oldSharedSecret.Id);
                    await this.keyVault.BackupAsync(oldSharedSecret);
                    await this.keyVault.RemoveAsync(oldSharedSecret.Id);
                }
                #endregion

                #region Start creating shared secrets
                var keyMeta = new KeyMetadata()
                {
                    Purpose = $"Create new shared secrets by {client}",
                    //Expando = new
                    //{
                    //    ClientId = ...,
                    //    Description = ...
                    //},
                    Owner = new CipherKeyOwner
                    {
                        Name = owner,
                        Host = string.Empty
                    }
                };

                finalSharedSecret = await this.keyVault.CreateSharedSecretAsync(keyMeta);
                #endregion
            }

            #endregion

            #region Encrypt the shared secret for master

            using (var es = new TripleDesService())
            {
                string originData = JsonConvert.SerializeObject(finalSharedSecret);
                var cipher = await es.EncryptAsync(encryptKey.Key1, originData);
                return cipher;
            }
            #endregion
        }

        private async Task<KeyAuditReport> auditAsync(string client, string encryptedKey)
        {
            var reportOn = DateTimeOffset.Now;
            
            try
            {
                #region Get the Decrypt key
                var decryptKey =
                    (await this.keyVault.FindAsync(
                        KeyEventHubUtils.ExpressAvailableClientKeys(
                            targetClientName: client.ToString(),
                            targetKeyType: KeyTypeEnum.TripleDes).Compile()))
                            .OrderByDescending(x => x.ActiveOn)
                            .FirstOrDefault();

                if (decryptKey == null)
                {
                    this.logger.LogWarning($"No available decrypt key for client: {client.ToString()}!");
                }
                #endregion

                #region Decrypt the working keys
                CipherKey workingKey = null;
                using (var es = new TripleDesService())
                {
                    var json = await es.DecryptAsync(decryptKey.Key1, encryptedKey);
                    workingKey = JsonConvert.DeserializeObject<CipherKey>(json);
                }
                #endregion

                #region Audit and save audit report(result) to KMS's memory cache

                var auditReport = await this.keyAuditReporter.AuditAsync(client, workingKey);
                return auditReport;
                #endregion
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Report working keys error");
                throw;
            }
        }
    }
}
