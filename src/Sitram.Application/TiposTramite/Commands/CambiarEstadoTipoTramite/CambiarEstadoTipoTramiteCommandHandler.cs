using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.TiposTramite;

namespace Sitram.Application.TiposTramite.Commands.CambiarEstadoTipoTramite;

public sealed class CambiarEstadoTipoTramiteCommandHandler(ITipoTramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<CambiarEstadoTipoTramiteCommand>
{
    public async Task Handle(CambiarEstadoTipoTramiteCommand request, CancellationToken cancellationToken)
    {
        var tipoTramite = await repositorio.ObtenerPorIdAsync(request.TipoTramiteId, cancellationToken)
            ?? throw new TipoTramiteNoDisponibleException(request.TipoTramiteId);

        if (request.Activar) tipoTramite.Activar();
        else tipoTramite.Desactivar();

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
