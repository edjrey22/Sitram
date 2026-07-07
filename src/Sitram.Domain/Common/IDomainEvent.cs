namespace Sitram.Domain.Common;

/// <summary>Hecho relevante del negocio que ya ocurrió (p. ej. TramiteAprobado).</summary>
/// <remarks>
/// Marcador puro: el dominio no depende de MediatR. La adaptación a un mecanismo de
/// publicación (MediatR, cola, etc.) se hace en Application/Infrastructure.
/// </remarks>
public interface IDomainEvent { }
