using System.Collections.Generic;
using System.Threading.Tasks;
using Kms.Core;

namespace Kms.gRPC.Services.Report
{
    /// <summary>
    /// Interface for reporting working keys from client
    /// </summary>
    public interface IKeyAuditReporter
    {
        /// <summary>
        /// Audit the client working key and return the result
        /// </summary>
        /// <param name="key">Client's working key</param>
        /// <returns>The original key information and audit result</returns>
        Task<KeyAuditReport> AuditAsync(string client, CipherKey key);

        /// <summary>
        /// Insert or update current working keys to Redis
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="reports">Working keys report</param>
        /// <returns>True(OK)/False(NG)</returns>
        Task MergeReportAsync(string client, List<KeyAuditReport> reports);

        /// <summary>
        /// Get client's working key report
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>Working keys report</returns>
        Task<List<KeyAuditReport>> GetReportAsync(string client);
    }
}
