using System.Threading.Tasks;

namespace Kms.gRPC.Client.Services.Startup
{
    /// <summary>
    /// Interface for Startup services
    /// </summary>
    public interface IStartupService
    {
        /// <summary>
        /// Priority
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Start do something
        /// </summary>
        Task StartAsync();
    }
}
