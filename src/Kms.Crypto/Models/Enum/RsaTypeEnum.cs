namespace Kms.Crypto.Models.Enum
{
    /// <summary>
    /// RSA Types
    /// </summary>
    public enum RsaTypeEnum
    {
        /// <summary>
        /// RSA
        /// </summary>
        /// <remarks>
        /// The length of private key must at least 1024, 2048 will be more secure
        /// </remarks>
        RSA = 0,

        /// <summary>
        /// RSA2
        /// </summary>
        /// <remarks>
        /// Signature(數位簽章) algorithm is SHA256WithRSA, which requirs RSA private key as legth 2048, more secure than RSA.
        /// </remarks>
        RSA2
    }
}
