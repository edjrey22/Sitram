using MediatR;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Queries.ListarDocumentosTramite;

public sealed class ListarDocumentosTramiteQueryHandler(ITramiteRepository repositorio)
    : IRequestHandler<ListarDocumentosTramiteQuery, IReadOnlyList<DocumentoDto>?>
{
    public async Task<IReadOnlyList<DocumentoDto>?> Handle(ListarDocumentosTramiteQuery request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken);
        return tramite?.Documentos.Select(d => new DocumentoDto(d.Id, d.NombreArchivo, d.HashSha256, d.SubidoUtc)).ToList();
    }
}
