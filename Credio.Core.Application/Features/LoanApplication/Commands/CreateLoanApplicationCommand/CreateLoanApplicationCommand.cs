using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Helpers;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Credio.Core.Application.Features.LoanApplications.Commands.CreateLoanApplicationCommand;

public class CreateLoanApplicationCommand : ICommand<LoanApplicationDto>
{
    [SwaggerParameter(Description = "Tasa de interés solicitada")]
    public double RequestedInterestRate { get; set; } 
    
    [SwaggerParameter(Description = "Cantidad solicitada")]
    public double RequestedAmount { get; set; }
    
    [SwaggerParameter(Description = "Termino solicitado")]
    public int RequestedTerm { get; set; }

    [SwaggerParameter(Description = "Id del cliente")]
    public string ClientId { get; set; } = string.Empty;
    
    [SwaggerParameter(Description = "Id del empleado")]
    public string EmployeeId { get; set; } = string.Empty;
}

public class CreateLoanApplicationCommandHandler : ICommandHandler<CreateLoanApplicationCommand, LoanApplicationDto>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly IApplicationStatusRepository _applicationStatusRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;


    public CreateLoanApplicationCommandHandler(
        ILoanApplicationRepository loanApplicationRepository,
        IApplicationStatusRepository applicationStatusRepository,
        IClientRepository clientRepository,
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _loanApplicationRepository = loanApplicationRepository;
        _applicationStatusRepository = applicationStatusRepository;
        _clientRepository = clientRepository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<LoanApplicationDto>> Handle(CreateLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            ApplicationStatus? foundApplicationStatus = await _applicationStatusRepository.GetByPropertyAsync(x => x.Name == "Pendiente");

            if (foundApplicationStatus is null)
            {
                return Result<LoanApplicationDto>.Failure(Error.NotFound("No se pudo encontrar el estado de aplicacion de pendiente"));
            }
            
            bool clientExists = await _clientRepository.ExistsAsync(x => x.Id == request.ClientId, cancellationToken);
            
            if (!clientExists)
            {
                return Result<LoanApplicationDto>.Failure(Error.NotFound("No se pudo encontrar el cliente"));
            }
            
            bool employeeExists = await _employeeRepository.ExistsAsync(x => x.Id == request.EmployeeId, cancellationToken);

            if (!employeeExists)
            {
                return Result<LoanApplicationDto>.Failure(Error.NotFound("No se pudo encontrar el empleado"));
            }

            LoanApplication newLoanApplication = new LoanApplication
            {
                ApplicationCode = ApplicationCodeGenerator.Generate(),
                ClientId = request.ClientId,
                EmployeeId = request.EmployeeId,
                RequestedAmount = request.RequestedAmount,
                RequestTerm = request.RequestedTerm,
                ApplicationStatusId = foundApplicationStatus.Id,
                RequestedInterestRate = request.RequestedInterestRate
            };
        
            LoanApplication createdLoanApplication = await _loanApplicationRepository.AddAsync(newLoanApplication);
        
            LoanApplication result = await _loanApplicationRepository
                .GetByIdWithIncludeAsync(x => x.Id  == createdLoanApplication.Id, [x => x.Client, x => x.ApplicationStatus]);
        
            return Result<LoanApplicationDto>.Success(_mapper.Map<LoanApplicationDto>(result));
        }
        catch 
        {
            return Result<LoanApplicationDto>.Failure(Error.NotFound("Ocurrio un error al momento de crear la solicitud de prestamo"));
        }
    }
}