namespace Kms.Core.Models.Config
{
    /// <summary>
    /// KMS options
    /// </summary>
    public class KmsClientOptions
    {
        /// <summary>
        /// Check key's status period (seconds)
        /// </summary>
        public int CheckKeyTime { get; set; }

        /// <summary>
        /// Report notification period (seconds)
        /// </summary>
        public int ReportNotifyTime { get; set; }
    }
}
