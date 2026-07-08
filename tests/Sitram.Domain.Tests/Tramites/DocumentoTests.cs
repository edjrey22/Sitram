using FluentAssertions;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Tramites;

namespace Sitram.Domain.Tests.Tramites;

public class DocumentoTests
{
    private static Tramite CrearTramiteValido() =>
        Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-6000");

    [Fact]
    public void AdjuntarDocumento_ConExtensionValida_SeAgregaALaLista()
    {
        var tramite = CrearTramiteValido();

        tramite.AdjuntarDocumento("recibo.pdf", "ruta-fisica.pdf", "hash123");

        tramite.Documentos.Should().ContainSingle(d => d.NombreArchivo == "recibo.pdf");
    }

    [Theory]
    [InlineData("virus.exe")]
    [InlineData("documento.docx")]
    public void AdjuntarDocumento_ConExtensionNoPermitida_LanzaDomainException(string nombreArchivo)
    {
        var tramite = CrearTramiteValido();

        var act = () => tramite.AdjuntarDocumento(nombreArchivo, "ruta", "hash");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AdjuntarDocumento_ATramiteAprobado_LanzaDomainException()
    {
        var tramite = CrearTramiteValido();
        tramite.Enviar();
        tramite.IniciarRevision();
        tramite.Aprobar();

        var act = () => tramite.AdjuntarDocumento("recibo.pdf", "ruta", "hash");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ValidarExtensionDocumento_ConExtensionValida_NoLanza()
    {
        var act = () => Tramite.ValidarExtensionDocumento("foto.png");

        act.Should().NotThrow();
    }
}
