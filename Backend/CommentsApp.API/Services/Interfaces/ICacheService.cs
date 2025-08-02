namespace CommentsApp.API.Services.Interfaces;

public interface ICacheService
{
    object? GetFromCache(string key);
    void SetToCache<T>(string key, T value, TimeSpan duration);
    void ClearCommentCache();
}