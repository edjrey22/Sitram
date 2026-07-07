namespace Sitram.Domain.Exceptions;

/// <summary>Se lanza cuando se intenta una transición de estado no permitida en un trámite.</summary>
public sealed class TransicionInvalidaException : DomainException
{
    public TransicionInvalidaException(string estadoActual, string estadoDestino)
        : base($"Transición inválida: no se puede pasar de '{estadoActual}' a '{estadoDestino}'.") { }
}
