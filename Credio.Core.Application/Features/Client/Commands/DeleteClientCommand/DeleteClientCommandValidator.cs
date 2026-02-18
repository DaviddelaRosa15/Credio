using FluentValidation;

namespace Credio.Core.Application.Features.Clients.Commands.DeleteClientCommand;

public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
{
    public DeleteClientCommandValidator()
    {
        RuleFor(c => c.cliendId)
            .NotNull().WithMessage("ClientId cannot be null")
            .NotEmpty().WithMessage("ClientId cannot be empty");
    }
}