using System.Collections.Generic;
using System.Threading.Tasks;
using Kms.Client.Dispatcher.Services;
using Kms.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kms.gRPC.Client.Controllers
{
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IKeyDispatcher keyDispatcher;
        private readonly IMemoryCache memoryCache;

        public DemoController(
            ILogger<HomeController> logger,
            IKeyDispatcher keyDispatcher,
            IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.keyDispatcher = keyDispatcher;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("SharedSecrets/{client}")]
        public async Task<IReadOnlyCollection<CipherKey>> SharedSecrets([FromRoute]string client)
        {
            return await this.keyDispatcher.GetSharedSecretsAsync(client);
        }

        [HttpGet]
        [Route("PublicKeys/{client}")]
        public async Task<IReadOnlyCollection<CipherKey>> PublicKeys([FromRoute] string client, [FromQuery]IList<string> receivers)
        {
            return await this.keyDispatcher.GetPublicKeysAsync(client, receivers);
        }
    }
}
