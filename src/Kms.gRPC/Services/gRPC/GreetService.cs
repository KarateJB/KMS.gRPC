using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Kms.gRPC.Services.gRPC
{
    public class GreetService : Greeter.GreeterBase
    {
        private readonly ILogger<GreetService> _logger;
        public GreetService(ILogger<GreetService> logger)
        {
            _logger = logger;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
