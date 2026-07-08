using MediatR;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.EnviarAlertaVencimiento;

namespace Sitram.Api.Services;

/// <summary>
/// Job periódico (RF-053): revisa los trámites Observado próximos a vencer y dispara su
/// alerta por correo. Se omite en el entorno "Testing" para no interferir con las pruebas
/// de integración (que ejercitan la consulta y el comando directamente, sin depender del timer).
/// </summary>
public sealed class AlertaVencimientoBackgroundService(
    IServiceScopeFactory scopeFactory, IHostEnvironment environment, ILogger<AlertaVencimientoBackgroundService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(6);
    private const int DiasAnticipacion = 2;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (environment.IsEnvironment("Testing"))
            return;

        using var temporizador = new PeriodicTimer(Intervalo);
        do
        {
            try
            {
                await RevisarVencimientosAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al revisar trámites próximos a vencer (RF-053).");
            }
        }
        while (await temporizador.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RevisarVencimientosAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<ITramitesReadService>();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var proximosAVencer = await readService.ListarProximosAVencerAsync(DiasAnticipacion, cancellationToken);
        foreach (var tramiteId in proximosAVencer)
            await sender.Send(new EnviarAlertaVencimientoCommand(tramiteId), cancellationToken);
    }
}
