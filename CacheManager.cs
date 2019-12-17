public class CacheManager : ICache, IDisposable
{
    private static object _padlock = new object();

    public InMemoryCache InMemoryCache { get; }
    public SqlCache SqlCache { get; }

    public CacheManager(string sqlConnection, int sqlSlidingExpiryTimeInSeconds, int memoryAbsoluteExpiryTimeInSeconds)
    {
        SqlCache = new SqlCache(sqlConnection, sqlSlidingExpiryTimeInSeconds);
        InMemoryCache = new InMemoryCache(memoryAbsoluteExpiryTimeInSeconds);
    }

    public T Get<T>(string key)
    {
        return TryGetFromMemoryThenFromSource<T>(key);
    }

    public void Put<T>(string key, T item)
    {
        lock(_padlock)
        {
            SqlCache.Put(key, item.ToByteArray());
            InMemoryCache.Put(key, item);
        }
    }

    private T TryGetFromMemoryThenFromSource<T>(string key)
    {
        T cacheItem;
        lock (_padlock)
        {
            cacheItem = InMemoryCache.Get<T>(key);
            if (cacheItem == null)
            {
                // Key not in cache, so get data.
                cacheItem = SqlCache.Get<T>(key);
                if (cacheItem != null)
                {
                    InMemoryCache.Put(key, cacheItem);
                }
            }
        }
        return cacheItem;
    }

    public void Dispose()
    {
        MemoryCache.Dispose();
        SqlCache.Dispose();
    }
}
