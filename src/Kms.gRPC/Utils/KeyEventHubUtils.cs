using System;
using System.Linq.Expressions;
using Kms.Core;
using LinqKit;
using static Kms.Core.CipherKey.Types;

namespace Kms.gRPC.Utils
{
    /// <summary>
    /// Utlities for KeyEventHub
    /// </summary>
    public class KeyEventHubUtils
    {
        /// <summary>
        /// Get Available client keys filter condition
        /// </summary>
        /// <param name="targetClientName">Target client name</param>
        /// <param name="targetKeyType">Target key type</param>
        /// <returns>Filter condition</returns>
        public static Expression<Func<CipherKey, bool>> ExpressAvailableClientKeys(
            string targetClientName = "", KeyTypeEnum? targetKeyType = null)
        {
            var now = DateTimeOffset.Now;
            var predicateBuilder = PredicateBuilder.New<CipherKey>();
            predicateBuilder = predicateBuilder.And(x => !x.IsDeprecated);
            predicateBuilder = predicateBuilder.And(x => x.ActiveOn.ToDateTimeOffset() <= now);
            predicateBuilder = predicateBuilder.And(x => string.IsNullOrEmpty(targetClientName) ?
                true : x.Owner != null && x.Owner.Name == targetClientName);
            predicateBuilder = predicateBuilder.And(x => targetKeyType == null ? true : x.KeyType == targetKeyType.Value);

            var filterCondition = (Expression<Func<CipherKey, bool>>)predicateBuilder;
            return filterCondition;
        }

        /// <summary>
        /// Check if a key is deprecated
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True(Deprecated)/False(Alive and OK)</returns>
        public static bool CheckIfDeprecateKey(CipherKey key)
        {
            const bool isDeprecated = true;
            var now = DateTimeOffset.Now;

            if (key != null && !key.IsDeprecated && key.ExpireOn.ToDateTimeOffset() > now)
            {
                return !isDeprecated;
            }
            else
            {
                return isDeprecated;
            }
        }
    }
}
