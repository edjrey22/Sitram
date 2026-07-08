using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.IniciarTramite;

/// <summary>Caso de uso: crea el agregado <see cref="Tramite"/> y lo persiste (RF-020).</summary>
public sealed class IniciarTramiteCommandHandler(
    ITramiteRepository repositorio, ITipoTramiteRepository tipoTramiteRepositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<IniciarTramiteCommand, Guid>
{
    public async Task<Guid> Handle(IniciarTramiteCommand request, CancellationToken cancellationToken)
    {
        // Corrige el hueco de integridad: el tipo de trámite debe existir y estar activo.
        var tipoTramite = await tipoTramiteRepositorio.ObtenerPorIdAsync(request.TipoTramiteId, cancellationToken);
        if (tipoTramite is null || !tipoTramite.Activo)
            throw new TipoTramiteNoDisponibleException(request.TipoTramiteId);

        var tramite = Tramite.Crear(request.CiudadanoId, request.TipoTramiteId, request.Codigo);

        await repositorio.AddAsync(tramite, cancellationToken);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return tramite.Id.Value;
    }
}
