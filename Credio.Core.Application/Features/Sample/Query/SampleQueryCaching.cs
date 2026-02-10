using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;

namespace Credio.Core.Application.Features.Sample.Query;

public class SampleQueryCaching : ICachedQuery<string>
{
    public string CachedKey => "SampleQuery";
}

public class SampleQueryCachingHandler : IQueryHandler<SampleQueryCaching, string>
{
    public Task<Result<string>> Handle(SampleQueryCaching request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result<string>.Success("Hello Caching")); 
    }
}