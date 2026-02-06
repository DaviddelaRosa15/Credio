using Credio.Authentication.Api.Common;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Features.Sample.Command;
using Credio.Core.Application.Features.Sample.Query;
using Credio.Interface.Authentication.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Credio.Interface.Authentication.Controllers;

[Route("api/v1/sample")]
[ApiController]
public class SampleController : ControllerBase
{
    private readonly ISender _sender;

    public SampleController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    public async Task<IResult> CommandSample(SampleCommand command,CancellationToken cancellationToken)
    {
        Result<string> result = await _sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure: CustomResult.Problem);
    }

    [HttpGet]
    public async Task<IResult> Get(CancellationToken cancellationToken)
    {
        Result<string> result = await _sender.Send(new SampleQuery(), cancellationToken);
        
        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure:CustomResult.Problem);
    }
}