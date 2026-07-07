using FluentAssertions;
using Sitram.Domain.Tramites;
using Sitram.Domain.Tramites.Events;

namespace Sitram.Domain.Tests.Tramites;

public class TramiteEventosDominioTests
{
    [Fact]
    public void Crear_SiempreEmiteTramiteCreadoEvent()
    {
        // Act
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-0500");

        // Assert
        tramite.DomainEvents.Should().ContainSingle(e => e is TramiteCreadoEvent);
    }

    [Fact]
    public void Enviar_EmiteTramiteEstadoCambiadoEvent_ConEstadosCorrectos()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-0501");
        tramite.ClearDomainEvents(); // descarta el evento de creación para aislar esta transición

        // Act
        tramite.Enviar();

        // Assert
        var evento = tramite.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<TramiteEstadoCambiadoEvent>().Subject;
        evento.EstadoAnterior.Should().Be(EstadoTramite.Borrador);
        evento.EstadoNuevo.Should().Be(EstadoTramite.Recibido);
    }

    [Fact]
    public void Observar_EmiteAmbosEventos_EspecificoYGenerico()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-0502");
        tramite.Enviar();
        tramite.IniciarRevision();
        tramite.ClearDomainEvents();

        // Act
        tramite.Observar("Falta un documento.");

        // Assert: el evento especifico (para notificaciones) y el generico (para auditoria)
        tramite.DomainEvents.Should().Contain(e => e is TramiteObservadoEvent);
        tramite.DomainEvents.Should().Contain(e => e is TramiteEstadoCambiadoEvent);
        tramite.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void TransicionInvalida_NoEmiteEventos()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-0503");
        tramite.ClearDomainEvents();

        // Act
        var act = () => tramite.Aprobar(); // invalido desde Borrador

        // Assert
        act.Should().Throw<Sitram.Domain.Exceptions.TransicionInvalidaException>();
        tramite.DomainEvents.Should().BeEmpty();
    }
}
