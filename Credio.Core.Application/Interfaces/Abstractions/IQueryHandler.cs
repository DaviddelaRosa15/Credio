using Credio.Core.Application.Common.Primitives;
using MediatR;

namespace Credio.Core.Application.Interfaces.Abstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }