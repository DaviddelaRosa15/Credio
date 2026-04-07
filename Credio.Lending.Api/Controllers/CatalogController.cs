using Credio.Core.Application.Dtos.Catalog;
using Credio.Core.Application.Dtos.LoanStatus;
using Credio.Core.Application.Features.Catalog.Queries.GetApplicationStatuses;
using Credio.Core.Application.Features.Catalog.Queries.GetDocumentTypes;
using Credio.Core.Application.Features.Catalog.Queries.GetPaymentFrequencies;
using Credio.Core.Application.Features.Catalog.Queries.GetPaymentMethods;
using Credio.Core.Application.Features.LoanStatus.Queries.GetLoanStatuses;
using Credio.Interface.Lending.Extensions;
using Credio.Lending.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Lending.Api.Controllers
{
    [Route("/api/v1/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {

        private readonly ISender _sender;

        public CatalogController(ISender sender)
        {
            _sender = sender;
        }

        [SwaggerOperation(
        Summary = "Obtiene los estados de los prestamos",
        Description = "Obtiene los prestamos activos"
        )]
        [Authorize(Roles = "Administrator, Officer")]
        [HttpGet("loan-statuses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanStatusDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetAllStatus([FromQuery] GetLoanStatusesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }

        [SwaggerOperation(
        Summary = "Obtiene la periodicidad de las cuotas",
        Description = "Obtiene la frecuencia de los pagos"
        )]
        [Authorize(Roles = "Administrator, Officer")]
        [HttpGet("payment-frequencies")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentFrequencyDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetPaymentFrequencies([FromQuery] GetPaymentFrequenciesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }

        [SwaggerOperation(
        Summary = "Obtiene el estado de las solicitudes de credito",
        Description = "Obtiene los estados de todas las solicitudes de credito que ha hecho el usuario"
        )]
        [Authorize(Roles = "Administrator, Officer")]
        [HttpGet("application-statuses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationStatusDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetApplicationStatuses([FromQuery] GetApplicationStatusesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }

        [SwaggerOperation(
        Summary = "Obtiene los tipos de documento",
        Description = "Obtiene los tipos de documentos con los cuales se obtiene la identidad de la persona"
        )]
        [Authorize(Roles = "Administrator, Officer")]
        [HttpGet("document-types")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DocumentTypeDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetDocumentTypes([FromQuery] GetDocumentTypesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }

        [SwaggerOperation(
        Summary = "Obtiene los metodos de pago",
        Description = "Obtiene los metodos de pagos admitidos por el sistema"
        )]
        [Authorize(Roles = "Cobrador , Admin,  Oficial , Administrator, Officer")]
        [HttpGet("payment-methods")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentMethodDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetPaymentMethod([FromQuery] GetPaymentMethodsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
              onSuccess: () => CustomResult.Success(result),
              onFailure: CustomResult.Problem);
        }
    }
}
