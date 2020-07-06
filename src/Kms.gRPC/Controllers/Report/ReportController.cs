using System.Collections.Generic;
using System.Threading.Tasks;
using Kms.Core;
using Kms.Core.Mock;
using Kms.gRPC.Services.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kms.gRPC.Controllers.Report
{
    /// <summary>
    /// ReportController
    /// </summary>
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportController : Controller
    {
        private readonly ILogger logger = null;
        private readonly IKeyAuditReporter reporter = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportController(
            ILogger<ReportController> logger,
            IKeyAuditReporter reporter)
        {
            this.logger = logger;
            this.reporter = reporter;
        }

        /// <summary>
        /// Index
        /// </summary>
        [Route("Index")]
        public async Task<ActionResult> Index()
        {
            var viewModel = new List<KeyAuditReport>();

            var clients = new string[] { MockClients.Me };
            foreach (var client in clients)
            {
                var reports = await this.reporter.GetReportAsync(client);
                if (reports != null)
                {
                    viewModel.AddRange(reports);
                }
            }

            ViewData["Title"] = "Audit Report";
            return this.View(viewModel);
        }
    }
}
