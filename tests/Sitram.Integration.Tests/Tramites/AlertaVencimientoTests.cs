using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.EnviarAlertaVencimiento;
using Sitram.Domain.Tramites;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Integration.Tests.Tramites;

/// <summary>
/// RF-053: como el job (<c>AlertaVencimientoBackgroundService</c>) se omite en el entorno de
/// pruebas, se verifica su lógica invocando directamente la consulta y el comando que dispara.
/// </summary>
[Collection("Integración")]
public class AlertaVencimientoTests(SitramWebFactory factory)
{
    [Fact]
    public async Task ListarProximosAVencer_IncluyeSoloElTramiteConPlazoVencido_YEnviaLaAlerta()
    {
        // Arrange: un trámite Observado cuyo plazo ya "venció" (se fuerza la fecha en el pasado)
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var (ciudadano, ciudadanoId) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");
        var revisor = await factory.CrearClienteAutenticadoAsync("Revisor");
        var mesaDePartes = await factory.CrearClienteAutenticadoAsync("MesaDePartes");

        // El CiudadanoId del trámite debe ser el del ciudadano REAL registrado (no uno al azar),
        // para que el handler de la alerta encuentre su perfil y correo al enviarla.
        var codigo = $"TRA-2026-{Guid.NewGuid():N}"[..20];
        var creacion = await ciudadano.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId, tipoTramiteId = 1, codigo });
        var tramite = (await creacion.Content.ReadFromJsonAsync<TramiteCreadoResponse>())!;
        await ciudadano.PostAsync($"/api/tramites/{tramite.Id}/enviar", null);
        await mesaDePartes.PostAsync($"/api/tramites/{tramite.Id}/revision", null);
        await revisor.PostAsJsonAsync($"/api/tramites/{tramite.Id}/observar", new { motivo = "Falta un anexo." });

        // Simula el paso del tiempo: el plazo de subsanación ya venció
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Tramites SET FechaLimiteSubsanacionUtc = {DateTime.UtcNow.AddDays(-1)} WHERE Id = {tramite.Id}");
        }

        // Act: se invoca directamente lo que el job ejecutaría en su siguiente tick
        using (var scope = factory.Services.CreateScope())
        {
            var readService = scope.ServiceProvider.GetRequiredService<ITramitesReadService>();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var proximosAVencer = await readService.ListarProximosAVencerAsync(diasAnticipacion: 2);
            proximosAVencer.Should().Contain(tramite.Id);

            foreach (var id in proximosAVencer)
                await sender.Send(new EnviarAlertaVencimientoCommand(id));
        }

        // Assert: se envió el correo y quedó marcado para no repetirse
        fakeEmail.Enviados.Should().Contain(e => e.Asunto.Contains(codigo) && e.Asunto.Contains("por vencer"));

        using var verificacion = factory.Services.CreateScope();
        var dbVerificacion = verificacion.ServiceProvider.GetRequiredService<SitramDbContext>();
        var tramiteActualizado = await dbVerificacion.Tramites.AsNoTracking()
            .FirstAsync(t => t.Id == new TramiteId(tramite.Id));
        tramiteActualizado.AlertaVencimientoEnviada.Should().BeTrue();
    }

    private sealed record TramiteCreadoResponse(Guid Id);
}
