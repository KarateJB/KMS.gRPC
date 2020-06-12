using System.Collections.Generic;
using Kms.Core;

namespace Kms.gRPC.Services.CheckKey
{
    /// <summary>
    /// KeyCheckEvertArgs
    /// </summary>
    public class KeyCheckEventArgs
    {
        /// <summary>
        /// Decrecated keys
        /// </summary>
        public IReadOnlyCollection<CipherKey> DeprecatedKeys { get; set; }
    }
}
