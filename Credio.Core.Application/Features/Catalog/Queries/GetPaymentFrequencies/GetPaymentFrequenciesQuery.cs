using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Catalog;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;


namespace Credio.Core.Application.Features.Catalog.Queries.GetPaymentFrequencies
{
    public class GetPaymentFrequenciesQuery : PaginationRequest, ICachedQuery<List<PaymentFrequencyDTO>>
    {
        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetPaymentFrequenciesQuery_{PageNumber}_{PageSize}";
    }

    public class GetLoanStatusesQueryHandler : IQueryHandler<GetPaymentFrequenciesQuery, List<PaymentFrequencyDTO>>
    {
        private readonly IPaymentFrequencyRepository _paymentFrequencyRepository;
        private readonly IMapper _mapper;

        public GetLoanStatusesQueryHandler(IPaymentFrequencyRepository paymentFrequencyRepository, IMapper mapper)
        {
            _paymentFrequencyRepository = paymentFrequencyRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<PaymentFrequencyDTO>>> Handle(GetPaymentFrequenciesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var Payments = await _paymentFrequencyRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.PaymentFrequency, object>>>
                    {
                        m => m.Loans
                    }
                );

                var paymentDTOs = _mapper.Map<List<PaymentFrequencyDTO>>(Payments.Items);

                return Result<List<PaymentFrequencyDTO>>.Success(paymentDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<PaymentFrequencyDTO>>.Failure(Error.InternalServerError("Hubo un error al consultar la frecuencia de pago"));
            }
        }
    }
}
