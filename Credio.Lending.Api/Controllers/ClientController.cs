using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Requests;
using Credio.Core.Application.Features.Client.Queries.GetAll;
using Credio.Core.Application.Features.Clients.Commands.CreateClientCommand;
using Credio.Core.Application.Features.Clients.Commands.DeleteClientCommand;
using Credio.Core.Application.Features.Clients.Queries;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers;

[Route("api/v1/client")]
[ApiController]
public class ClientController : ControllerBase
{
   private readonly ISender _sender;

   public ClientController(ISender sender)
   {
      _sender = sender;
   }
   
   [Authorize(Roles =  "Administrator, Officer")]
   [HttpPost("create")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status409Conflict)]
   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   [SwaggerOperation(
      Summary = "Registro de clientes",
      Description = "Cree clientes para usar en el sistema"
   )]
   public async Task<IResult> CreateClient([FromForm] CreateClientCommand command, CancellationToken cancellationToken)
   {
      Result<CreateClientCommandResponse> result = await _sender.Send(command, cancellationToken);

      return result.Match(
         onSuccess: () => CustomResult.Success(result),
         onFailure: CustomResult.Problem);
   }
   
   [Authorize(Roles =  "Administrator, Officer")]
   [HttpPut("update/{clientId}")]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   [SwaggerOperation(
      Summary = "Actualizacion de clientes",
      Description = "Actualize clientes que esten registrados en el sistema"
   )]
   public async Task<IResult> UpdateClient(string clientId,[FromBody] UpdateClientRequest request ,CancellationToken cancellationToken)
   {
      Result result = await _sender.Send(request.ToCommand(clientId), cancellationToken);

      return result.Match(
         onSuccess: Results.NoContent,
         onFailure: CustomResult.Problem);
   }
   
   [Authorize(Roles =  "Administrator")]
   [HttpDelete("delete/{clientId}")]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   [SwaggerOperation(
      Summary = "Eliminacion de clientes",
      Description = "Elimine clientes que esten registrados en el sistema"
   )]
   public async Task<IResult> DeleteClient(string clientId, CancellationToken cancellationToken)
   {
      Result result = await _sender.Send(new DeleteClientCommand(clientId), cancellationToken);

      return result.Match(
         onSuccess: Results.NoContent,
         onFailure:CustomResult.Problem);
   }

    [Authorize(Roles = "Administrator, Officer, Collector")]
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClientDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
    Summary = "Obtiene los clientes",
    Description = "Obtiene al cliente segun el numero de documento"
    )]
    public async Task<IResult> GetClients([FromQuery] GetAllClientQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure: CustomResult.Problem);
    }

    [Authorize(Roles = "Administrator, Officer, Collector")]
    [HttpGet("by-document/{documentNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
    Summary = "Obtiene el cliente por numero de documento",
    Description = "Obtiene al cliente segun el numero de documento"
    )]
    public async Task<IResult> GetClientByDocumentNumber(string documentNumber, CancellationToken cancellationToken)
    {
        Result<ClientDTO> result = await _sender.Send(new GetClientByDocumentNumberQuery(documentNumber), cancellationToken);

        return result.Match(
          onSuccess: () => CustomResult.Success(result),
          onFailure:CustomResult.Problem);
    }
}