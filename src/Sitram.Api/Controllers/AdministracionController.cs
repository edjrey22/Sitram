using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Administracion.Commands.CrearCuentaFuncionario;

namespace Sitram.Api.Controllers;

/// <summary>Gestión de cuentas de funcionario, exclusiva del Administrador.</summary>
[ApiController]
[Route("api/administracion")]
[Authorize(Policy = "AdministracionGestionar")]
public sealed class AdministracionController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Crea una cuenta de funcionario con el rol indicado. El autorregistro público
    /// (<c>POST /api/auth/registro</c>) nunca asigna estos roles: solo produce Ciudadanos.
    /// </summary>
    [HttpPost("funcionarios")]
    public async Task<IActionResult> CrearFuncionario([FromBody] CrearCuentaFuncionarioRequest request, CancellationToken ct)
    {
        var id = await sender.Send(
            new CrearCuentaFuncionarioCommand(request.UserName, request.Email, request.Password, request.Rol), ct);
        return Created(string.Empty, new { id });
    }

    /// <summary>Roles asignables desde <see cref="CrearFuncionario"/> (para poblar el selector de la UI).</summary>
    [HttpGet("roles-funcionario")]
    public IActionResult ListarRolesFuncionario() => Ok(RolesFuncionario.Permitidos);
}

public sealed record CrearCuentaFuncionarioRequest(string UserName, string Email, string Password, string Rol);
