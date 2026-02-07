using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;

namespace Credio.Core.Application.Features.Sample.Query;

public record SampleQuery : IQuery<string> { }

public class SampleQueryHandler : IQueryHandler<SampleQuery, string>
{
    public Task<Result<string>> Handle(SampleQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result<string>.Success("Hello world!"));
    }
}