using MediatR;

namespace Sitram.Application.Administracion.Commands.CrearCuentaFuncionario;

/// <summary>
/// Crea una cuenta de funcionario con el rol asignado por el Administrador desde el momento de
/// la creación. A diferencia del autorregistro público (RF-001), que siempre produce un
/// Ciudadano, esta vía es exclusiva del Administrador y nunca asigna el rol "Ciudadano".
/// </summary>
public sealed record CrearCuentaFuncionarioCommand(string UserName, string Email, string Password, string Rol)
    : IRequest<Guid>;

/// <summary>Roles que el Administrador puede asignar al crear una cuenta de funcionario.</summary>
public static class RolesFuncionario
{
    public static readonly IReadOnlyList<string> Permitidos =
        ["MesaDePartes", "Revisor", "JefeDeArea", "Administrador", "Auditor", "OficialDatosPersonales"];
}
