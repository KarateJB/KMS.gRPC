namespace Kms.Core.Models.Config
{
    /// <summary>
    /// Default key option
    /// </summary>
    public class DefaultKeyExpireOptions
    {
        /// <summary>
        /// Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Month
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Day
        /// </summary>
        public int? Day { get; set; }

        /// <summary>
        /// Hour
        /// </summary>
        public int? Hour { get; set; }

        /// <summary>
        /// Minute
        /// </summary>
        public int? Minute { get; set; }

        /// <summary>
        /// Second
        /// </summary>
        public int? Second { get; set; }
    }
}
