using FluentAssertions;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Pagos;
using Sitram.Domain.Pagos.Events;
using Sitram.Domain.Tramites;

namespace Sitram.Domain.Tests.Pagos;

public class PagoTests
{
    private static Pago CrearPagoValido() => Pago.Registrar(TramiteId.New(), 150m);

    [Fact]
    public void Registrar_ConMontoValido_QuedaPendiente()
    {
        var pago = CrearPagoValido();

        pago.Estado.Should().Be(EstadoPago.Pendiente);
    }

    [Fact]
    public void Registrar_ConMontoCero_LanzaDomainException()
    {
        var act = () => Pago.Registrar(TramiteId.New(), 0m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Confirmar_DesdePendiente_CambiaAConfirmadoYEmiteEvento()
    {
        var pago = CrearPagoValido();

        pago.Confirmar();

        pago.Estado.Should().Be(EstadoPago.Confirmado);
        pago.DomainEvents.Should().ContainSingle(e => e is PagoConfirmadoEvent);
    }

    [Fact]
    public void Confirmar_DosVeces_LanzaDomainException()
    {
        var pago = CrearPagoValido();
        pago.Confirmar();

        var act = () => pago.Confirmar();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MarcarFallido_DesdePendiente_CambiaAFallido()
    {
        var pago = CrearPagoValido();

        pago.MarcarFallido();

        pago.Estado.Should().Be(EstadoPago.Fallido);
    }
}
