using FluentAssertions;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Tramites;

namespace Sitram.Domain.Tests.Tramites;

public class AlertaVencimientoTests
{
    private static Tramite CrearTramiteObservado()
    {
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-6200");
        tramite.Enviar();
        tramite.IniciarRevision();
        tramite.Observar("Falta un documento.");
        return tramite;
    }

    [Fact]
    public void Observar_FijaFechaLimiteSubsanacion()
    {
        var tramite = CrearTramiteObservado();

        tramite.FechaLimiteSubsanacionUtc.Should().NotBeNull();
        tramite.FechaLimiteSubsanacionUtc.Should().BeCloseTo(DateTime.UtcNow.AddDays(10), TimeSpan.FromMinutes(1));
        tramite.AlertaVencimientoEnviada.Should().BeFalse();
    }

    [Fact]
    public void Subsanar_LimpiaFechaLimiteYAlerta()
    {
        var tramite = CrearTramiteObservado();
        tramite.MarcarAlertaVencimientoEnviada();

        tramite.Subsanar();

        tramite.FechaLimiteSubsanacionUtc.Should().BeNull();
        tramite.AlertaVencimientoEnviada.Should().BeFalse();
    }

    [Fact]
    public void MarcarAlertaVencimientoEnviada_EnEstadoNoObservado_LanzaDomainException()
    {
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-6201");

        var act = () => tramite.MarcarAlertaVencimientoEnviada();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MarcarAlertaVencimientoEnviada_EnObservado_QuedaEnTrue()
    {
        var tramite = CrearTramiteObservado();

        tramite.MarcarAlertaVencimientoEnviada();

        tramite.AlertaVencimientoEnviada.Should().BeTrue();
    }
}
