using FluentAssertions;
using Sitram.Domain.Ciudadanos;
using Sitram.Domain.Ciudadanos.Events;
using Sitram.Domain.Exceptions;

namespace Sitram.Domain.Tests.Ciudadanos;

public class CiudadanoTests
{
    private static Ciudadano CrearCiudadanoValido() =>
        Ciudadano.Registrar(Guid.NewGuid(), "Ana", "Pérez", "12345678", "ana@correo.com", "987654321", "Av. Perú 123");

    [Fact]
    public void Registrar_ConDatosValidos_QuedaNoAnonimizadoYEmiteEvento()
    {
        var ciudadano = CrearCiudadanoValido();

        ciudadano.EstaAnonimizado.Should().BeFalse();
        ciudadano.DomainEvents.Should().ContainSingle(e => e is CiudadanoRegistradoEvent);
    }

    [Fact]
    public void Registrar_ConDniInvalido_LanzaDomainException()
    {
        var act = () => Ciudadano.Registrar(Guid.NewGuid(), "Ana", "Pérez", "123", "ana@correo.com", "987654321", "Av. Perú 123");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Rectificar_ActualizaSoloLosCamposIndicados()
    {
        var ciudadano = CrearCiudadanoValido();
        ciudadano.ClearDomainEvents();

        ciudadano.Rectificar(nombres: "Ana María", apellidos: null, correo: null, telefono: null, direccion: null);

        ciudadano.Nombres.Should().Be("Ana María");
        ciudadano.Apellidos.Should().Be("Pérez"); // sin cambios
        ciudadano.DomainEvents.Should().ContainSingle(e => e is DatosCiudadanoRectificadosEvent);
    }

    [Fact]
    public void Anonimizar_BorraDatosPersonalesYEsIdempotente()
    {
        var ciudadano = CrearCiudadanoValido();

        ciudadano.Anonimizar();
        ciudadano.EstaAnonimizado.Should().BeTrue();
        ciudadano.Nombres.Should().Be("ANONIMIZADO");
        ciudadano.DomainEvents.Should().Contain(e => e is CiudadanoAnonimizadoEvent);

        // idempotente: una segunda llamada no lanza ni añade otro evento
        var eventosAntes = ciudadano.DomainEvents.Count;
        ciudadano.Anonimizar();
        ciudadano.DomainEvents.Should().HaveCount(eventosAntes);
    }

    [Fact]
    public void Rectificar_SobreCiudadanoAnonimizado_LanzaDomainException()
    {
        var ciudadano = CrearCiudadanoValido();
        ciudadano.Anonimizar();

        var act = () => ciudadano.Rectificar("Nombre", null, null, null, null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void OtorgarYRevocarConsentimiento_FuncionaParaLaMismaFinalidad()
    {
        var ciudadano = CrearCiudadanoValido();

        ciudadano.OtorgarConsentimiento("marketing");
        ciudadano.Consentimientos.Should().ContainSingle(c => c.Finalidad == "marketing" && c.Otorgado);

        ciudadano.RevocarConsentimiento("marketing");
        ciudadano.Consentimientos.Single().Otorgado.Should().BeFalse();
    }

    [Fact]
    public void RevocarConsentimiento_SinConsentimientoPrevio_LanzaDomainException()
    {
        var ciudadano = CrearCiudadanoValido();

        var act = () => ciudadano.RevocarConsentimiento("marketing");

        act.Should().Throw<DomainException>();
    }
}
