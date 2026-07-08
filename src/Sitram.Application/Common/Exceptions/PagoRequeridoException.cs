namespace Sitram.Application.Common.Exceptions;

/// <summary>El trámite tiene una tasa impaga; se impide su avance (RF-043, HTTP 402).</summary>
public sealed class PagoRequeridoException(Guid tramiteId)
    : Exception($"El trámite {tramiteId} tiene una tasa pendiente de pago.");
