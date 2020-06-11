using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Core.Utils.Extensions;
using Kms.gRPC.Services.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kms.gRPC.Services.Report
{
    /// <summary>
    /// Report working keys from client
    /// </summary>
    public class KeyAuditReporter : IKeyAuditReporter
    {
        private const string KeyPrefixAuditClientWorkingKeys = "ClientWorkingKeys";
        private readonly ILogger logger = null;
        private readonly IMemoryCache memoryCache = null;
        private readonly IKeyVault keyVault = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public KeyAuditReporter(
            ILogger<KeyAuditReporter> logger,
            IMemoryCache memoryCache,
            IKeyVault keyVault)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.keyVault = keyVault;
        }

        /// <summary>
        /// Audit the client working key and return the result
        /// </summary>
        /// <param name="key">Client's working key</param>
        /// <returns>The original key information and audit result</returns>
        public async Task<KeyAuditReport> AuditAsync([Required]string client, [Required]CipherKey key)
        {
            var result = new KeyAuditReport(key);
            var matchedKey = await this.keyVault.FindAsync(key.Id);

            result.Client = client;
            if (matchedKey != null)
            {
                result.IsMatched = true;
                result.KmsKeyId = matchedKey.Id;
            }
            else
            {
                result.IsMatched = false;
                var allKeys = await this.keyVault.GetAllAsync();
                if (allKeys != null)
                {
                    var serverWorkingKey = allKeys.FirstOrDefault(k => k.KeyType.Equals(key.KeyType) && k.Owner != null && k.Owner.Name.Equals(key.Owner.Name));
                    result.KmsKeyId = serverWorkingKey?.Id;
                }
            }

            result.ReportOn = DateTimeOffset.Now.ToProtobufTimestamp();

            return result;
        }

        /// <summary>
        /// Insert or update current working keys to Redis
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="auditRslt">Working keys</param>
        /// <returns>True(OK)/False(NG)</returns>
        public async Task MergeReportAsync(string client, List<KeyAuditReport> auditRslt)
        {
            this.memoryCache.Set(this.getKeyAuditClientWorkingKeys(client), auditRslt);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get client's working key report
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>Working keys report</returns>
        public async Task<List<KeyAuditReport>> GetReportAsync(string client)
        {
            this.memoryCache.TryGetValue(this.getKeyAuditClientWorkingKeys(client), out List<KeyAuditReport> reports);
            return await Task.FromResult(reports);
        }

        /// <summary>
        /// Key for AuditClientWorkingKeys
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>Cache key</returns>
        private string getKeyAuditClientWorkingKeys(string client) => $"{KeyPrefixAuditClientWorkingKeys}-{client.ToString()}";
    }
}
