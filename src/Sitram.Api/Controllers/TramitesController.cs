using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Auditoria.Queries.ObtenerAuditoriaTramite;
using Sitram.Application.Tramites.Commands.AdjuntarDocumento;
using Sitram.Application.Tramites.Commands.IniciarTramite;
using Sitram.Application.Tramites.Commands.Transiciones;
using Sitram.Application.Tramites.Queries.DescargarDocumento;
using Sitram.Application.Tramites.Queries.ListarDocumentosTramite;
using Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;
using Sitram.Application.Tramites.Queries.ObtenerEstadoTramite;
using Sitram.Application.Tramites.Queries.ObtenerReporteTramites;

namespace Sitram.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TramitesController(ISender sender) : ControllerBase
{
    /// <summary>Bandeja del ciudadano: lista paginada de sus trámites (RF-050).</summary>
    [HttpGet]
    [Authorize(Policy = "TramiteConsultar")]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid ciudadanoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await sender.Send(
            new ListarTramitesCiudadanoQuery(ciudadanoId, page, pageSize), cancellationToken);
        return Ok(resultado);
    }

    /// <summary>Reporte de trámites por estado, tipo y periodo (RF-072).</summary>
    [HttpGet("reporte")]
    [Authorize(Policy = "ReportesLeer")]
    public async Task<IActionResult> ObtenerReporte(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct)
    {
        var reporte = await sender.Send(new ObtenerReporteTramitesQuery(desde, hasta), ct);
        return Ok(reporte);
    }

    /// <summary>Inicia un trámite nuevo en estado Borrador (RF-020).</summary>
    [HttpPost]
    [Authorize(Policy = "TramiteIniciar")]
    public async Task<IActionResult> Iniciar([FromBody] IniciarTramiteRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new IniciarTramiteCommand(request.CiudadanoId, request.TipoTramiteId, request.Codigo),
            cancellationToken);

        return CreatedAtAction(nameof(ObtenerEstado), new { id }, new { id });
    }

    /// <summary>Consulta el estado y el historial de un trámite (RF-050, RF-052).</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "TramiteConsultar")]
    public async Task<IActionResult> ObtenerEstado(Guid id, CancellationToken cancellationToken)
    {
        var estado = await sender.Send(new ObtenerEstadoTramiteQuery(id), cancellationToken);
        return estado is null ? NotFound() : Ok(estado);
    }

    /// <summary>Envía el trámite: Borrador → Recibido (RF-023).</summary>
    [HttpPost("{id:guid}/enviar")]
    [Authorize(Policy = "TramiteEnviar")]
    public async Task<IActionResult> Enviar(Guid id, CancellationToken ct)
    {
        await sender.Send(new EnviarTramiteCommand(id), ct);
        return NoContent();
    }

    /// <summary>Mesa de Partes inicia la revisión: Recibido → EnRevision (RF-024).</summary>
    [HttpPost("{id:guid}/revision")]
    [Authorize(Policy = "TramiteRecepcionar")]
    public async Task<IActionResult> IniciarRevision(Guid id, CancellationToken ct)
    {
        await sender.Send(new IniciarRevisionTramiteCommand(id), ct);
        return NoContent();
    }

    /// <summary>El jefe de área aprueba: EnRevision → Aprobado (RF-028).</summary>
    [HttpPost("{id:guid}/aprobar")]
    [Authorize(Policy = "TramiteAprobar")]
    public async Task<IActionResult> Aprobar(Guid id, CancellationToken ct)
    {
        await sender.Send(new AprobarTramiteCommand(id), ct);
        return NoContent();
    }

    /// <summary>El jefe de área rechaza: EnRevision → Rechazado (RF-028).</summary>
    [HttpPost("{id:guid}/rechazar")]
    [Authorize(Policy = "TramiteRechazar")]
    public async Task<IActionResult> Rechazar(Guid id, [FromBody] MotivoRequest body, CancellationToken ct)
    {
        await sender.Send(new RechazarTramiteCommand(id, body.Motivo), ct);
        return NoContent();
    }

    /// <summary>El revisor observa: EnRevision → Observado (RF-026).</summary>
    [HttpPost("{id:guid}/observar")]
    [Authorize(Policy = "TramiteObservar")]
    public async Task<IActionResult> Observar(Guid id, [FromBody] MotivoRequest body, CancellationToken ct)
    {
        await sender.Send(new ObservarTramiteCommand(id, body.Motivo), ct);
        return NoContent();
    }

    /// <summary>El ciudadano subsana: Observado → EnRevision (RF-027).</summary>
    [HttpPost("{id:guid}/subsanar")]
    [Authorize(Policy = "TramiteSubsanar")]
    public async Task<IActionResult> Subsanar(Guid id, CancellationToken ct)
    {
        await sender.Send(new SubsanarTramiteCommand(id), ct);
        return NoContent();
    }

    /// <summary>El auditor consulta el registro de auditoría del trámite (RF-071, solo lectura).</summary>
    [HttpGet("{id:guid}/auditoria")]
    [Authorize(Policy = "AuditoriaLeer")]
    public async Task<IActionResult> ObtenerAuditoria(Guid id, CancellationToken ct)
    {
        var eventos = await sender.Send(new ObtenerAuditoriaTramiteQuery(id), ct);
        return Ok(eventos);
    }

    /// <summary>Adjunta un documento (PDF/imagen) al expediente (RF-021).</summary>
    [HttpPost("{id:guid}/documentos")]
    [Authorize(Policy = "TramiteAdjuntar")]
    [RequestSizeLimit(10_000_000)] // 10 MB (RNF-022)
    public async Task<IActionResult> AdjuntarDocumento(Guid id, IFormFile archivo, CancellationToken ct)
    {
        await using var contenido = archivo.OpenReadStream();
        var documentoId = await sender.Send(new AdjuntarDocumentoCommand(id, archivo.FileName, contenido), ct);
        return CreatedAtAction(nameof(ListarDocumentos), new { id }, new { documentoId });
    }

    /// <summary>Lista los documentos adjuntos del expediente (RF-021).</summary>
    [HttpGet("{id:guid}/documentos")]
    [Authorize(Policy = "TramiteConsultar")]
    public async Task<IActionResult> ListarDocumentos(Guid id, CancellationToken ct)
    {
        var documentos = await sender.Send(new ListarDocumentosTramiteQuery(id), ct);
        return documentos is null ? NotFound() : Ok(documentos);
    }

    /// <summary>Descarga un documento adjunto del expediente.</summary>
    [HttpGet("{id:guid}/documentos/{documentoId:guid}")]
    [Authorize(Policy = "TramiteConsultar")]
    public async Task<IActionResult> DescargarDocumento(Guid id, Guid documentoId, CancellationToken ct)
    {
        var documento = await sender.Send(new DescargarDocumentoQuery(id, documentoId), ct);
        return documento is null ? NotFound() : File(documento.Contenido, "application/octet-stream", documento.NombreArchivo);
    }
}

/// <summary>DTO de entrada del endpoint (evita el <i>over-posting</i>; errores-conocidos 4.3).</summary>
public sealed record IniciarTramiteRequest(Guid CiudadanoId, int TipoTramiteId, string Codigo);

/// <summary>Cuerpo con el motivo para observar o rechazar un trámite.</summary>
public sealed record MotivoRequest(string Motivo);
