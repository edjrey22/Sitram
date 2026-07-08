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
    private readonly List<Documento> _documentos = new();

    private static readonly string[] ExtensionesPermitidas = [".pdf", ".jpg", ".jpeg", ".png"];

    /// <summary>Días de plazo para subsanar una observación antes de que se considere por vencer (RF-053).</summary>
    private const int DiasPlazoSubsanacion = 10;

    public Guid CiudadanoId { get; private set; }
    public int TipoTramiteId { get; private set; }
    public EstadoTramite Estado { get; private set; }
    public string Codigo { get; private set; } = default!;
    public DateTime CreadoUtc { get; private set; }

    /// <summary>Historial inmutable de transiciones de estado del expediente (RF-052).</summary>
    public IReadOnlyCollection<Actuacion> Historial => _historial.AsReadOnly();

    /// <summary>Documentos adjuntos al expediente (RF-021).</summary>
    public IReadOnlyCollection<Documento> Documentos => _documentos.AsReadOnly();

    /// <summary>Plazo límite para subsanar una observación; nulo si no está observado (RF-053).</summary>
    public DateTime? FechaLimiteSubsanacionUtc { get; private set; }

    /// <summary>Evita reenviar la alerta de vencimiento más de una vez (RF-053).</summary>
    public bool AlertaVencimientoEnviada { get; private set; }

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
        FechaLimiteSubsanacionUtc = DateTime.UtcNow.AddDays(DiasPlazoSubsanacion);
        AlertaVencimientoEnviada = false;
        RaiseDomainEvent(new TramiteObservadoEvent(Id, motivo));
    }

    /// <summary>El ciudadano subsana la observación (RF-027): Observado → EnRevision.</summary>
    public void Subsanar()
    {
        CambiarEstado(EstadoTramite.EnRevision, null, EstadoTramite.Observado);
        FechaLimiteSubsanacionUtc = null;
        AlertaVencimientoEnviada = false;
    }

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

    /// <summary>Marca que ya se envió la alerta de vencimiento, para no repetirla (RF-053).</summary>
    public void MarcarAlertaVencimientoEnviada()
    {
        if (Estado != EstadoTramite.Observado)
            throw new DomainException("Solo se puede marcar la alerta de un trámite observado.");

        AlertaVencimientoEnviada = true;
    }

    /// <summary>
    /// Valida que la extensión sea PDF o imagen (RF-021). Se expone para que el llamador pueda
    /// verificarlo <b>antes</b> de guardar el contenido físico y evitar así un archivo huérfano
    /// si el trámite terminara rechazando el adjunto por otro motivo.
    /// </summary>
    public static void ValidarExtensionDocumento(string nombreArchivo)
    {
        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
        if (!ExtensionesPermitidas.Contains(extension))
            throw new DomainException($"Solo se permiten documentos PDF o imagen ({string.Join(", ", ExtensionesPermitidas)}).");
    }

    /// <summary>
    /// Adjunta un documento al expediente (RF-021). No se puede adjuntar a un trámite ya
    /// resuelto (estado terminal).
    /// </summary>
    public void AdjuntarDocumento(string nombreArchivo, string rutaAlmacenamiento, string hashSha256)
    {
        if (Estado is EstadoTramite.Aprobado or EstadoTramite.Rechazado)
            throw new DomainException("No se pueden adjuntar documentos a un trámite ya resuelto.");

        ArgumentException.ThrowIfNullOrWhiteSpace(nombreArchivo);
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaAlmacenamiento);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashSha256);
        ValidarExtensionDocumento(nombreArchivo);

        _documentos.Add(new Documento(nombreArchivo, rutaAlmacenamiento, hashSha256));
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
