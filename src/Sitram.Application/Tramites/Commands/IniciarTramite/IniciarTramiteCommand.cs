using MediatR;

namespace Sitram.Application.Tramites.Commands.IniciarTramite;

/// <summary>Inicia un trámite nuevo en estado Borrador (RF-020). Devuelve el identificador creado.</summary>
public sealed record IniciarTramiteCommand(Guid CiudadanoId, int TipoTramiteId, string Codigo)
    : IRequest<Guid>;
