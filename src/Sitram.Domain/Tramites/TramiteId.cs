namespace Sitram.Domain.Tramites;

/// <summary>Identificador fuertemente tipado del agregado <see cref="Tramite"/>.</summary>
public readonly record struct TramiteId(Guid Value)
{
    public static TramiteId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
