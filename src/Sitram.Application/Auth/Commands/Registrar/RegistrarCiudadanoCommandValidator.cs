using FluentValidation;

namespace Sitram.Application.Auth.Commands.Registrar;

public sealed class RegistrarCiudadanoCommandValidator : AbstractValidator<RegistrarCiudadanoCommand>
{
    public RegistrarCiudadanoCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
