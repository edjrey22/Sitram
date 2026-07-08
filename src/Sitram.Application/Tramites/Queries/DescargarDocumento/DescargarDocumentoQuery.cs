using MediatR;

namespace Sitram.Application.Tramites.Queries.DescargarDocumento;

/// <summary>Descarga el contenido de un documento adjunto (RF-021).</summary>
public sealed record DescargarDocumentoQuery(Guid TramiteId, Guid DocumentoId) : IRequest<DocumentoDescargaDto?>;

public sealed record DocumentoDescargaDto(string NombreArchivo, Stream Contenido);
