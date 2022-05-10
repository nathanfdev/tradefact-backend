using Core;
using Core.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Tradefact.Infrastructure.Caching
{
    public class TradefactMemoryCache: ITradefactMemoryCache
    {
        private bool _disposed;
        private readonly ILogger _log;

        private readonly IMemoryCache _memoryCache;

        private readonly PlatformOptions _tradefactOptions;
        protected TimeSpan? AbsoluteExpiration => _tradefactOptions.CacheAbsoluteExpiration;
        protected TimeSpan? SlidingExpiration => _tradefactOptions.CacheSlidingExpiration;

        protected bool CacheEnabled => _tradefactOptions.CacheEnabled;
        public TradefactMemoryCache(IMemoryCache memoryCache, IOptions<PlatformOptions> options, ILoggerFactory loggerFactory)
        {
            _memoryCache = memoryCache;
            _tradefactOptions = options.Value;
            _log = loggerFactory?.CreateLogger<TradefactMemoryCache>();
        }

        public MemoryCacheEntryOptions GetDefaultCacheEntryOptions()
        {
            var result = new MemoryCacheEntryOptions();

            if (!CacheEnabled)
            {
                result.AbsoluteExpirationRelativeToNow = TimeSpan.FromTicks(1);
            }
            else
            {
                if (AbsoluteExpiration != null)
                {
                    result.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;
                }
                else if (SlidingExpiration != null)
                {
                    result.SlidingExpiration = SlidingExpiration;
                }
            }
            return result;
        }

        public virtual ICacheEntry CreateEntry(object key)
        {
            var result = _memoryCache.CreateEntry(key);
            if (result != null)
            {
                result.RegisterPostEvictionCallback(callback: EvictionCallback);
                var options = GetDefaultCacheEntryOptions();
                result.SetOptions(options);
            }
            return result;
        }

        public virtual void Remove(object key)
        {
            _memoryCache.Remove(key);
        }

        public virtual bool TryGetValue(object key, out object value)
        {
            var result = _memoryCache.TryGetValue(key, out value);
            return result;
        }

        ~TradefactMemoryCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _memoryCache.Dispose();
                }
                _disposed = true;
            }
        }

        protected virtual void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            _log.LogInformation($"EvictionCallback: Cache with key {key} has expired.");
        }
    }
}
