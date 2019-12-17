public class SqlCache : ICache, IDisposable
{
    public const int DEFAULT_SLIDING_EXPIRATION_TIME_IN_SECONDS = 20 * 60;

    private int SlidingCacheExpiryTimeInSeconds { get; }

    private void SpinUpCache(string sqlConnection)
    {
        SqlServerCacheOptions sqlServerCacheOptions = new SqlServerCacheOptions();
        sqlServerCacheOptions.ConnectionString = sqlConnection;
        sqlServerCacheOptions.SchemaName = "dbo";
        sqlServerCacheOptions.TableName = "TestCache";
        sqlServerCacheOptions.DefaultSlidingExpiration = TimeSpan.FromSeconds(SlidingCacheExpiryTimeInSeconds);
        _cache = new SqlServerCache(sqlServerCacheOptions);
    }

    private IDistributedCache _cache;

    public SqlCache(string sqlConnection, int slidingExpiryTimeInSeconds)
    {
        if (_cache == null)
        {
            SlidingCacheExpiryTimeInSeconds = slidingExpiryTimeInSeconds;
            SpinUpCache(sqlConnection);
        }
    }

    public T Get<T>(string key)
    {
        return _cache.Get(key).FromByteArray<T>();
    }

    public void Put<T>(string key, T item)
    {
        this.Put<T>(key, item, SlidingCacheExpiryTimeInSeconds);
    }

    public void Put<T>(string key, T item, int expirationInSeconds)
    {
        _cache.Set(key, item.ToByteArray(), new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromSeconds(expirationInSeconds) });
    }

    public void Dispose()
    {
        _cache = null;
    }
}
