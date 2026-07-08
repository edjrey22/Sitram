using MediatR;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Seguridad;

namespace Sitram.Application.Seguridad.Commands.RegistrarIncidenteSeguridad;

public sealed class RegistrarIncidenteSeguridadCommandHandler(
    IIncidenteSeguridadRepository incidenteRepositorio, IIdentityService identityService, IUnitOfWork unitOfWork)
    : IRequestHandler<RegistrarIncidenteSeguridadCommand, RegistrarIncidenteSeguridadResultado>
{
    public async Task<RegistrarIncidenteSeguridadResultado> Handle(
        RegistrarIncidenteSeguridadCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<GravedadIncidente>(request.Gravedad, ignoreCase: true, out var gravedad))
            throw new DomainException($"Gravedad de incidente inválida: '{request.Gravedad}'.");

        var incidente = IncidenteSeguridad.Detectar(request.Titulo, request.Descripcion, gravedad);

        // RF-065: el sistema notifica de inmediato al Oficial de Datos Personales designado (RF-066).
        var oficial = await identityService.ObtenerOficialDatosActivoAsync(cancellationToken);
        incidente.Notificar(oficial?.Id);

        await incidenteRepositorio.AddAsync(incidente, cancellationToken);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new RegistrarIncidenteSeguridadResultado(incidente.Id.Value, incidente.Estado.ToString(), oficial is not null);
    }
}
