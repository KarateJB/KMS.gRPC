using System;
using System.Collections.Generic;
using System.Text;

namespace Kms.Core.Models.Exceptions
{
    /// <summary>
    /// API exception
    /// </summary>
    [Serializable]
    public class ApiException : Exception
    {
        /// <summary>
        /// Error code
        /// </summary>
        protected ErrorCodeEnum errorCode = ErrorCodeEnum.Other;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errCode">Error code</param>
        /// <param name="message">Message</param>
        public ApiException(ErrorCodeEnum errCode, string message, Exception ex = null)
            : base(message)
        {
            this.errorCode = errCode;
            this.Exception = ex;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public ApiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected ApiException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Override Message to show high-level error message
        /// </summary>
        /// <remarks>Use this for client/frontend</remarks>
        public override string Message
        {
            get
            {
                return $"{{\"errorCode\":\"{(int)this.errorCode}\",\"message\":\"{base.Message}\"}}";
            }
        }
    }
}
