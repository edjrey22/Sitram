using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Seguridad;

namespace Sitram.Application.Seguridad.Commands.ResolverIncidenteSeguridad;

public sealed class ResolverIncidenteSeguridadCommandHandler(
    IIncidenteSeguridadRepository incidenteRepositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<ResolverIncidenteSeguridadCommand>
{
    public async Task Handle(ResolverIncidenteSeguridadCommand request, CancellationToken cancellationToken)
    {
        var incidente = await incidenteRepositorio.ObtenerPorIdAsync(new IncidenteSeguridadId(request.IncidenteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el incidente de seguridad {request.IncidenteId}.");

        incidente.Resolver(request.Resolucion);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
