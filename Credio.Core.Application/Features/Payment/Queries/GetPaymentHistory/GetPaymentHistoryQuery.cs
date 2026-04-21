using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Features.Client.Queries.GetAll;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Credio.Core.Application.Features.Payments.Queries.GetPaymentHistory
{
    public class GetPaymentHistoryQuery : PaginationRequest, ICachedQuery<List<PaymentHistoryDTO>>
    {
        public string? EmployeeId { get; set; }

        [JsonIgnore]
        [SwaggerIgnore]
        public string CachedKey => $"GetPaymentHistoryQuery{PageNumber}_{PageSize}_{EmployeeId}";
    }

    public class GetPaymentHistoryQueryHandler : IQueryHandler<GetPaymentHistoryQuery, List<PaymentHistoryDTO>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public GetPaymentHistoryQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<PaymentHistoryDTO>>> Handle(GetPaymentHistoryQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await _paymentRepository.GetPagedAsync(query.PageNumber, query.PageSize,
                    new List<Expression<Func<Domain.Entities.Payment, object>>>
                    {
                        m => m.Loan.Client,
                        m => m.PaymentMethod
                    },
                    !string.IsNullOrEmpty(query.EmployeeId)
                        ? (Expression<Func<Domain.Entities.Payment, bool>>)(c => c.EmployeeId == query.EmployeeId)
                        : null
                );

                var orderedPayments = payment.Items
                .OrderBy(p => p.PaymentDate)
                .ToList();

                var paymentDTOs = _mapper.Map<List<PaymentHistoryDTO>>(orderedPayments);


                return Result<List<PaymentHistoryDTO>>.Success(paymentDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<PaymentHistoryDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar el historial de pago"));
            }
        }
    }
}

