namespace Sitram.Application.Common.Exceptions;

/// <summary>El tipo de trámite referenciado no existe o está desactivado (HTTP 400).</summary>
public sealed class TipoTramiteNoDisponibleException(int tipoTramiteId)
    : Exception($"El tipo de trámite {tipoTramiteId} no existe o no está activo.");
