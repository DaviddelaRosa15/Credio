using FluentValidation;

namespace Credio.Core.Application.Features.Clients.Commands.CreateClientCommand;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotNull().WithMessage("The firstname can't be null")
            .NotEmpty().WithMessage("The firstname can't be empty");

        RuleFor(command => command.LastName)
            .NotNull().WithMessage("The lastname can't be null")
            .NotEmpty().WithMessage("The lastname can't be empty");
        
        RuleFor(command => command.Age)
            .NotNull().WithMessage("The age can't be null")
            .NotEmpty().WithMessage("The age can't be empty")
            .GreaterThan(0).WithMessage("The age must be greater than 0");

        RuleFor(command => command.DocumentType)
            .NotNull().WithMessage("The document type can't be null")
            .NotEmpty().WithMessage("The document type can't be empty");
        
        RuleFor(command => command.Phone)
            .NotNull().WithMessage("The phone number can't be null")
            .NotEmpty().WithMessage("The phone number can't be empty")
            .Length(10).WithMessage("The phone must be 10 digits")
            .Matches(@"^(809|829|849)\d{7}$").WithMessage("The number format must be valid");
        
        RuleFor(command => command.DocumentNumber)
            .NotNull().WithMessage("The document number can't be null")
            .NotEmpty().WithMessage("The document number can't be empty");

        RuleFor(command => command.AddressDto)
            .NotNull().WithMessage("The address can't be null")
            .NotEmpty().WithMessage("The address can't be empty");
        
        RuleFor(command => command.EmployeeId)
            .NotNull().WithMessage("The employee id can't be null")
            .NotEmpty().WithMessage("The employee id can't be empty");
        
        RuleFor(command => command.HomeLatitude)
            .NotNull().WithMessage("The home latitude can't be null")
            .NotEmpty().WithMessage("The home latitude id can't be empty");
        
        RuleFor(command => command.HomeLongitude)
            .NotNull().WithMessage("The home longitude id can't be null")
            .NotEmpty().WithMessage("The home longitude can't be empty");
        
        RuleFor(command => command.Email)
            .NotNull().WithMessage("The email can't be null")
            .NotEmpty().WithMessage("The email can't be empty")
            .EmailAddress().WithMessage("The email is not valid");
    }
}