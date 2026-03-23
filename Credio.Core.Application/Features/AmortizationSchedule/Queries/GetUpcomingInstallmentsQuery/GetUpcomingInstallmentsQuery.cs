using AutoMapper;
using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Features.AmortizationSchedules.Queries.GetUpcomingInstallmentsQuery;

public class GetUpcomingInstallmentsQuery : PaginationRequest, ICachedQuery<List<UpcomingInstallmentDTO>>
{
    public string CachedKey => $"GetUpcomingInstallmentsQuery_{PageNumber}_{PageSize}";
}

public class GetUpcomingInstallmentsQueryHandler : IQueryHandler<GetUpcomingInstallmentsQuery,List<UpcomingInstallmentDTO>>
{
    private readonly IAmortizationScheduleRepository _amortizationScheduleRepository;
    private readonly IMapper _mapper;

    public GetUpcomingInstallmentsQueryHandler(
        IAmortizationScheduleRepository amortizationScheduleRepository,
        IMapper mapper)
    {
        _amortizationScheduleRepository = amortizationScheduleRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<List<UpcomingInstallmentDTO>>> Handle(GetUpcomingInstallmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Getting a DateOnly instance from the current date
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            // Monday of the current week
            int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(-diff));

            // Getting a DateOnly instance from today in 7 days
            DateOnly endDate = today.AddDays(7);
            
            PagedResult<AmortizationSchedule> result = await _amortizationScheduleRepository.GetPagedAsync(
                request.PageNumber, request.PageSize,
                [props => props.Loan, props => props.Loan.Client, props => props.AmortizationStatus],
                predicate => predicate.AmortizationStatus.Description == "Pendiente" &&
                             predicate.DueDate >= startOfWeek && predicate.DueDate <= endDate);

            List<UpcomingInstallmentDTO>? installmentsDto = _mapper.Map<List<UpcomingInstallmentDTO>>(result.Items);

            return Result<List<UpcomingInstallmentDTO>>.Success(installmentsDto);
        }
        catch
        {
            return Result<List<UpcomingInstallmentDTO>>.Failure(Error.InternalServerError("Hubo un error al intentar consultar las cuotas que están a punto de vencer"));
        }
    }
}