namespace Kms.gRPC.Utils.Factory
{
    /// <summary>
    /// Redis key factory
    /// </summary>
    public static class RedisKeyFactory
    {
        /// <summary>
        /// Key for Key Vault
        /// </summary>
        public static string KeyVault
        {
            get { return "KeyVault"; }
        }

        /// <summary>
        /// Key for Deprecated-Key Vault
        /// </summary>
        public static string KeyVaultDeprecated
        {
            get { return "KeyVaultDeprecated"; }
        }
    }
}
