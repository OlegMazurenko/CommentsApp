using CommentsApp.API.Services.Interfaces;

using Microsoft.Extensions.Caching.Memory;

namespace CommentsApp.API.Services;

public class CacheService(IMemoryCache cache) : ICacheService
{
    private readonly IMemoryCache _cache = cache;
    private static readonly IList<string> _commentCacheKeys = [];

    public object? GetFromCache(string key)
    {
        _cache.TryGetValue(key, out var value);

        return value;
    }

    public void SetToCache<T>(string key, T value, TimeSpan duration)
    {
        _cache.Set(key, value, duration);

        lock (_commentCacheKeys)
        {
            if (!_commentCacheKeys.Contains(key))
            {
                _commentCacheKeys.Add(key);
            }
        }
    }

    public void ClearCommentCache()
    {
        lock (_commentCacheKeys)
        {
            foreach (var key in _commentCacheKeys)
            {
                _cache.Remove(key);
            }

            _commentCacheKeys.Clear();
        }
    }
}
