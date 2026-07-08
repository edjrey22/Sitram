using MediatR;
using Sitram.Application.Common.Interfaces;
using DomainTipoTramite = Sitram.Domain.TiposTramite.TipoTramite;
using Sitram.Domain.TiposTramite;

namespace Sitram.Application.TiposTramite.Commands.CrearTipoTramite;

public sealed class CrearTipoTramiteCommandHandler(ITipoTramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<CrearTipoTramiteCommand, int>
{
    public async Task<int> Handle(CrearTipoTramiteCommand request, CancellationToken cancellationToken)
    {
        var tipoTramite = DomainTipoTramite.Crear(
            request.Nombre, request.Descripcion, request.AreaResponsable, request.Tasa);

        await repositorio.AddAsync(tipoTramite, cancellationToken);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return tipoTramite.Id;
    }
}
