using FluentValidation;

namespace Sitram.Application.TiposTramite.Commands.CrearTipoTramite;

public sealed class CrearTipoTramiteCommandValidator : AbstractValidator<CrearTipoTramiteCommand>
{
    public CrearTipoTramiteCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.AreaResponsable).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(500);
        RuleFor(x => x.Tasa).GreaterThanOrEqualTo(0);
    }
}
