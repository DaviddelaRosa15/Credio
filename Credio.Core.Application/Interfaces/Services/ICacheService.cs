using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Credio.Core.Application.Interfaces.Services;

public interface ICacheService
{
    void Set<T>(string key, T value, MemoryCacheEntryOptions? options = null);

    T? Get<T>(string key, JsonSerializerOptions? options = null);
    
    void Remove(string key);

    void RemoveByPrefix(string prefix);
}