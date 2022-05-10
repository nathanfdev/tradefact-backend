using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Core.Caching
{
    public static class AsyncLock
    {
        private static ConcurrentDictionary<string, Nito.AsyncEx.AsyncLock> _lockMap = new ConcurrentDictionary<string, Nito.AsyncEx.AsyncLock>();

        public static Nito.AsyncEx.AsyncLock GetLockByKey(string key)
        {
            return _lockMap.GetOrAdd(key, (x) => new Nito.AsyncEx.AsyncLock());
        }

    }
}
