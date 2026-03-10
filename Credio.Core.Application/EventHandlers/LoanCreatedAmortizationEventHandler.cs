using AutoMapper;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Contracts;
using Credio.Core.Domain.Entities;
using Credio.Core.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.EventHandlers;

public class LoanCreatedAmortizationEventHandler : IDomainEventHandler<LoanCreatedAmortizationEvent>
{
    private readonly IAmortizationCalculatorService _calculatorService;
    private readonly ILogger<LoanCreatedAmortizationEventHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAmortizationScheduleRepository _scheduleRepository;
    private readonly IAmortizationStatusRepository _statusRepository;

    public LoanCreatedAmortizationEventHandler(
        IAmortizationCalculatorService calculatorService,
        ILogger<LoanCreatedAmortizationEventHandler> logger,
        IMapper mapper,
        IAmortizationScheduleRepository scheduleRepository,
        IAmortizationStatusRepository statusRepository
        )
    {
        _calculatorService = calculatorService;
        _logger = logger;
        _mapper = mapper;
        _scheduleRepository = scheduleRepository;
        _statusRepository = statusRepository;
    }
    
    public async Task Handle(LoanCreatedAmortizationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando registro de cuotas");

            // Obtener el calendario de pagos utilizando el servicio de cálculo de amortización
            _logger.LogInformation("Calculando cuotas");
            var schedule = _calculatorService.Calculate((decimal)notification.Amount, (decimal)notification.InterestRate,
                notification.Term, notification.FirstPaymentDate, notification.DaysInterval);

            // Obtener el estado "Pendiente" para asignarlo a las cuotas
            var status = await _statusRepository.GetByPropertyAsync(x => x.Name == "Pendiente");

            // Mapear el calendario de pagos al modelo de entidad AmortizationSchedule
            // Asignar el estado y el ID del préstamo
            _logger.LogInformation("Mapeando cuotas al modelo de datos");
            List <AmortizationSchedule> paymentSchedule = _mapper.Map<List<AmortizationSchedule>>(schedule);
            paymentSchedule.ForEach(x => x.AmortizationStatusId = status.Id);
            paymentSchedule.ForEach(x => x.LoanId = notification.LoanId);

            // Guardar el calendario de pagos en la base de datos
            _logger.LogInformation("Guardando cuotas en la base de datos");
            await _scheduleRepository.AddManyAsync(paymentSchedule);

            _logger.LogInformation("Cuotas registradas exitosamente para el préstamo {LoanId}", notification.LoanId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular el calendario de pagos para el préstamo {LoanId}", notification.LoanId);
            throw;
        }
    }
}