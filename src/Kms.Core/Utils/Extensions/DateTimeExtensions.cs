using System;

namespace Kms.Core.Utils.Extensions
{
    /// <summary>
    /// DateTime extensions
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// To DateTimeOffset
        /// </summary>
        /// <param name="timestamp">Google.Protobuf.WellKnownTypes.Timestamp</param>
        /// <returns>DateTimeOffset</returns>
        public static Google.Protobuf.WellKnownTypes.Timestamp ToProtobufTimestamp(this DateTimeOffset datetime)
        {
            return Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(datetime);
        }
    }
}
