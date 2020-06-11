namespace Kms.Core.Models.Config
{
    /// <summary>
    /// Redis's options
    /// </summary>
    public class RedisOptions
    {
        /// <summary>
        /// Redis host
        /// </summary>
        /// <example>localhost:6379</example>
        public string Host { get; set; }

        /// <summary>
        /// Timeout in millionsecond for SyncTimeout and ConnectTimeout
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Retry times when disconnected
        /// </summary>
        public int ConnectRetry { get; set; }
    }
}
