namespace Kms.Core.Models.Config
{
    /// <summary>
    /// Key base options
    /// </summary>
    public class KeyBaseOptions
    {
        /// <summary>
        /// Default key expiry
        /// </summary>
        public DefaultKeyExpireOptions DefaultKeyExpire { get; set; }
    }

    /// <summary>
    /// Symmetric key options
    /// </summary>
    public class SymmetricKeyOption : KeyBaseOptions
    {
    }

    /// <summary>
    /// Shared secret options
    /// </summary>
    public class SharedSecretOption : KeyBaseOptions
    {
    }

    /// <summary>
    /// Asymmetric key options
    /// </summary>
    public class AsymmetricKeyOption : KeyBaseOptions
    {
    }
}
