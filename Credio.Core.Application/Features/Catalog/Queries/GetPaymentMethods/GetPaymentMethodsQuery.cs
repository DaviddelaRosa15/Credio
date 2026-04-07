using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Catalog;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Features.Catalog.Queries.GetPaymentFrequencies;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Credio.Core.Application.Features.Catalog.Queries.GetPaymentMethods
{
    public class GetPaymentMethodsQuery : PaginationRequest, ICachedQuery<List<PaymentMethodDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetPaymentMethodsQuery_{PageNumber}_{PageSize}";
    }

    public class GetPaymentMethodsQueryHandler : IQueryHandler<GetPaymentMethodsQuery, List<PaymentMethodDTO>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IMapper _mapper;

        public GetPaymentMethodsQueryHandler(IPaymentMethodRepository paymentMethodyRepository, IMapper mapper)
        {
            _paymentMethodRepository = paymentMethodyRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<PaymentMethodDTO>>> Handle(GetPaymentMethodsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var Payments = await _paymentMethodRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.PaymentMethod, object>>>
                    {

                    }
                );

                var paymentDTOs = _mapper.Map<List<PaymentMethodDTO>>(Payments.Items);

                return Result<List<PaymentMethodDTO>>.Success(paymentDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<PaymentMethodDTO>>.Failure(Error.InternalServerError("Hubo un error al consultar el metodo de pago"));
            }
        }
    }
}
