using System.Reflection;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Extensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Credio.Core.Application.Common.Pipelines;

public class RequestValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationFailure[] failures = await ValidateAsync(request, cancellationToken);

        if (failures.Length <= 0)
        {
            return await next(cancellationToken);
        }

        if (typeof(TResponse).IsGenericType
             && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type genericType = typeof(TResponse).GetGenericArguments()[0];

            MethodInfo? failureMethod = typeof(Result<>).MakeGenericType(genericType)
                                                .GetMethod(nameof(Result<object>.Failure));

            if (failureMethod is not null)
            {
                #pragma warning disable CS8600
                #pragma warning disable CS8603
                return (TResponse)failureMethod.Invoke(null,
                                                       [CreateValidationError(failures)]);
            }
        }

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(CreateValidationError(failures));
        }

        throw new Exceptions.ValidationException(failures);
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        if (!_validators.Any())
        {
            return Array.Empty<ValidationFailure>();
        }

        ValidationContext<TRequest> validationContext = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await _validators.Select(validator => validator.ValidateAsync(request, cancellationToken)).WhenAll();

        ValidationFailure[] validationFailures = validationResults.Where(validation => !validation.IsValid)
                                                                  .SelectMany(validation => validation.Errors)
                                                                  .ToArray();
        return validationFailures;
    }
    private static ValidationError CreateValidationError(ValidationFailure[] failures)
            => new(failures.Select(x => Error.Validation(x.ErrorMessage)).ToArray());
}