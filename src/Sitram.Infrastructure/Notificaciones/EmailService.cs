using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Infrastructure.Notificaciones;

/// <summary>
/// Envía correos vía SMTP (MailKit) si <c>Smtp:Host</c> está configurado (User Secrets/entorno).
/// Si no lo está —caso por defecto en desarrollo y pruebas—, registra el envío simulado en el
/// log, enmascarando el destinatario (RNF-010: ningún dato personal en logs).
/// </summary>
public sealed class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task EnviarAsync(string destino, string asunto, string cuerpo, CancellationToken cancellationToken = default)
    {
        var host = configuration["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            // Sin SMTP el cuerpo TIENE que ir al log: contiene el código MFA (RF-005) y los
            // enlaces de verificación/recuperación (RF-001/RF-004). Si solo se registrara el
            // asunto, esos flujos serían imposibles de completar en desarrollo (un funcionario
            // con MFA no podría iniciar sesión jamás). Esta rama nunca corre en producción,
            // donde Smtp:Host viene del entorno.
            logger.LogInformation(
                "Correo simulado (SMTP no configurado) para {Destinatario}: {Asunto}\n--- CUERPO (solo visible en desarrollo) ---\n{Cuerpo}",
                EnmascararCorreo(destino), asunto, cuerpo);
            return;
        }

        var mensaje = new MimeMessage();
        mensaje.From.Add(MailboxAddress.Parse(configuration["Smtp:From"] ?? "no-responder@sitram.local"));
        mensaje.To.Add(MailboxAddress.Parse(destino));
        mensaje.Subject = asunto;
        mensaje.Body = new TextPart("plain") { Text = cuerpo };

        using var cliente = new SmtpClient();
        var puerto = int.Parse(configuration["Smtp:Port"] ?? "587");
        await cliente.ConnectAsync(host, puerto, SecureSocketOptions.StartTls, cancellationToken);

        var usuario = configuration["Smtp:UserName"];
        if (!string.IsNullOrWhiteSpace(usuario))
            await cliente.AuthenticateAsync(usuario, configuration["Smtp:Password"] ?? string.Empty, cancellationToken);

        await cliente.SendAsync(mensaje, cancellationToken);
        await cliente.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Correo enviado a {Destinatario}: {Asunto}", EnmascararCorreo(destino), asunto);
    }

    private static string EnmascararCorreo(string correo)
    {
        var posicionArroba = correo.IndexOf('@');
        return posicionArroba <= 1 ? "***" : $"{correo[..2]}***{correo[posicionArroba..]}";
    }
}
