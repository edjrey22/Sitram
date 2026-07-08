namespace Sitram.Domain.Seguridad;

/// <summary>Identificador del agregado <see cref="IncidenteSeguridad"/>.</summary>
public readonly record struct IncidenteSeguridadId(Guid Value)
{
    public static IncidenteSeguridadId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
