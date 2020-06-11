using System;
using System.Collections.Generic;
using static Kms.Core.CipherKey.Types;

namespace Kms.Crypto.Models.DTO
{
    /// <summary>
    /// Key Metadata
    /// </summary>
    public class KeyMetadata
    {
        /// <summary>
        /// Purpose
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// Expando
        /// </summary>
        public object Expando { get; set; }

        /// <summary>
        /// Active on
        /// </summary>
        public DateTimeOffset? ActiveOn { get; set; }

        /// <summary>
        /// Expired on
        /// </summary>
        public DateTimeOffset? ExpireOn { get; set; }

        /// <summary>
        /// Key Owner
        /// </summary>
        public CipherKeyOwner Owner { get; set; }

        /// <summary>
        /// Key Owner
        /// </summary>
        public IList<CipherKeyUser> Users { get; set; }
    }
}
