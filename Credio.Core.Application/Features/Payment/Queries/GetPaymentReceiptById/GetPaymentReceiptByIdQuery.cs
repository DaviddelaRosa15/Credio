using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Credio.Core.Application.Features.Payment.Queries.GetPaymentReceiptById
{
    public sealed class GetPaymentReceiptByIdQuery : ICachedQuery<PaymentReceiptDTO>
    {
        public GetPaymentReceiptByIdQuery(string Id)
        {
            this.Id = Id;
        }

        public string Id { get; set; }

        public string CachedKey => $"Payment-{Id}";
    }

    public sealed class GetPaymentReceiptByIdQueryHandler : IQueryHandler<GetPaymentReceiptByIdQuery, PaymentReceiptDTO>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public GetPaymentReceiptByIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<Result<PaymentReceiptDTO>> Handle(GetPaymentReceiptByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Id))
                {
                    return Result<PaymentReceiptDTO>.Failure(
                        Error.BadRequest("El campo id del pago no puede estar vacio")
                    );
                }

                var foundPayment = await _paymentRepository.GetByIdWithIncludeAsync(x => x.Id == request.Id,
                    query => query
                        .Include(x => x.Loan.Client)
                        .Include(x => x.PaymentMethod)
                        .Include(x => x.Loan.LoanBalance)
                        .Include(x => x.Loan.AmortizationSchedules)
                );


                if (foundPayment is null)
                    return Result<PaymentReceiptDTO>.Failure(Error.NotFound("No se encontro el pago con el id dado"));

                var PaymentDTO = _mapper.Map<PaymentReceiptDTO>(foundPayment);

                var memoryInstallments = PaymentDTO.TotalInstallments - PaymentDTO.PaidInstallments;


                return Result<PaymentReceiptDTO>.Success(PaymentDTO);
            }
            catch (Exception)
            {
                return Result<PaymentReceiptDTO>.Failure(
                    Error.InternalServerError($"Ocurrio un error inesperado consultando el recibo")
                );
            }
        }
    }
}

