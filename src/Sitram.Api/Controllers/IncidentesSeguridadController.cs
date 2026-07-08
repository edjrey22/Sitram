using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Seguridad.Commands.RegistrarIncidenteSeguridad;
using Sitram.Application.Seguridad.Commands.ResolverIncidenteSeguridad;
using Sitram.Application.Seguridad.Queries.ListarIncidentesSeguridad;
using Sitram.Application.Seguridad.Queries.ObtenerIncidenteSeguridad;

namespace Sitram.Api.Controllers;

/// <summary>
/// Incidentes de seguridad / brechas de datos personales (RF-065, D.S. 016-2024-JUS). Registrar
/// un incidente lo notifica de inmediato al Oficial de Datos Personales activo (RF-066) y deja
/// constancia en auditoría.
/// </summary>
[ApiController]
[Route("api/incidentes-seguridad")]
[Authorize]
public sealed class IncidentesSeguridadController(ISender sender) : ControllerBase
{
    /// <summary>Registra la detección de un incidente de seguridad.</summary>
    [HttpPost]
    [Authorize(Policy = "AdministracionGestionar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarIncidenteSeguridadRequest request, CancellationToken ct)
    {
        var resultado = await sender.Send(
            new RegistrarIncidenteSeguridadCommand(request.Titulo, request.Descripcion, request.Gravedad), ct);
        return CreatedAtAction(nameof(Obtener), new { id = resultado.IncidenteId }, resultado);
    }

    /// <summary>Lista los incidentes de seguridad, para el Oficial de Datos Personales.</summary>
    [HttpGet]
    [Authorize(Policy = "DatosArco")]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await sender.Send(new ListarIncidentesSeguridadQuery(), ct));

    /// <summary>Consulta el detalle de un incidente de seguridad.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "DatosArco")]
    public async Task<IActionResult> Obtener(Guid id, CancellationToken ct)
    {
        var incidente = await sender.Send(new ObtenerIncidenteSeguridadQuery(id), ct);
        return incidente is null ? NotFound() : Ok(incidente);
    }

    /// <summary>Cierra un incidente notificado con la resolución aplicada.</summary>
    [HttpPost("{id:guid}/resolver")]
    [Authorize(Policy = "DatosArco")]
    public async Task<IActionResult> Resolver(Guid id, [FromBody] ResolverIncidenteSeguridadRequest request, CancellationToken ct)
    {
        await sender.Send(new ResolverIncidenteSeguridadCommand(id, request.Resolucion), ct);
        return NoContent();
    }
}

public sealed record RegistrarIncidenteSeguridadRequest(string Titulo, string Descripcion, string Gravedad);
public sealed record ResolverIncidenteSeguridadRequest(string Resolucion);
