using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Auditoria.Queries.ConsultarAuditoria;

namespace Sitram.Api.Controllers;

/// <summary>Vista transversal del registro de auditoría, solo lectura, para el Auditor (RF-071).</summary>
[ApiController]
[Route("api/auditoria")]
[Authorize(Policy = "AuditoriaLeer")]
public sealed class AuditoriaController(ISender sender) : ControllerBase
{
    /// <summary>Consulta el registro de auditoría filtrado por usuario, acción y/o rango de fechas.</summary>
    [HttpGet]
    public async Task<IActionResult> Consultar(
        [FromQuery] Guid? usuarioId, [FromQuery] string? accion, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var resultado = await sender.Send(new ConsultarAuditoriaQuery(usuarioId, accion, desde, hasta, page, pageSize), ct);
        return Ok(resultado);
    }
}
