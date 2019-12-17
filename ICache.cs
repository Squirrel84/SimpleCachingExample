public interface ICache
{
    T Get<T>(string key);
    void Put<T>(string key, T item);
}
