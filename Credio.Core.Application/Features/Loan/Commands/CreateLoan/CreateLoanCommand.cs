using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Constants;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Events;
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
    private readonly ICacheService _cacheService;
    private readonly IAmortizationCalculatorService _calculatorService;
    private readonly IAmortizationMethodRepository _methodRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly ILoanStatusRepository _statusRepository;
    private readonly IMapper _mapper;


    public CreateLoanCommandHandler(
        ICacheService cacheService,
        IAmortizationCalculatorService calculatorService,
        IAmortizationMethodRepository methodRepository,
        ILoanRepository loanRepository,
        ILoanApplicationRepository loanApplicationRepository,
        ILoanStatusRepository statusRepository,
        IMapper mapper)
    {
        _cacheService = cacheService;
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
                return Result<LoanDTO>.Failure(Error.BadRequest("Su solicitud no ha sido procesada"));
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

            // Obtener la última fecha de pago del cronograma para establecer la fecha de vencimiento del préstamo
            var lastPayment = _calculatorService.CalculateLastPaymentDate(request.FirstPaymentDate, (int)foundApplication.ApprovedTerm,
                foundApplication.PaymentFrequency.DaysInterval);

            // Obtener el método de amortización por defecto (Cuota Fija) si no se proporciona uno en la solicitud
            var amortizationMethod = await _methodRepository.GetByPropertyAsync(x => x.Name == "Cuota Fija");

            // Obtener el estado de préstamo "Creado" para asignarlo al nuevo préstamo
            var loanStatus = await _statusRepository.GetByPropertyAsync(x => x.Name == LoanStatuses.Created);

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
                MaturityDate = lastPayment,
                PaymentFrequencyId = foundApplication.PaymentFrequencyId,
                Term = foundApplication.ApprovedTerm ?? foundApplication.RequestTerm            
            };

            // Agregar evento de dominio para generar el cronograma de amortización
            newLoan.AddEvent(new LoanCreatedAmortizationEvent(newLoan.Id, newLoan.Amount, newLoan.Term, newLoan.InterestRate,
                newLoan.FirstPaymentDate, foundApplication.PaymentFrequency.DaysInterval));

            var loan = await _loanRepository.AddAsync(newLoan);

            var result = _mapper.Map<LoanDTO>(loan);
            result.ClientName = foundApplication.Client.FirstName + " " + foundApplication.Client.LastName;
            result.LoanStatus = loanStatus.Name;
            result.Frequency = foundApplication.PaymentFrequency.Name;

            // Remover de la cache las listas de préstamos para que se actualicen con el nuevo préstamo creado
            _cacheService.RemoveByPrefix("loan-all-");

            return Result<LoanDTO>.Success(result);
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