using System;
using System.Collections.Specialized;

namespace Kms.Core.Utils.Extensions
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Mask the original string
        /// </summary>
        /// <param name="text">Original text</param>
        /// <returns>Masked text</returns>
        public static string Mask(this string text, int minLength = 5, int maxLength = 30)
        {
            const string ALL_MASKED = "***********";
            const int MIN_LENGTH_RESTRICT = 5;
            const int MAX_LENGTH_RESTRICT = 30;
            const int SPLIT_BY = 3;

            #region Check arguments

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (minLength < MIN_LENGTH_RESTRICT)
                minLength = MIN_LENGTH_RESTRICT;

            if (maxLength < MAX_LENGTH_RESTRICT)
                maxLength = MAX_LENGTH_RESTRICT;

            #endregion

            try
            {
                #region MyRegion

                if (text.Length <= minLength)
                {
                    return ALL_MASKED;
                }
                else if (text.Length > maxLength)
                {
                    int keepLength = (int)Math.Floor((decimal)(maxLength / SPLIT_BY));
                    text = text.Substring(0, keepLength) + text.Substring(text.Length - keepLength, keepLength);
                }

                #endregion

                #region Start masking the text

                int maskLength = (int)Math.Floor((decimal)(text.Length / SPLIT_BY));
                int maskStart = maskLength;

                

                var textPrefix = text.Remove(maskStart, text.Length - maskLength);
                var textPostfix = text.Remove(0, textPrefix.Length + maskLength);
                var maskedText = maskStart <= 1 ? ALL_MASKED : $"{textPrefix}{string.Empty.PadRight(maskLength, '*')}{textPostfix}";
                return maskedText;
                #endregion
            }
            catch (Exception ex)
            {
                return ALL_MASKED;
            }
        }
    }
}
