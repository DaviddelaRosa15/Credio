using Credio.Core.Application.Common.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Credio.Core.Application.Interfaces.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery { }

public interface ICachedQuery
{
    string CachedKey { get; }

    MemoryCacheEntryOptions? Options => null;
}