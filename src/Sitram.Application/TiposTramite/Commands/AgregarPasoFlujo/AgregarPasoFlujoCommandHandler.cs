using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.TiposTramite;

namespace Sitram.Application.TiposTramite.Commands.AgregarPasoFlujo;

public sealed class AgregarPasoFlujoCommandHandler(ITipoTramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<AgregarPasoFlujoCommand>
{
    public async Task Handle(AgregarPasoFlujoCommand request, CancellationToken cancellationToken)
    {
        var tipoTramite = await repositorio.ObtenerPorIdAsync(request.TipoTramiteId, cancellationToken)
            ?? throw new TipoTramiteNoDisponibleException(request.TipoTramiteId);

        tipoTramite.AgregarPasoFlujo(request.Orden, request.RolResponsableId);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
