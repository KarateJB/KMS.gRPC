using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using Kms.Core;
using Kms.Core.Mock;
using Kms.Core.Models.Config.Server;
using Kms.Core.Utils.Extensions;
using Kms.Crypto.Models.DTO;
using Kms.gRPC.Services.DataProtection;
using Kms.gRPC.Services.Report;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kms.gRPC.Services.gRPC
{
    public partial class KeyVaultService : KeyVaulter.KeyVaulterBase
    {
        private const int DefaultReportNotifyTime = 600;
        private readonly AppSettings appSettings = null;
        private readonly ILogger<KeyVaultService> logger = null;
        private readonly IKeyVault keyVault = null;
        private readonly IKeyAuditReporter keyAuditReporter = null;

        public KeyVaultService(
            IOptions<AppSettings> configuration,
            ILogger<KeyVaultService> logger,
            IKeyVault keyVault,
            IKeyAuditReporter keyAuditReporter)
        {
            this.appSettings = configuration.Value;
            this.logger = logger;
            this.keyVault = keyVault;
            this.keyAuditReporter = keyAuditReporter;
        }

        public override async Task<CipherKey> CreateSymmentricKey(KeyRequest request, ServerCallContext context)
        {
            var remoteIpAddress = context.GetHttpContext().Connection.RemoteIpAddress;
            var now = DateTimeOffset.Now;
            var keyMeta = new KeyMetadata()
            {
                Owner = new Core.CipherKey.Types.CipherKeyOwner(
                    name: request.Client,
                    host: remoteIpAddress.IsIPv4MappedToIPv6 ? remoteIpAddress.MapToIPv4().ToString() : remoteIpAddress.ToString()),
                Purpose = "Create Symmetric Key"
            };

            var key = await this.keyVault.CreateTripleDesAsync(keyMeta);

            return key;
        }

        public override async Task<EncryptedData> CreateAsymmetricKeys(KeyRequest request, ServerCallContext context)
        {
            var remoteIpAddress = context.GetHttpContext().Connection.RemoteIpAddress;
            var now = DateTimeOffset.Now;
            var keyMeta = new KeyMetadata()
            {
                Owner = new Core.CipherKey.Types.CipherKeyOwner(
                    name: request.Client,
                    host: remoteIpAddress.IsIPv4MappedToIPv6 ? remoteIpAddress.MapToIPv4().ToString() : remoteIpAddress.ToString()),
                Purpose = "Create Asymmetric Key"
            };

            var encryptedKey = await this.getEncryptedAsymmetricKey(request.Client);

            return new EncryptedData { Client = request.Client, Cipher = encryptedKey };
        }

        public override async Task CreateSharedSectets(
            KeyRequest request, IServerStreamWriter<EncryptedData> responseStream, ServerCallContext context)
        {
            this.logger.LogDebug("Start streaming for creating multiple shared secrets...");

            // we'll use a channel here to handle in-process 'messages' concurrently being written to and read from the channel.
            var channel = Channel.CreateUnbounded<EncryptedData>();

            // Background task which uses async streams to write each data from the channel to the response stream.
            _ = Task.Run(async () =>
            {
                await foreach (var encryptedReply in channel.Reader.ReadAllAsync())
                {
                    await responseStream.WriteAsync(encryptedReply);
                }
            });

            IList<Task> createKeyTasks = new List<Task>();

            // a local function which defines a task to handle creating key
            // multiple instances of this will run concurrently for single request
            async Task GetSharedSecretsAsync(string master)
            {
                // Request information
                var remoteIpAddress = context.GetHttpContext().Connection.RemoteIpAddress;
                var now = DateTimeOffset.Now;

                foreach (var mockClient in MockDataFactory.Clients)
                {
                    var encryptedKey = await this.getEncryptedShareSecret(master, mockClient);

                    // write the forecast to the channel which will be picked up concurrently by the channel reading background task
                    await channel.Writer.WriteAsync(new EncryptedData
                    {
                        Cipher = encryptedKey
                    });

                    await Task.Delay(2000); // Simulate delay 
                }
            }

            await GetSharedSecretsAsync(request.Client);

            channel.Writer.TryComplete();

            this.logger.LogDebug("Completed response streaming for creating multiple shared secrets.");
        }

        public override async Task<KeyAuditReports> AuditWorkingKeys(
            IAsyncStreamReader<EncryptedData> requestStream,
            ServerCallContext context)
        {
            string client = string.Empty;
            var auditReports = new List<KeyAuditReport>();

            // A list of tasks handling requests concurrently
            var reportTasks = new List<Task>();

            await foreach (var encryptedData in requestStream.ReadAllAsync())
            {
                client = encryptedData.Client;
                this.logger.CustomLogDebug($"Getting report from {client}");

                // Get audit report
                var auditReportTask = this.auditAsync(encryptedData.Client, encryptedData.Cipher);

                // Add the request handling task
                reportTasks.Add(auditReportTask);

                // Add the audited report(result) to list
                auditReports.Add(auditReportTask.Result);
            }

            // Wait for all responses to be written to the channel 
            await Task.WhenAll(reportTasks);

            // Save the audit reports to memory cache
            await this.keyAuditReporter.MergeReportAsync(client, auditReports);

            return new KeyAuditReports(auditReports);
        }

        public override async Task AuditWorkingKeysBid(
            IAsyncStreamReader<EncryptedData> requestStream,
            IServerStreamWriter<KeyAuditReport> responseStream,
            ServerCallContext context)
        {
            string client = string.Empty;
            var auditReports = new List<KeyAuditReport>();

            // Use a channel here to handle in-process 'messages' concurrently being written to and read from the channel.
            var channel = Channel.CreateUnbounded<KeyAuditReport>();

            // Background task which uses async streams to write each request from the channel to the response steam.
            _ = Task.Run(async () =>
            {
                await foreach (var keyAuditReport in channel.Reader.ReadAllAsync())
                {
                    await responseStream.WriteAsync(keyAuditReport);
                }
            });

            // A list of tasks handling requests concurrently
            var reportTasks = new List<Task>();

            await foreach (var encryptedData in requestStream.ReadAllAsync())
            {
                client = encryptedData.Client;
                this.logger.CustomLogDebug($"Getting report from {client}");

                // Get audit report
                var auditReportTask = this.auditAsync(encryptedData.Client, encryptedData.Cipher);

                // Add the request handling task
                reportTasks.Add(auditReportTask);

                // Add the audited report(result) to list
                auditReports.Add(auditReportTask.Result);

                // Write the response result to the channel which will be picked up concurrently by the channel reading background task
                await channel.Writer.WriteAsync(auditReportTask.Result);
            }

            // Wait for all responses to be written to the channel 
            await Task.WhenAll(reportTasks);

            channel.Writer.TryComplete();

            // Wait for all responses to be read from the channel and streamed as responses
            await channel.Reader.Completion;

            // Save the audit reports to memory cache
            await this.keyAuditReporter.MergeReportAsync(client, auditReports);

            this.logger.LogDebug("Completed response streaming");
        }
    }
}
