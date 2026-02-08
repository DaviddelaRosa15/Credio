using System.Text.Json;
using System.Text.Json.Serialization;
using Credio.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Credio.Infrastructure.Shared.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;
    
    public CacheService(
        IMemoryCache memoryCache,
        ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true,
    };

    private static readonly MemoryCacheEntryOptions DefaultOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        Priority = CacheItemPriority.Normal,
    };
    
    public void Set<T>(string key, T value, MemoryCacheEntryOptions? options = null)
    {
        try
        {
            string serializedValue = JsonSerializer.Serialize(value, SerializerOptions);
            
            _memoryCache.Set(key, serializedValue, options ?? DefaultOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "An unexpected error happen trying to set a value to the cache with the error message '{message}'.",
                ex.Message);

            throw;
        }
    }

    public T? Get<T>(string key)
    {
        try
        {
            string? value = _memoryCache.Get<string>(key);

            if (value is null)
            {
                return default;
            }

            T? deserializedValue = JsonSerializer.Deserialize<T>(value);

            return deserializedValue;

        }
        catch (Exception ex)
        {
            _logger.LogError(
                "An unexpected error happen trying to retrieve a value from the cache with the error message '{message}'.",
                ex.Message);

            throw;
        }
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}