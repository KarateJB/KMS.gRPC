using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Kms.Core.Utils.Extensions
{
    /// <summary>
    /// MemoryCache extensions
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logging with caller name
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="msg">Message</param>
        /// <param name="caller">Caller</param>
        public static void CustomLogTrace(this ILogger logger, string msg, [CallerMemberName] string caller = "")
        {
            string formattedMsg = $"<{caller}> {msg}";
            logger.LogTrace(formattedMsg);
        }

        /// <summary>
        /// Logging with caller name
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="msg">Message</param>
        /// <param name="caller">Caller</param>
        public static void CustomLogDebug(this ILogger logger, string msg, [CallerMemberName] string caller = "")
        {
            string formattedMsg = $"<{caller}> {msg}";
            logger.LogDebug(formattedMsg);
        }

        /// <summary>
        /// Logging with caller name
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="msg">Message</param>
        /// <param name="caller">Caller</param>
        public static void CustomLogInfo(this ILogger logger, string msg, [CallerMemberName] string caller = "")
        {
            string formattedMsg = $"<{caller}> {msg}";
            logger.LogInformation(formattedMsg);
        }

        /// <summary>
        /// Logging with caller name
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="msg">Message</param>
        /// <param name="caller">Caller</param>
        public static void CustomLogWarn(this ILogger logger, string msg, [CallerMemberName] string caller = "")
        {
            string formattedMsg = $"<{caller}> {msg}";
            logger.LogWarning(formattedMsg);
        }

        /// <summary>
        /// Logging with caller name
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="msg">Message</param>
        /// <param name="caller">Caller</param>
        public static void CustomLogError(this ILogger logger, string msg, [CallerMemberName] string caller = "")
        {
            string formattedMsg = $"<{caller}> {msg}";
            logger.LogError(formattedMsg);
        }

        /// <summary>
        /// Logging with caller name
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="msg">Message</param>
        /// <param name="caller">Caller</param>
        public static void CustomLogCritical(this ILogger logger, string msg, [CallerMemberName] string caller = "")
        {
            string formattedMsg = $"<{caller}> {msg}";
            logger.LogCritical(formattedMsg);
        }
    }
}
