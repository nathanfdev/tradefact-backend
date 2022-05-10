using Core;
using Core.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Tradefact.Api.Common.Caching
{
    public class CachingOptions
    {
        public bool CacheEnabled { get; set; } = true;

        public TimeSpan? CacheAbsoluteExpiration { get; set; }
        public TimeSpan? CacheSlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);
    }
}
