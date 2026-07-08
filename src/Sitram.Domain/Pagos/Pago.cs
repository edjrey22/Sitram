using Sitram.Domain.Common;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Pagos.Events;
using Sitram.Domain.Tramites;

namespace Sitram.Domain.Pagos;

/// <summary>
/// Agregado raíz: pago de la tasa de un trámite (RF-040…044). Referencia a <see cref="Tramites.TramiteId"/>
/// solo por su identificador (DDD: los agregados no se referencian por objeto).
/// </summary>
public sealed class Pago : AggregateRoot<PagoId>
{
    public TramiteId TramiteId { get; private set; }
    public decimal Monto { get; private set; }
    public EstadoPago Estado { get; private set; }
    public string? ReferenciaPasarela { get; private set; }
    public DateTime FechaUtc { get; private set; }

    // Requerido por EF Core
    private Pago() { }

    private Pago(PagoId id, TramiteId tramiteId, decimal monto) : base(id)
    {
        TramiteId = tramiteId;
        Monto = monto;
        Estado = EstadoPago.Pendiente;
        FechaUtc = DateTime.UtcNow;
    }

    /// <summary>Registra un pago pendiente por el monto de la tasa (RF-040, RF-041).</summary>
    public static Pago Registrar(TramiteId tramiteId, decimal monto)
    {
        if (monto <= 0)
            throw new DomainException("El monto del pago debe ser mayor a cero.");

        return new Pago(PagoId.New(), tramiteId, monto);
    }

    /// <summary>Asocia la referencia devuelta por la pasarela de pagos (modo prueba).</summary>
    public void AsignarReferenciaPasarela(string referencia)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referencia);
        ReferenciaPasarela = referencia;
    }

    /// <summary>Confirma el pago (RF-042); solo procede desde <see cref="EstadoPago.Pendiente"/>.</summary>
    public void Confirmar()
    {
        if (Estado != EstadoPago.Pendiente)
            throw new DomainException("Solo un pago pendiente puede confirmarse.");

        Estado = EstadoPago.Confirmado;
        RaiseDomainEvent(new PagoConfirmadoEvent(Id, TramiteId));
    }

    /// <summary>Marca el pago como fallido (p. ej. la pasarela rechazó la operación).</summary>
    public void MarcarFallido()
    {
        if (Estado != EstadoPago.Pendiente)
            throw new DomainException("Solo un pago pendiente puede marcarse como fallido.");

        Estado = EstadoPago.Fallido;
    }
}
