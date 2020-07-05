using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Kms.Client.Dispatcher.Services;
using Kms.gRPC.Client.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kms.gRPC.Client.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly ILogger logger;
        private readonly IKeyDispatcher keyDispatcher;
        private readonly IMemoryCache memoryCache;

        public HomeController(
            ILogger<HomeController> logger,
            IKeyDispatcher keyDispatcher,
            IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.keyDispatcher = keyDispatcher;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            string logs = string.Empty;

            // Greeting
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });

            ViewBag.logs = new HtmlString(reply.Message);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
