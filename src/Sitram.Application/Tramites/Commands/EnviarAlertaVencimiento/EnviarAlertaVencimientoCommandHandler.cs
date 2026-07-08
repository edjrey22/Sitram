using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.EnviarAlertaVencimiento;

public sealed class EnviarAlertaVencimientoCommandHandler(
    ITramiteRepository tramiteRepositorio, ICiudadanoRepository ciudadanoRepositorio,
    IEmailService emailService, IUnitOfWork unitOfWork)
    : IRequestHandler<EnviarAlertaVencimientoCommand>
{
    public async Task Handle(EnviarAlertaVencimientoCommand request, CancellationToken cancellationToken)
    {
        var tramite = await tramiteRepositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        var ciudadano = await ciudadanoRepositorio.ObtenerPorIdAsync(new CiudadanoId(tramite.CiudadanoId), cancellationToken);
        if (ciudadano is not null && !ciudadano.EstaAnonimizado)
        {
            await emailService.EnviarAsync(
                ciudadano.Correo, $"Tu trámite {tramite.Codigo} está por vencer",
                $"Estimado(a) {ciudadano.Nombres}, tu trámite {tramite.Codigo} está observado y vence el " +
                $"{tramite.FechaLimiteSubsanacionUtc:yyyy-MM-dd}. Ingresa a SITRAM para subsanarlo a tiempo.",
                cancellationToken);
        }

        tramite.MarcarAlertaVencimientoEnviada();
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
