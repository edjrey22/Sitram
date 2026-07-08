using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Pagos.Commands.ConfirmarPago;
using Sitram.Application.Pagos.Commands.RegistrarPago;
using Sitram.Application.Pagos.Queries.ObtenerPago;

namespace Sitram.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "TramitePagar")]
public sealed class PagosController(ISender sender) : ControllerBase
{
    /// <summary>Registra el pago de la tasa de un trámite, calculada desde su tipo (RF-040, RF-041).</summary>
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPagoRequest request, CancellationToken ct)
    {
        var resultado = await sender.Send(new RegistrarPagoCommand(request.TramiteId), ct);
        return CreatedAtAction(nameof(ObtenerPago), new { id = resultado.PagoId }, resultado);
    }

    /// <summary>Confirma el pago (simula el callback de la pasarela, modo prueba); avanza el trámite (RF-042, RNF-032).</summary>
    [HttpPost("{id:guid}/confirmar")]
    public async Task<IActionResult> Confirmar(Guid id, CancellationToken ct)
    {
        await sender.Send(new ConfirmarPagoCommand(id), ct);
        return NoContent();
    }

    /// <summary>Consulta el estado de un pago.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPago(Guid id, CancellationToken ct)
    {
        var pago = await sender.Send(new ObtenerPagoQuery(id), ct);
        return pago is null ? NotFound() : Ok(pago);
    }

    /// <summary>Descarga el comprobante del pago, solo si está confirmado (RF-044).</summary>
    [HttpGet("{id:guid}/comprobante")]
    public async Task<IActionResult> Comprobante(Guid id, CancellationToken ct)
    {
        var pago = await sender.Send(new ObtenerPagoQuery(id), ct);
        if (pago is null) return NotFound();
        if (pago.Estado != "Confirmado")
            return BadRequest(new { mensaje = "El comprobante solo está disponible para pagos confirmados." });

        var texto = $"""
            SITRAM — Comprobante de pago
            Pago: {pago.Id}
            Trámite: {pago.TramiteId}
            Monto: S/ {pago.Monto:0.00}
            Referencia pasarela: {pago.ReferenciaPasarela}
            Fecha: {pago.FechaUtc:yyyy-MM-dd HH:mm} UTC
            """;

        return File(Encoding.UTF8.GetBytes(texto), "text/plain", $"comprobante-{id}.txt");
    }
}

public sealed record RegistrarPagoRequest(Guid TramiteId);
