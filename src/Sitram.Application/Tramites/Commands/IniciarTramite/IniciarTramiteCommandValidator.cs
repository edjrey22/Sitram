using FluentValidation;

namespace Sitram.Application.Tramites.Commands.IniciarTramite;

/// <summary>Validación de entrada del comando (se ejecuta en el pipeline antes del handler).</summary>
public sealed class IniciarTramiteCommandValidator : AbstractValidator<IniciarTramiteCommand>
{
    public IniciarTramiteCommandValidator()
    {
        RuleFor(x => x.CiudadanoId).NotEmpty();
        RuleFor(x => x.TipoTramiteId).GreaterThan(0);
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(30);
    }
}
