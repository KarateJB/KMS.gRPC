namespace Kms.Core.Models.Config.Server
{
    /// <summary>
    /// AppSettings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Redis options
        /// </summary>
        public RedisOptions Redis { get; set; }

        /// <summary>
        /// KMS options
        /// </summary>
        public KmsOptions Kms { get; set; }
    }
}
