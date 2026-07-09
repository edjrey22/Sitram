using FluentValidation;
using MediatR;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Administracion.Commands.CrearCuentaFuncionario;

public sealed class CrearCuentaFuncionarioCommandHandler(IIdentityService identityService, IEmailService emailService)
    : IRequestHandler<CrearCuentaFuncionarioCommand, Guid>
{
    public async Task<Guid> Handle(CrearCuentaFuncionarioCommand request, CancellationToken cancellationToken)
    {
        var resultado = await identityService.CrearUsuarioConRolAsync(
            request.UserName, request.Email, request.Password, request.Rol, cancellationToken);

        if (!resultado.Succeeded)
            throw new ValidationException(
                resultado.Errores.Select(e => new FluentValidation.Results.ValidationFailure(nameof(request.Password), e)));

        await emailService.EnviarAsync(
            request.Email, "Se creó tu cuenta de funcionario en SITRAM",
            $"Hola, el Administrador creó tu cuenta con el rol {request.Rol}. Usuario: {request.UserName}. " +
            "Esta cuenta exige verificación en dos pasos (MFA) al iniciar sesión.", cancellationToken);

        return resultado.UsuarioId;
    }
}
