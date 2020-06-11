using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Kms.KeyMngr.Utils.Extensions
{
    /// <summary>
    /// Object extensions
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Check if match filters
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="filters">Filters with key(prop name) and expected value</param>
        /// <returns>True(All matched)/False(Not matched)</returns>
        public static bool MatchFilters(this object obj, IDictionary<string, string> filters)
        {
            Type type = obj.GetType();

            foreach (var filter in filters)
            {
                string expected = filter.Value;
                string actual = string.Empty;
                switch (obj)
                {
                    case JObject j:
                        actual = ((JObject)obj)[filter.Key].ToString();
                        if (actual != expected)
                        {
                            return false;
                        }

                        break;
                    case object o:
                    default:
                        actual = type.GetProperty(filter.Key)?.GetValue(obj, null)?.ToString();
                        if (actual != expected)
                        {
                            return false;
                        }

                        break;
                }
            }

            return true;
        }
    }
}
