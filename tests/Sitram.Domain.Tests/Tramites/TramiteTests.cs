using FluentAssertions;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Tramites;
using Sitram.Domain.Tramites.Events;

namespace Sitram.Domain.Tests.Tramites;

public class TramiteTests
{
    private static Tramite CrearTramiteValido() =>
        Tramite.Crear(ciudadanoId: Guid.NewGuid(), tipoTramiteId: 1, codigo: "TRA-2026-0001");

    // ---------- Creación ----------

    [Fact]
    public void Crear_ConDatosValidos_QuedaEnBorrador()
    {
        // Arrange & Act
        var tramite = CrearTramiteValido();

        // Assert
        tramite.Estado.Should().Be(EstadoTramite.Borrador);
        tramite.Historial.Should().BeEmpty();
        tramite.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Crear_SinCiudadano_LanzaDomainException()
    {
        // Arrange
        var act = () => Tramite.Crear(Guid.Empty, tipoTramiteId: 1, codigo: "TRA-2026-0001");

        // Act & Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Crear_SinCodigo_LanzaArgumentException()
    {
        var act = () => Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, codigo: "   ");

        act.Should().Throw<ArgumentException>();
    }

    // ---------- Transiciones válidas ----------

    [Fact]
    public void Enviar_DesdeBorrador_CambiaARecibido()
    {
        var tramite = CrearTramiteValido();

        tramite.Enviar();

        tramite.Estado.Should().Be(EstadoTramite.Recibido);
        tramite.Historial.Should().ContainSingle()
            .Which.EstadoNuevo.Should().Be(EstadoTramite.Recibido);
    }

    [Fact]
    public void FlujoCompleto_HastaAprobado_RegistraHistorialYEvento()
    {
        // Arrange
        var tramite = CrearTramiteValido();

        // Act: Borrador -> Recibido -> EnRevision -> Aprobado
        tramite.Enviar();
        tramite.IniciarRevision();
        tramite.Aprobar();

        // Assert
        tramite.Estado.Should().Be(EstadoTramite.Aprobado);
        tramite.Historial.Should().HaveCount(3);
        tramite.DomainEvents.Should().ContainSingle(e => e is TramiteAprobadoEvent);
    }

    [Fact]
    public void Observar_EnRevision_CambiaAObservadoYEmiteEvento()
    {
        var tramite = CrearTramiteValido();
        tramite.Enviar();
        tramite.IniciarRevision();

        tramite.Observar("Falta el recibo de pago.");

        tramite.Estado.Should().Be(EstadoTramite.Observado);
        tramite.DomainEvents.Should().ContainSingle(e => e is TramiteObservadoEvent);
    }

    [Fact]
    public void Subsanar_DesdeObservado_VuelveAEnRevision()
    {
        var tramite = CrearTramiteValido();
        tramite.Enviar();
        tramite.IniciarRevision();
        tramite.Observar("Corrige el formulario.");

        tramite.Subsanar();

        tramite.Estado.Should().Be(EstadoTramite.EnRevision);
    }

    // ---------- Transiciones inválidas (RF-029) ----------

    [Fact]
    public void Aprobar_TramiteEnBorrador_LanzaTransicionInvalidaException()
    {
        var tramite = CrearTramiteValido();

        var act = () => tramite.Aprobar();

        act.Should().Throw<TransicionInvalidaException>();
        tramite.Estado.Should().Be(EstadoTramite.Borrador);
    }

    [Fact]
    public void Enviar_DosVeces_LanzaTransicionInvalidaException()
    {
        var tramite = CrearTramiteValido();
        tramite.Enviar();

        var act = () => tramite.Enviar();

        act.Should().Throw<TransicionInvalidaException>();
    }

    [Fact]
    public void Rechazar_SinMotivo_LanzaArgumentException()
    {
        var tramite = CrearTramiteValido();
        tramite.Enviar();
        tramite.IniciarRevision();

        var act = () => tramite.Rechazar("");

        act.Should().Throw<ArgumentException>();
    }
}
