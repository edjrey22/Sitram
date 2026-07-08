using Sitram.Domain.Common;

namespace Sitram.Domain.Ciudadanos;

/// <summary>Consentimiento del titular para una finalidad de tratamiento (RF-063, RF-064).</summary>
public sealed class Consentimiento : Entity<Guid>
{
    public string Finalidad { get; private set; } = default!;
    public bool Otorgado { get; private set; }
    public DateTime FechaUtc { get; private set; }
    public DateTime? RevocadoUtc { get; private set; }

    internal static Consentimiento Otorgar(string finalidad) => new(finalidad);

    private Consentimiento(string finalidad) : base(Guid.NewGuid())
    {
        Finalidad = finalidad;
        Otorgado = true;
        FechaUtc = DateTime.UtcNow;
    }

    // Requerido por EF Core
    private Consentimiento() { }

    internal void Revocar()
    {
        Otorgado = false;
        RevocadoUtc = DateTime.UtcNow;
    }
}
