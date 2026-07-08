using FluentAssertions;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Seguridad;
using Sitram.Domain.Seguridad.Events;

namespace Sitram.Domain.Tests.Seguridad;

public class IncidenteSeguridadTests
{
    private static IncidenteSeguridad CrearIncidenteValido() =>
        IncidenteSeguridad.Detectar("Acceso no autorizado", "Se detectaron intentos de acceso a la BD.", GravedadIncidente.Alta);

    [Fact]
    public void Detectar_ConDatosValidos_QuedaEnEstadoDetectado()
    {
        var incidente = CrearIncidenteValido();

        incidente.Estado.Should().Be(EstadoIncidenteSeguridad.Detectado);
        incidente.Gravedad.Should().Be(GravedadIncidente.Alta);
    }

    [Fact]
    public void Detectar_ConTituloVacio_LanzaExcepcion()
    {
        var act = () => IncidenteSeguridad.Detectar("", "Descripción", GravedadIncidente.Baja);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Notificar_DesdeDetectado_CambiaAEstadoNotificadoYEmiteEvento()
    {
        var incidente = CrearIncidenteValido();
        var oficialId = Guid.NewGuid();

        incidente.Notificar(oficialId);

        incidente.Estado.Should().Be(EstadoIncidenteSeguridad.Notificado);
        incidente.OficialNotificadoId.Should().Be(oficialId);
        incidente.FechaNotificacionUtc.Should().NotBeNull();
        incidente.DomainEvents.Should().ContainSingle(e => e is IncidenteSeguridadNotificadoEvent);
    }

    [Fact]
    public void Notificar_SinOficialDesignado_AceptaOficialNulo()
    {
        var incidente = CrearIncidenteValido();

        incidente.Notificar(null);

        incidente.OficialNotificadoId.Should().BeNull();
        incidente.Estado.Should().Be(EstadoIncidenteSeguridad.Notificado);
    }

    [Fact]
    public void Notificar_DosVeces_LanzaDomainException()
    {
        var incidente = CrearIncidenteValido();
        incidente.Notificar(Guid.NewGuid());

        var act = () => incidente.Notificar(Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Resolver_DesdeNotificado_CambiaAEstadoResuelto()
    {
        var incidente = CrearIncidenteValido();
        incidente.Notificar(Guid.NewGuid());

        incidente.Resolver("Se revocaron las credenciales comprometidas.");

        incidente.Estado.Should().Be(EstadoIncidenteSeguridad.Resuelto);
        incidente.Resolucion.Should().NotBeNullOrWhiteSpace();
        incidente.FechaResolucionUtc.Should().NotBeNull();
    }

    [Fact]
    public void Resolver_SinNotificar_LanzaDomainException()
    {
        var incidente = CrearIncidenteValido();

        var act = () => incidente.Resolver("Resolución");

        act.Should().Throw<DomainException>();
    }
}
