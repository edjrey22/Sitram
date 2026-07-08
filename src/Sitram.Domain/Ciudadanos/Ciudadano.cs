using Sitram.Domain.Ciudadanos.Events;
using Sitram.Domain.Common;
using Sitram.Domain.Exceptions;

namespace Sitram.Domain.Ciudadanos;

/// <summary>
/// Agregado raíz: perfil del ciudadano, titular de datos personales (RF-060…066). Comparte Id
/// con el usuario de Identity (relación 1:1). El cifrado de <c>Dni</c>/<c>Correo</c>/<c>Telefono</c>
/// es transparente al dominio: ocurre solo en el <c>ValueConverter</c> de Infrastructure (RNF-003).
/// </summary>
public sealed class Ciudadano : AggregateRoot<CiudadanoId>
{
    private readonly List<Consentimiento> _consentimientos = new();

    public string Nombres { get; private set; } = default!;
    public string Apellidos { get; private set; } = default!;
    public Dni Dni { get; private set; } = default!;
    public string Correo { get; private set; } = default!;
    public string Telefono { get; private set; } = default!;
    public string Direccion { get; private set; } = default!;

    /// <summary>Derecho al olvido (RF-062): una vez anonimizado, no se puede rectificar.</summary>
    public bool EstaAnonimizado { get; private set; }

    public DateTime CreadoUtc { get; private set; }

    public IReadOnlyCollection<Consentimiento> Consentimientos => _consentimientos.AsReadOnly();

    // Requerido por EF Core
    private Ciudadano() { }

    private Ciudadano(CiudadanoId id, string nombres, string apellidos, Dni dni, string correo, string telefono, string direccion)
        : base(id)
    {
        Nombres = nombres;
        Apellidos = apellidos;
        Dni = dni;
        Correo = correo;
        Telefono = telefono;
        Direccion = direccion;
        EstaAnonimizado = false;
        CreadoUtc = DateTime.UtcNow;
        RaiseDomainEvent(new CiudadanoRegistradoEvent(Id));
    }

    /// <summary>Registra el perfil de un ciudadano, ligado al usuario de Identity ya creado.</summary>
    public static Ciudadano Registrar(
        Guid usuarioId, string nombres, string apellidos, string dni, string correo, string telefono, string direccion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombres);
        ArgumentException.ThrowIfNullOrWhiteSpace(apellidos);
        ArgumentException.ThrowIfNullOrWhiteSpace(correo);
        ArgumentException.ThrowIfNullOrWhiteSpace(telefono);
        ArgumentException.ThrowIfNullOrWhiteSpace(direccion);

        return new Ciudadano(
            new CiudadanoId(usuarioId), nombres.Trim(), apellidos.Trim(), new Dni(dni), correo.Trim(), telefono.Trim(), direccion.Trim());
    }

    /// <summary>Rectifica los datos indicados (RF-061); los campos nulos no se modifican.</summary>
    public void Rectificar(string? nombres, string? apellidos, string? correo, string? telefono, string? direccion)
    {
        if (EstaAnonimizado)
            throw new DomainException("No se puede rectificar un perfil anonimizado.");

        if (nombres is not null) Nombres = nombres.Trim();
        if (apellidos is not null) Apellidos = apellidos.Trim();
        if (correo is not null) Correo = correo.Trim();
        if (telefono is not null) Telefono = telefono.Trim();
        if (direccion is not null) Direccion = direccion.Trim();

        RaiseDomainEvent(new DatosCiudadanoRectificadosEvent(Id));
    }

    /// <summary>
    /// Derecho al olvido (RF-062): anonimiza los datos personales de forma irreversible. El
    /// expediente de sus trámites **no se borra** (obligación de archivo municipal).
    /// </summary>
    public void Anonimizar()
    {
        if (EstaAnonimizado) return; // idempotente

        Nombres = "ANONIMIZADO";
        Apellidos = "ANONIMIZADO";
        Dni = Dni.Anonimo();
        Correo = $"anonimizado-{Guid.NewGuid():N}@sitram.local";
        Telefono = "000000000";
        Direccion = "ANONIMIZADO";
        EstaAnonimizado = true;

        RaiseDomainEvent(new CiudadanoAnonimizadoEvent(Id));
    }

    /// <summary>Registra el consentimiento informado para una finalidad de tratamiento (RF-063).</summary>
    public void OtorgarConsentimiento(string finalidad)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(finalidad);
        _consentimientos.Add(Consentimiento.Otorgar(finalidad));
    }

    /// <summary>Revoca el consentimiento vigente para una finalidad (RF-064).</summary>
    public void RevocarConsentimiento(string finalidad)
    {
        var consentimiento = _consentimientos
            .Where(c => c.Finalidad == finalidad && c.Otorgado && c.RevocadoUtc is null)
            .MaxBy(c => c.FechaUtc)
            ?? throw new DomainException($"No existe un consentimiento vigente para la finalidad '{finalidad}'.");

        consentimiento.Revocar();
    }
}
