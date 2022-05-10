using Microsoft.Extensions.Caching.Memory;

namespace Core.Caching
{
    public interface ITradefactMemoryCache : IMemoryCache
    {
        MemoryCacheEntryOptions GetDefaultCacheEntryOptions();
    }
}
