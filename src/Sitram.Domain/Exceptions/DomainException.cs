namespace Sitram.Domain.Exceptions;

/// <summary>Excepción base para violaciones de invariantes del dominio.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
