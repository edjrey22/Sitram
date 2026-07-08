using System.Text.RegularExpressions;
using Sitram.Domain.Common;
using Sitram.Domain.Exceptions;

namespace Sitram.Domain.Ciudadanos;

/// <summary>Value Object: DNI peruano, exactamente 8 dígitos numéricos.</summary>
public sealed partial class Dni : ValueObject
{
    public string Valor { get; }

    public Dni(string valor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valor);
        if (!FormatoDni().IsMatch(valor))
            throw new DomainException("El DNI debe tener exactamente 8 dígitos numéricos.");

        Valor = valor;
    }

    /// <summary>Marcador de DNI anonimizado (derecho al olvido, RF-062): sigue siendo un Dni válido.</summary>
    public static Dni Anonimo() => new("00000000");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor;

    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex FormatoDni();
}
