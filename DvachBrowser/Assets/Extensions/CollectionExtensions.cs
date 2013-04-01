using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.Assets.Extensions
{
    public static class CollectionExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return GetValueOrDefault(source, key, default(TValue));
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            TValue value;
            if (source.TryGetValue(key, out value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
