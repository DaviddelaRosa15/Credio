using Credio.Core.Application.Common.Primitives;
using MediatR;

namespace Credio.Core.Application.Interfaces.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand { }

public interface ICommand : IRequest<Result>, IBaseCommand { }

public interface IBaseCommand { }