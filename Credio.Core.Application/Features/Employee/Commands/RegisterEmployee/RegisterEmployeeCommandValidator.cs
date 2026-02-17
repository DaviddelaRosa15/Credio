using FluentValidation;

namespace Credio.Core.Application.Features.Employee.Commands.RegisterEmployee
{
    public class RegisterEmployeeCommandValidator : AbstractValidator<RegisterEmployeeCommand>
    {
        public RegisterEmployeeCommandValidator()
        {
            RuleFor(command => command.FirstName)
                .NotNull().WithMessage("The firstname can't be null")
                .NotEmpty().WithMessage("The firstname can't be empty");

            RuleFor(command => command.LastName)
                .NotNull().WithMessage("The lastname can't be null")
                .NotEmpty().WithMessage("The lastname can't be empty");

            RuleFor(command => command.DocumentType)
                .NotNull().WithMessage("The document type can't be null")
                .NotEmpty().WithMessage("The document type can't be empty");

            RuleFor(command => command.DocumentNumber)
                .NotNull().WithMessage("The document number can't be null")
                .NotEmpty().WithMessage("The document number can't be empty")
                .Length(11).WithMessage("The document number must be 11 digits");

            RuleFor(command => command.Phone)
                .NotNull().WithMessage("The phone number can't be null")
                .NotEmpty().WithMessage("The phone number can't be empty")
                .Length(10).WithMessage("The phone must be 10 digits")
                .Matches(@"^(809|829|849)\d{7}$").WithMessage("The number format must be valid");

            RuleFor(command => command.Email)
                .NotNull().WithMessage("The email can't be null")
                .NotEmpty().WithMessage("The email can't be empty")
                .EmailAddress().WithMessage("The email is not valid");

            RuleFor(command => command.Role)
                .NotNull().WithMessage("The role can't be null")
                .NotEmpty().WithMessage("The role can't be empty");
        }
    }
}
