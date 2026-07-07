namespace Sitram.Application.Common.Interfaces;

/// <summary>Confirma en una sola transacción los cambios de un caso de uso (persistencia).</summary>
public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
