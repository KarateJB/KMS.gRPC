using System;
using System.Collections.Generic;
using System.Text;

namespace Kms.Core
{
    public sealed partial class KeyAuditReports
    {
        public KeyAuditReports(List<KeyAuditReport> reports)
        {
            reports.ForEach(r => this.Reports.Add(r));
        }
    }
}
