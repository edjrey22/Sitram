using FluentValidation;

namespace Sitram.Application.Administracion.Commands.CrearCuentaFuncionario;

public sealed class CrearCuentaFuncionarioCommandValidator : AbstractValidator<CrearCuentaFuncionarioCommand>
{
    public CrearCuentaFuncionarioCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Rol)
            .NotEmpty()
            .Must(rol => RolesFuncionario.Permitidos.Contains(rol))
            .WithMessage($"El rol debe ser uno de: {string.Join(", ", RolesFuncionario.Permitidos)}.");
    }
}
