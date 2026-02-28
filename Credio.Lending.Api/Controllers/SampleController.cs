using Credio.Lending.Api.Common;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Features.Sample.Command;
using Credio.Core.Application.Features.Sample.Query;
using Credio.Interface.Lending.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Credio.Interface.Lending.Controllers;

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

    [Authorize]
    [HttpGet]
    public async Task<IResult> Get(CancellationToken cancellationToken)
    {
        Result<string> result = await _sender.Send(new SampleQuery(), cancellationToken);
        
        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure:CustomResult.Problem);
    }
    
    [HttpGet("caching")]
    public async Task<IResult> GetCaching(CancellationToken cancellationToken)
    {
        Result<string> result = await _sender.Send(new SampleQueryCaching(), cancellationToken);
        
        return result.Match(
            onSuccess: () => CustomResult.Success(result),
            onFailure:CustomResult.Problem);
    }
}