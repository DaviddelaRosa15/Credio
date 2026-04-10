using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Features.CoreConfiguration.Commands.PrepareEndOfDay
{
    public class PrepareEndOfDayCommand : ICommand<string>
    {

    }

    public class PrepareEndOfDayCommandHandler : ICommandHandler<PrepareEndOfDayCommand, string>
    {
        private readonly IEndOfDayService _endOfDayService;
        private readonly ILogger<PrepareEndOfDayCommandHandler> _logger;

        public PrepareEndOfDayCommandHandler(
            IEndOfDayService endOfDayService,
            ILogger<PrepareEndOfDayCommandHandler> logger)
        {
            _endOfDayService = endOfDayService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(PrepareEndOfDayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string logId = string.Empty;

                // Invocar al servicio para preparar el proceso de COB para el día de hoy
                _logger.LogInformation("Iniciando el proceso de preparación de COB para el día de hoy.");
                logId = await _endOfDayService.PrepareAsync();

                return Result<string>.Success(logId);
            }
            catch
            {
                return Result<string>.Failure(Error.InternalServerError("Ocurrio un error al preparar el proceso de COB para el dia de hoy"));
            }
        }
    }
}