namespace Kms.Core.Models.Config.Client
{
    /// <summary>
    /// AppSettings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// gRPC options
        /// </summary>
        public GrpcOptions Grpc { get; set; }

        /// <summary>
        /// KMS options
        /// </summary>
        public KmsClientOptions KmsClient { get; set; }
    }
}
