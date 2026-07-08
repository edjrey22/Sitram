using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.TiposTramite.Commands.AgregarPasoFlujo;
using Sitram.Application.TiposTramite.Commands.AgregarRequisito;
using Sitram.Application.TiposTramite.Commands.CambiarEstadoTipoTramite;
using Sitram.Application.TiposTramite.Commands.CrearTipoTramite;
using Sitram.Application.TiposTramite.Queries.ObtenerCatalogoTramites;
using Sitram.Application.TiposTramite.Queries.ObtenerTipoTramiteDetalle;

namespace Sitram.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TiposTramiteController(ISender sender) : ControllerBase
{
    /// <summary>Catálogo público de trámites disponibles (RF-014): transparencia del TUPA.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Catalogo(CancellationToken ct)
    {
        var catalogo = await sender.Send(new ObtenerCatalogoTramitesQuery(), ct);
        return Ok(catalogo);
    }

    /// <summary>Detalle público de un tipo de trámite: requisitos y flujo de aprobación.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Detalle(int id, CancellationToken ct)
    {
        var detalle = await sender.Send(new ObtenerTipoTramiteDetalleQuery(id), ct);
        return detalle is null ? NotFound() : Ok(detalle);
    }

    /// <summary>Crea un tipo de trámite nuevo, activo por defecto (RF-010).</summary>
    [HttpPost]
    [Authorize(Policy = "AdministracionGestionar")]
    public async Task<IActionResult> Crear([FromBody] CrearTipoTramiteRequest request, CancellationToken ct)
    {
        var id = await sender.Send(
            new CrearTipoTramiteCommand(request.Nombre, request.Descripcion, request.AreaResponsable, request.Tasa), ct);
        return CreatedAtAction(nameof(Detalle), new { id }, new { id });
    }

    /// <summary>Añade un documento requerido al tipo de trámite (RF-011).</summary>
    [HttpPost("{id:int}/requisitos")]
    [Authorize(Policy = "AdministracionGestionar")]
    public async Task<IActionResult> AgregarRequisito(int id, [FromBody] RequisitoRequest request, CancellationToken ct)
    {
        await sender.Send(new AgregarRequisitoDocumentoCommand(id, request.Nombre, request.Obligatorio), ct);
        return NoContent();
    }

    /// <summary>Añade un paso al flujo de aprobación del tipo de trámite (RF-012).</summary>
    [HttpPost("{id:int}/pasos")]
    [Authorize(Policy = "AdministracionGestionar")]
    public async Task<IActionResult> AgregarPaso(int id, [FromBody] PasoFlujoRequest request, CancellationToken ct)
    {
        await sender.Send(new AgregarPasoFlujoCommand(id, request.Orden, request.RolResponsableId), ct);
        return NoContent();
    }

    /// <summary>Activa el tipo de trámite (RF-013).</summary>
    [HttpPost("{id:int}/activar")]
    [Authorize(Policy = "AdministracionGestionar")]
    public async Task<IActionResult> Activar(int id, CancellationToken ct)
    {
        await sender.Send(new CambiarEstadoTipoTramiteCommand(id, Activar: true), ct);
        return NoContent();
    }

    /// <summary>Desactiva el tipo de trámite sin borrarlo (RF-013, borrado lógico).</summary>
    [HttpPost("{id:int}/desactivar")]
    [Authorize(Policy = "AdministracionGestionar")]
    public async Task<IActionResult> Desactivar(int id, CancellationToken ct)
    {
        await sender.Send(new CambiarEstadoTipoTramiteCommand(id, Activar: false), ct);
        return NoContent();
    }
}

public sealed record CrearTipoTramiteRequest(string Nombre, string Descripcion, string AreaResponsable, decimal Tasa);
public sealed record RequisitoRequest(string Nombre, bool Obligatorio);
public sealed record PasoFlujoRequest(int Orden, int RolResponsableId);
