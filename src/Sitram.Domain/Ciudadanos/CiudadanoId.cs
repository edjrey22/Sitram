namespace Sitram.Domain.Ciudadanos;

/// <summary>
/// Identificador del agregado <see cref="Ciudadano"/>. Comparte valor con el <c>UsuarioId</c>
/// de Identity (relación 1:1 CIUDADANO–USUARIO de modelo-datos.md): no se genera aparte, se
/// construye a partir del Guid del usuario ya creado.
/// </summary>
public readonly record struct CiudadanoId(Guid Value)
{
    public override string ToString() => Value.ToString();
}
