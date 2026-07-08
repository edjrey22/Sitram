using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Ciudadanos.Commands.Anonimizar;
using Sitram.Application.Ciudadanos.Commands.OtorgarConsentimiento;
using Sitram.Application.Ciudadanos.Commands.RectificarDatos;
using Sitram.Application.Ciudadanos.Commands.RevocarConsentimiento;
using Sitram.Application.Ciudadanos.Queries.ObtenerPerfilCiudadano;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Api.Controllers;

/// <summary>
/// Derechos ARCO (RF-060…064). Un ciudadano solo puede acceder o modificar <b>su propio</b>
/// perfil: el Id de ruta debe coincidir con el <c>UsuarioId</c> del token (comparten Guid).
/// </summary>
[ApiController]
[Route("api/ciudadanos")]
[Authorize]
public sealed class CiudadanosController(ISender sender, ICurrentUserService currentUser) : ControllerBase
{
    private bool EsPropio(Guid id) => currentUser.UsuarioId == id;

    /// <summary>Consulta el perfil propio (base para el derecho de acceso, RF-060).</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPerfil(Guid id, CancellationToken ct)
    {
        if (!EsPropio(id)) return Forbid();

        var perfil = await sender.Send(new ObtenerPerfilCiudadanoQuery(id), ct);
        return perfil is null ? NotFound() : Ok(perfil);
    }

    /// <summary>Exporta los datos personales en formato interoperable JSON (RF-060, portabilidad).</summary>
    [HttpGet("{id:guid}/exportar")]
    public async Task<IActionResult> Exportar(Guid id, CancellationToken ct)
    {
        if (!EsPropio(id)) return Forbid();

        var perfil = await sender.Send(new ObtenerPerfilCiudadanoQuery(id), ct);
        if (perfil is null) return NotFound();

        return File(
            System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(perfil),
            "application/json", $"sitram-datos-personales-{id}.json");
    }

    /// <summary>Rectifica los datos personales indicados (RF-061).</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Rectificar(Guid id, [FromBody] RectificarRequest request, CancellationToken ct)
    {
        if (!EsPropio(id)) return Forbid();

        await sender.Send(new RectificarDatosCiudadanoCommand(
            id, request.Nombres, request.Apellidos, request.Correo, request.Telefono, request.Direccion), ct);
        return NoContent();
    }

    /// <summary>Derecho al olvido: anonimiza el perfil sin borrar el expediente (RF-062).</summary>
    [HttpPost("{id:guid}/anonimizar")]
    public async Task<IActionResult> Anonimizar(Guid id, CancellationToken ct)
    {
        if (!EsPropio(id)) return Forbid();

        await sender.Send(new AnonimizarCiudadanoCommand(id), ct);
        return NoContent();
    }

    /// <summary>Registra el consentimiento para una finalidad de tratamiento (RF-063).</summary>
    [HttpPost("{id:guid}/consentimientos")]
    public async Task<IActionResult> OtorgarConsentimiento(Guid id, [FromBody] ConsentimientoRequest request, CancellationToken ct)
    {
        if (!EsPropio(id)) return Forbid();

        await sender.Send(new OtorgarConsentimientoCommand(id, request.Finalidad), ct);
        return NoContent();
    }

    /// <summary>Revoca el consentimiento vigente para una finalidad (RF-064).</summary>
    [HttpPost("{id:guid}/consentimientos/revocar")]
    public async Task<IActionResult> RevocarConsentimiento(Guid id, [FromBody] ConsentimientoRequest request, CancellationToken ct)
    {
        if (!EsPropio(id)) return Forbid();

        await sender.Send(new RevocarConsentimientoCommand(id, request.Finalidad), ct);
        return NoContent();
    }
}

public sealed record RectificarRequest(string? Nombres, string? Apellidos, string? Correo, string? Telefono, string? Direccion);
public sealed record ConsentimientoRequest(string Finalidad);
