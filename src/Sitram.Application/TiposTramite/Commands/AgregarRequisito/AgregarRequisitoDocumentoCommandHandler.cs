using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.TiposTramite;

namespace Sitram.Application.TiposTramite.Commands.AgregarRequisito;

public sealed class AgregarRequisitoDocumentoCommandHandler(ITipoTramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<AgregarRequisitoDocumentoCommand>
{
    public async Task Handle(AgregarRequisitoDocumentoCommand request, CancellationToken cancellationToken)
    {
        var tipoTramite = await repositorio.ObtenerPorIdAsync(request.TipoTramiteId, cancellationToken)
            ?? throw new TipoTramiteNoDisponibleException(request.TipoTramiteId);

        tipoTramite.AgregarRequisito(request.Nombre, request.Obligatorio);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
