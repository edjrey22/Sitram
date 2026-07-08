namespace Sitram.Domain.Pagos;

/// <summary>Identificador del agregado <see cref="Pago"/>.</summary>
public readonly record struct PagoId(Guid Value)
{
    public static PagoId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
