using FluentValidation;
using MediatR;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Application.Auth.Commands.Registrar;

public sealed class RegistrarCiudadanoCommandHandler(
    IIdentityService identityService, ICiudadanoRepository ciudadanoRepositorio,
    IEmailService emailService, IUnitOfWork unitOfWork)
    : IRequestHandler<RegistrarCiudadanoCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarCiudadanoCommand request, CancellationToken cancellationToken)
    {
        var resultado = await identityService.CrearUsuarioAsync(
            request.UserName, request.Email, request.Password, cancellationToken);

        if (!resultado.Succeeded)
            throw new ValidationException(
                resultado.Errores.Select(e => new FluentValidation.Results.ValidationFailure(nameof(request.Password), e)));

        // El perfil comparte Id con el usuario de Identity (relación 1:1, modelo-datos.md).
        var ciudadano = Ciudadano.Registrar(
            resultado.UsuarioId, request.Nombres, request.Apellidos, request.Dni, request.Email, request.Telefono, request.Direccion);
        await ciudadanoRepositorio.AddAsync(ciudadano, cancellationToken);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        // RF-001: verificación de correo (enlace simulado; no hay frontend real que lo consuma aún).
        var token = await identityService.GenerarTokenConfirmacionEmailAsync(resultado.UsuarioId, cancellationToken);
        var enlace = $"https://sitram.local/confirmar-email?usuarioId={resultado.UsuarioId}&token={Uri.EscapeDataString(token)}";
        await emailService.EnviarAsync(
            request.Email, "Verifica tu correo en SITRAM",
            $"Hola {request.Nombres}, confirma tu correo ingresando a: {enlace}", cancellationToken);

        return resultado.UsuarioId;
    }
}
