using FluentValidation;

namespace Credio.Core.Application.Features.Clients.Commands.UpdateClientCommand;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.clientId)
            .NotNull().WithMessage("ClientId is required")
            .NotEmpty().WithMessage("ClientId is required");
        
        RuleFor(x => x.phone)
            .Length(10).WithMessage("The phone must be 10 digits")
            .Matches(@"^(809|829|849)\d{7}$").WithMessage("The number format must be valid")
            .When(x => !string.IsNullOrEmpty(x.phone));
        
        RuleFor(command => command.age)
            .GreaterThan(18).WithMessage("The age must be greater than 0")
            .When(x => x is not null);
        
        RuleFor(command => command.email)
            .EmailAddress().WithMessage("The email is not valid")
            .When(x => !string.IsNullOrEmpty(x.email));
    }
}