using FluentValidation;

namespace Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;

public sealed class ListarTramitesCiudadanoQueryValidator : AbstractValidator<ListarTramitesCiudadanoQuery>
{
    public ListarTramitesCiudadanoQueryValidator()
    {
        RuleFor(x => x.CiudadanoId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
