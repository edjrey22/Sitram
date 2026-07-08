using MediatR;

namespace Sitram.Application.Tramites.Queries.ListarDocumentosTramite;

/// <summary>Lista los documentos adjuntos de un trámite (RF-021).</summary>
public sealed record ListarDocumentosTramiteQuery(Guid TramiteId) : IRequest<IReadOnlyList<DocumentoDto>?>;

public sealed record DocumentoDto(Guid Id, string NombreArchivo, string HashSha256, DateTime SubidoUtc);
