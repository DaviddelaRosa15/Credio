using FluentValidation;

namespace Credio.Core.Application.Features.CoreConfiguration.Commands.UpdateSystemSetting;

public class UpdateSystemSettingCommandValidator : AbstractValidator<UpdateSystemSettingCommand>
{
    public UpdateSystemSettingCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotNull().WithMessage("El identificador no puede estar vacio")
            .NotEmpty().WithMessage("El identificador no puede estar vacio");

        RuleFor(x => x.Value)
            .NotNull().WithMessage("El valor no puede estar nulo")
            .NotEmpty().WithMessage("El valor no puede estar vacio");
    }
}