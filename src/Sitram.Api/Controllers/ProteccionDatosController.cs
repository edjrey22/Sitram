using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Seguridad.Commands.DesignarOficialDatos;

namespace Sitram.Api.Controllers;

/// <summary>Designación del Oficial de Datos Personales (RF-066).</summary>
[ApiController]
[Route("api/proteccion-datos")]
[Authorize(Policy = "AdministracionGestionar")]
public sealed class ProteccionDatosController(ISender sender) : ControllerBase
{
    /// <summary>Designa a un usuario como el Oficial de Datos Personales, con acceso a incidentes y solicitudes ARCO.</summary>
    [HttpPost("oficial-datos")]
    public async Task<IActionResult> DesignarOficialDatos([FromBody] DesignarOficialDatosRequest request, CancellationToken ct)
    {
        await sender.Send(new DesignarOficialDatosCommand(request.UsuarioId), ct);
        return NoContent();
    }
}

public sealed record DesignarOficialDatosRequest(Guid UsuarioId);
