using FluentValidation;

namespace Credio.Core.Application.Features.Account.Commands.Authenticate
{
    public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
    {
        public AuthenticateCommandValidator()
        {
            RuleFor(command => command.UserName)
                .NotNull().WithMessage("The username can't be null")
                .NotEmpty().WithMessage("The username can't be empty")
                .MinimumLength(2).WithMessage("The username must contain at least two characters.")
                .MaximumLength(20).WithMessage("The username must not exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("The username can only contain letters, numbers, and underscores.");

            RuleFor(command => command.Password)
                .MinimumLength(8).WithMessage("The password must contain at least eight characters.")
                .NotNull().WithMessage("The password can't be null")
                .NotEmpty().WithMessage("The password can't be empty");
        }
    }
}
