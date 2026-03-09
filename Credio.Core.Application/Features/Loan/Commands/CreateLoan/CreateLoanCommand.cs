using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.LoanApplications.Commands.CreateLoan;

public class CreateLoanCommand : ICommand<LoanDTO>
{
    [SwaggerParameter(Description = "Id de la solicitud")]
    public string LoanApplicationId { get; set; } = string.Empty;
    
    [SwaggerParameter(Description = "Id de metodo de amortización")]
    public string? AmortizationMethodId { get; set; } = string.Empty;

    [SwaggerParameter(Description = "Primera fecha de pago")]
    public DateOnly FirstPaymentDate { get; set; }
}

public class CreateLoanCommandHandler : ICommandHandler<CreateLoanCommand, LoanDTO>
{
    private readonly IAmortizationCalculatorService _calculatorService;
    private readonly IAmortizationMethodRepository _methodRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly ILoanStatusRepository _statusRepository;
    private readonly IMapper _mapper;


    public CreateLoanCommandHandler(
        IAmortizationCalculatorService calculatorService,
        IAmortizationMethodRepository methodRepository,
        ILoanRepository loanRepository,
        ILoanApplicationRepository loanApplicationRepository,
        ILoanStatusRepository statusRepository,
        IMapper mapper)
    {
        _calculatorService = calculatorService;
        _methodRepository = methodRepository;
        _loanRepository = loanRepository;
        _loanApplicationRepository = loanApplicationRepository;
        _statusRepository = statusRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<LoanDTO>> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar si la solicitud de préstamo existe y está aprobada
            var foundApplication = await _loanApplicationRepository.GetByIdWithIncludeAsync(x => x.Id == request.LoanApplicationId,
                [x => x.ApplicationStatus, x => x.Client, x => x.PaymentFrequency]);
            if (foundApplication is null || foundApplication.ApplicationStatus.Name != "Aprobada")
            {
                return Result<LoanDTO>.Failure(Error.NotFound("Su solicitud no ha sido procesada"));
            }

            // Verificar si ya existe un préstamo asociado a la solicitud
            var loanExists = await _loanRepository.GetByPropertyWithIncludeAsync(x => x.LoanApplicationId == request.LoanApplicationId,
                [x => x.Client, x => x.LoanStatus, x => x.PaymentFrequency]);
            if (loanExists is not null)
            {
                var response = _mapper.Map<LoanDTO>(loanExists);
                response.ClientName = loanExists.Client.FirstName + " " + loanExists.Client.LastName;
                response.LoanStatus = loanExists.LoanStatus.Name;
                response.Frequency = loanExists.PaymentFrequency.Name;
                return Result<LoanDTO>.Success(response);
            }

            // Validar la primera fecha de pago según la frecuencia de pagos aprobada
            var validationMessage = ValidateFirstPaymentDate(request.FirstPaymentDate, foundApplication.PaymentFrequency.DaysInterval);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return Result<LoanDTO>.Failure(Error.BadRequest(validationMessage));
            }

            // Obtener el último número de préstamo para asignar el siguiente número
            var lastNumber = await _loanRepository.GetLastLoanNumberAsync();

            // Calcular el cronograma de pagos utilizando el servicio de amortización
            var paymentSchedule = _calculatorService.Calculate((decimal)foundApplication.ApprovedAmount.Value, (decimal)foundApplication.ApprovedInterestRate.Value,
                (int)foundApplication.ApprovedTerm, request.FirstPaymentDate, foundApplication.PaymentFrequency.DaysInterval);

            // Obtener la última fecha de pago del cronograma para establecer la fecha de vencimiento del préstamo
            var lastPayment = paymentSchedule.LastOrDefault();

            // Obtener el método de amortización por defecto (Cuota Fija) si no se proporciona uno en la solicitud
            var amortizationMethod = await _methodRepository.GetByPropertyAsync(x => x.Name == "Cuota Fija");

            // Obtener el estado de préstamo "Activo" para asignarlo al nuevo préstamo
            var loanStatus = await _statusRepository.GetByPropertyAsync(x => x.Name == "Activo");

            Domain.Entities.Loan newLoan = new()
            {
                AmortizationMethodId = string.IsNullOrEmpty(request.AmortizationMethodId) ? amortizationMethod.Id : request.AmortizationMethodId,
                Amount = (double)foundApplication.ApprovedAmount,
                ClientId = foundApplication.ClientId,
                EffectiveDate = DateOnly.FromDateTime(DateTime.Now),
                EmployeeId = foundApplication.Client.EmployeeId,
                FirstPaymentDate = request.FirstPaymentDate,
                InterestRate = (double)foundApplication.ApprovedInterestRate,
                LoanApplicationId = foundApplication.Id,
                LoanNumber = lastNumber + 1,
                LoanStatusId = loanStatus.Id, // Estado inicial del préstamo
                MaturityDate = lastPayment.DueDate,
                PaymentFrequencyId = foundApplication.PaymentFrequencyId,
                Term = foundApplication.ApprovedTerm ?? foundApplication.RequestTerm            
            };

            var result = await _loanRepository.AddAsync(newLoan);

            var loanResponse = _mapper.Map<LoanDTO>(result);
            loanResponse.ClientName = foundApplication.Client.FirstName + " " + foundApplication.Client.LastName;
            loanResponse.LoanStatus = loanStatus.Name;
            loanResponse.Frequency = foundApplication.PaymentFrequency.Name;

            return Result<LoanDTO>.Success(loanResponse);
        }
        catch 
        {
            return Result<LoanDTO>.Failure(Error.InternalServerError("Ocurrió un error al intentar crear el préstamo"));
        }
    }

    private string ValidateFirstPaymentDate(DateOnly firstPaymentDate, int daysInterval)
    {
        if (daysInterval == 30 && firstPaymentDate < DateOnly.FromDateTime(DateTime.Now).AddMonths(1))
        {
            return $"La primera fecha de pago debe ser después del {DateOnly.FromDateTime(DateTime.Now).AddMonths(1).ToString("dd/MM/yyyy")}";
        }else if (daysInterval == 15 && firstPaymentDate < DateOnly.FromDateTime(DateTime.Now).AddDays(15))
        {
            return $"La primera fecha de pago debe ser después del {DateOnly.FromDateTime(DateTime.Now).AddDays(15).ToString("dd/MM/yyyy")}";
        }else if (daysInterval == 7 && firstPaymentDate < DateOnly.FromDateTime(DateTime.Now).AddDays(7))
        {
            return $"La primera fecha de pago debe ser después del {DateOnly.FromDateTime(DateTime.Now).AddDays(7).ToString("dd/MM/yyyy")}";
        }
        return string.Empty;
    }
}