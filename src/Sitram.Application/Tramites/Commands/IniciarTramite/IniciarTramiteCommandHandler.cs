using MediatR;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.IniciarTramite;

/// <summary>Caso de uso: crea el agregado <see cref="Tramite"/> y lo persiste (RF-020).</summary>
public sealed class IniciarTramiteCommandHandler(ITramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<IniciarTramiteCommand, Guid>
{
    public async Task<Guid> Handle(IniciarTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = Tramite.Crear(request.CiudadanoId, request.TipoTramiteId, request.Codigo);

        await repositorio.AddAsync(tramite, cancellationToken);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return tramite.Id.Value;
    }
}
