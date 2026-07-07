using MediatR;

namespace Sitram.Application.Tramites.Queries.ObtenerEstadoTramite;

/// <summary>Consulta el estado actual y el historial de un trámite (RF-050, RF-052).</summary>
public sealed record ObtenerEstadoTramiteQuery(Guid TramiteId) : IRequest<TramiteEstadoDto?>;

/// <summary>Modelo de lectura del estado de un trámite.</summary>
public sealed record TramiteEstadoDto(
    Guid Id,
    string Codigo,
    string Estado,
    DateTime CreadoUtc,
    IReadOnlyList<ActuacionDto> Historial);

/// <summary>Una transición registrada en el historial del expediente.</summary>
public sealed record ActuacionDto(string EstadoAnterior, string EstadoNuevo, string? Comentario, DateTime FechaUtc);
