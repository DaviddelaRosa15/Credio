using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Requests;
using Credio.Core.Application.Features.Clients.Commands.CreateClientCommand;
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
   
   [Authorize(Roles =  "Administrator")]
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
      Result result = await _sender.Send(command, cancellationToken);

      return result.Match(
         onSuccess: Results.Created,
         onFailure: CustomResult.Problem);
   }
   
   [Authorize(Roles =  "Administrator")]
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

   [Authorize(Roles = "Administrator")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
    Summary = "Listado de clientes",
    Description = "Obtiene la lista paginada de clientes"
    )]
    public async Task<IResult> GetClients(
    [FromQuery] GetClientsQuery query,
    CancellationToken cancellationToken)
    {
        Result<PagedResponse<ClientListDto>> result =
            await _sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: response =>
            {
                Response.Headers.Append("X-Pagination",
                    JsonSerializer.Serialize(new
                    {
                        response.TotalPages,
                        response.HasNext
                    }));

                return Results.Ok(response.Data);
            },
            onFailure: CustomResult.Problem
        );
    }
}