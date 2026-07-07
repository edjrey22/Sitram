using Sitram.Domain.Common;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Tramites.Events;

namespace Sitram.Domain.Tramites;

/// <summary>
/// Agregado raíz: expediente de un trámite municipal. Encapsula la máquina de estados y
/// garantiza que <b>ninguna transición inválida</b> ocurra desde fuera del agregado (RF-029).
/// Solo sus métodos cambian el estado; el <c>setter</c> es privado (errores-conocidos 1.2).
/// </summary>
public sealed class Tramite : AggregateRoot<TramiteId>
{
    private readonly List<Actuacion> _historial = new();

    public Guid CiudadanoId { get; private set; }
    public int TipoTramiteId { get; private set; }
    public EstadoTramite Estado { get; private set; }
    public string Codigo { get; private set; } = default!;
    public DateTime CreadoUtc { get; private set; }

    /// <summary>Historial inmutable de transiciones de estado del expediente (RF-052).</summary>
    public IReadOnlyCollection<Actuacion> Historial => _historial.AsReadOnly();

    // Requerido por EF Core
    private Tramite() { }

    private Tramite(TramiteId id, Guid ciudadanoId, int tipoTramiteId, string codigo) : base(id)
    {
        CiudadanoId = ciudadanoId;
        TipoTramiteId = tipoTramiteId;
        Codigo = codigo;
        Estado = EstadoTramite.Borrador;
        CreadoUtc = DateTime.UtcNow;
        RaiseDomainEvent(new TramiteCreadoEvent(Id));
    }

    /// <summary>Crea un trámite nuevo en estado <see cref="EstadoTramite.Borrador"/> (RF-020).</summary>
    public static Tramite Crear(Guid ciudadanoId, int tipoTramiteId, string codigo)
    {
        if (ciudadanoId == Guid.Empty)
            throw new DomainException("El ciudadano del trámite es obligatorio.");
        if (tipoTramiteId <= 0)
            throw new DomainException("El tipo de trámite es obligatorio.");
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo);

        return new Tramite(TramiteId.New(), ciudadanoId, tipoTramiteId, codigo.Trim());
    }

    /// <summary>El ciudadano envía el trámite (RF-023): Borrador → Recibido.</summary>
    public void Enviar() =>
        CambiarEstado(EstadoTramite.Recibido, null, EstadoTramite.Borrador);

    /// <summary>Mesa de Partes admite el expediente (RF-024): Recibido → EnRevision.</summary>
    public void IniciarRevision() =>
        CambiarEstado(EstadoTramite.EnRevision, null, EstadoTramite.Recibido);

    /// <summary>El revisor observa el expediente (RF-026): EnRevision → Observado.</summary>
    public void Observar(string motivo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
        CambiarEstado(EstadoTramite.Observado, motivo, EstadoTramite.EnRevision);
        RaiseDomainEvent(new TramiteObservadoEvent(Id, motivo));
    }

    /// <summary>El ciudadano subsana la observación (RF-027): Observado → EnRevision.</summary>
    public void Subsanar() =>
        CambiarEstado(EstadoTramite.EnRevision, null, EstadoTramite.Observado);

    /// <summary>El jefe de área aprueba el trámite (RF-028): EnRevision → Aprobado.</summary>
    public void Aprobar()
    {
        CambiarEstado(EstadoTramite.Aprobado, null, EstadoTramite.EnRevision);
        RaiseDomainEvent(new TramiteAprobadoEvent(Id));
    }

    /// <summary>El jefe de área rechaza el trámite (RF-028): EnRevision → Rechazado.</summary>
    public void Rechazar(string motivo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
        CambiarEstado(EstadoTramite.Rechazado, motivo, EstadoTramite.EnRevision);
        RaiseDomainEvent(new TramiteRechazadoEvent(Id, motivo));
    }

    /// <summary>
    /// Única vía de cambio de estado: valida que el estado actual esté entre los permitidos,
    /// registra la actuación y aplica la transición. Lanza <see cref="TransicionInvalidaException"/>
    /// si la transición no procede.
    /// </summary>
    private void CambiarEstado(EstadoTramite nuevo, string? comentario, params EstadoTramite[] estadosPermitidos)
    {
        if (!estadosPermitidos.Contains(Estado))
            throw new TransicionInvalidaException(Estado.ToString(), nuevo.ToString());

        var estadoAnterior = Estado;
        _historial.Add(new Actuacion(estadoAnterior, nuevo, comentario));
        Estado = nuevo;
        RaiseDomainEvent(new TramiteEstadoCambiadoEvent(Id, estadoAnterior, nuevo, comentario));
    }
}
