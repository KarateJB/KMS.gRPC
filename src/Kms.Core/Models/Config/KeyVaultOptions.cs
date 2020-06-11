namespace Kms.Core.Models.Config
{
    /// <summary>
    /// KeyVault options 
    /// </summary>
    public class KeyVaultOptions
    {
        /// <summary>
        /// Symmetric key options
        /// </summary>
        public SymmetricKeyOption Symmetric { get; set; }

        /// <summary>
        /// Shared secret options
        /// </summary>
        public SharedSecretOption SharedSecret { get; set; }

        /// <summary>
        /// Asymmetric key-pair options
        /// </summary>
        public AsymmetricKeyOption Asymmetric { get; set; }
    }
}
