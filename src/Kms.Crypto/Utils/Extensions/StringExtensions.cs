using System;

namespace Kms.Crypto.Utils.Extensions
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
        public static string Mask(this string text)
        {
            const string allMasked = "***********";

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            try
            {
                int maskLength = (int)Math.Floor((decimal)(text.Length / 3));
                int maskStart = maskLength;

                if (text.Length <= 5)
                {
                    return allMasked;
                }

                var textPrefix = text.Remove(maskStart, text.Length - maskLength);
                var textPostfix = text.Remove(0, textPrefix.Length + maskLength);
                var maskedText = maskStart <= 1 ? allMasked : $"{textPrefix}{string.Empty.PadRight(maskLength, '*')}{textPostfix}";
                return maskedText;
            }
            catch (Exception)
            {
                return allMasked;
            }
        }
    }
}
