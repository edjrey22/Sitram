using MediatR;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Queries.DescargarDocumento;

public sealed class DescargarDocumentoQueryHandler(ITramiteRepository repositorio, IAlmacenamientoArchivos almacenamiento)
    : IRequestHandler<DescargarDocumentoQuery, DocumentoDescargaDto?>
{
    public async Task<DocumentoDescargaDto?> Handle(DescargarDocumentoQuery request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken);
        var documento = tramite?.Documentos.FirstOrDefault(d => d.Id == request.DocumentoId);
        if (documento is null) return null;

        var contenido = await almacenamiento.AbrirAsync(documento.RutaAlmacenamiento, cancellationToken);
        return new DocumentoDescargaDto(documento.NombreArchivo, contenido);
    }
}
