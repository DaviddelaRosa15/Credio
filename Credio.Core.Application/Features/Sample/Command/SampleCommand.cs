using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;

namespace Credio.Core.Application.Features.Sample.Command;

public record SampleCommand(string username, string password) : ICommand<string>;

public class SampleCommandHandler : ICommandHandler<SampleCommand, string>
{
    public async Task<Result<string>> Handle(SampleCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        if (request.username == "admin") return Result<string>.Failure(Error.BadRequest("The username 'admin' is reserved."));

        return Result<string>.Success("Hello world");
    }
}