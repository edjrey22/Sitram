using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.AdjuntarDocumento;

public sealed class AdjuntarDocumentoCommandHandler(
    ITramiteRepository repositorio, IAlmacenamientoArchivos almacenamiento, IUnitOfWork unitOfWork)
    : IRequestHandler<AdjuntarDocumentoCommand, Guid>
{
    public async Task<Guid> Handle(AdjuntarDocumentoCommand request, CancellationToken cancellationToken)
    {
        // Se valida ANTES de guardar en disco: evita un archivo huérfano si la extensión es inválida.
        Tramite.ValidarExtensionDocumento(request.NombreArchivo);

        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        var (ruta, hash) = await almacenamiento.GuardarAsync(request.NombreArchivo, request.Contenido, cancellationToken);

        tramite.AdjuntarDocumento(request.NombreArchivo, ruta, hash);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return tramite.Documentos.Last().Id;
    }
}
