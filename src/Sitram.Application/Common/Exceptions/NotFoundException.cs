namespace Sitram.Application.Common.Exceptions;

/// <summary>El recurso solicitado no existe (se traduce a HTTP 404).</summary>
public sealed class NotFoundException(string message) : Exception(message);
