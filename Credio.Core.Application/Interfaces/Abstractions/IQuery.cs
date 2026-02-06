using Credio.Core.Application.Common.Primitives;
using MediatR;

namespace Credio.Core.Application.Interfaces.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseQuery { }

public interface IBaseQuery { }