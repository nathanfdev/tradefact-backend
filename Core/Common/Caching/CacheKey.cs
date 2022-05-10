using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Caching
{
    public static class CacheKey
    {
        public static string With(params string[] keys)
        {
            return string.Join("-", keys);
        }

        public static string With(Type ownerType, params string[] keys)
        {
            return With($"{ownerType.GetCacheKey()}:{string.Join("-", keys)}");
        }


    }
}
