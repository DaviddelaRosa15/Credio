using Credio.Core.Application.Common.Primitives;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Interfaces.Abstractions;
using Credio.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Credio.Core.Application.Features.CoreConfiguration.Commands.ProcessEndOfDayQueue
{
    public class ProcessEndOfDayQueueCommand : ICommand<EndOfDayProcessResponseDTO>
    {
        public string LogId { get; set; }
    }

    public class ProcessEndOfDayQueueCommandHandler : ICommandHandler<ProcessEndOfDayQueueCommand, EndOfDayProcessResponseDTO>
    {
        private readonly IEndOfDayService _endOfDayService;
        private readonly ILogger<ProcessEndOfDayQueueCommandHandler> _logger;

        public ProcessEndOfDayQueueCommandHandler(
            IEndOfDayService endOfDayService,
            ILogger<ProcessEndOfDayQueueCommandHandler> logger)
        {
            _endOfDayService = endOfDayService;
            _logger = logger;
        }

        public async Task<Result<EndOfDayProcessResponseDTO>> Handle(ProcessEndOfDayQueueCommand request, CancellationToken cancellationToken)
        {
            EndOfDayProcessResponseDTO responseDTO = new();

            // Invocando el servicio para procesar la cola de fin de día
            _logger.LogInformation("Iniciando el procesamiento de la cola de fin de día para LogId: {LogId}", request.LogId);
            responseDTO = await _endOfDayService.ProcessQueueAsync(request.LogId);

            return Result<EndOfDayProcessResponseDTO>.Success(responseDTO);
        }
    }
}