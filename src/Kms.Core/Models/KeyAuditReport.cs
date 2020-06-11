using System;
using System.Collections.Generic;
using System.Text;

namespace Kms.Core
{
    /// <summary>
    /// KeyAuditReport
    /// </summary>
    public sealed partial class KeyAuditReport
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        public KeyAuditReport(CipherKey key)
        {
            this.Key = key;
        }
    }
}
