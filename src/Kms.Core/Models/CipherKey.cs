using Kms.Core.Utils.Extensions;

namespace Kms.Core
{
    /// <summary>
    /// Key information
    /// </summary>
    public sealed partial class CipherKey
    {
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>The masked key information</returns>
        public string ToMaskedString()
        {
            var info =
                $"({this.Id}) {this.KeyType.ToString()} >> Owner: {this.Owner.Name} >> Key1(Public key): {this.Key1.Mask()} >> Key2(Private key): {this.Key2.Mask()}";
            return info;
        }

        public static partial class Types
        {
            public sealed partial class CipherKeyOwner
            {
                public CipherKeyOwner(string name, string host)
                {
                    this.Name = name;
                    this.Host = host;
                }
            }
        }
    }
}
