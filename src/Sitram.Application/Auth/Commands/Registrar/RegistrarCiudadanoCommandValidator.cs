using FluentValidation;

namespace Sitram.Application.Auth.Commands.Registrar;

public sealed class RegistrarCiudadanoCommandValidator : AbstractValidator<RegistrarCiudadanoCommand>
{
    public RegistrarCiudadanoCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dni).NotEmpty().Matches(@"^\d{8}$").WithMessage("El DNI debe tener 8 dígitos numéricos.");
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Direccion).NotEmpty().MaximumLength(200);
    }
}
