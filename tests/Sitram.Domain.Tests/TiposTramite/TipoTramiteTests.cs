using FluentAssertions;
using Sitram.Domain.Exceptions;
using Sitram.Domain.TiposTramite;

namespace Sitram.Domain.Tests.TiposTramite;

public class TipoTramiteTests
{
    [Fact]
    public void Crear_ConDatosValidos_QuedaActivo()
    {
        var tipoTramite = TipoTramite.Crear("Licencia", "Descripción", "Rentas", 100m);

        tipoTramite.Activo.Should().BeTrue();
        tipoTramite.Requisitos.Should().BeEmpty();
        tipoTramite.PasosFlujo.Should().BeEmpty();
    }

    [Fact]
    public void Crear_ConTasaNegativa_LanzaDomainException()
    {
        var act = () => TipoTramite.Crear("Licencia", "Descripción", "Rentas", -10m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AgregarRequisito_AcumulaEnLaLista()
    {
        var tipoTramite = TipoTramite.Crear("Licencia", "Descripción", "Rentas", 100m);

        tipoTramite.AgregarRequisito("DNI", obligatorio: true);
        tipoTramite.AgregarRequisito("Recibo de luz", obligatorio: false);

        tipoTramite.Requisitos.Should().HaveCount(2);
    }

    [Fact]
    public void AgregarPasoFlujo_ConOrdenDuplicado_LanzaDomainException()
    {
        var tipoTramite = TipoTramite.Crear("Licencia", "Descripción", "Rentas", 100m);
        tipoTramite.AgregarPasoFlujo(orden: 1, rolResponsableId: 2);

        var act = () => tipoTramite.AgregarPasoFlujo(orden: 1, rolResponsableId: 3);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Desactivar_YActivar_CambianElEstadoSinBorrar()
    {
        var tipoTramite = TipoTramite.Crear("Licencia", "Descripción", "Rentas", 100m);

        tipoTramite.Desactivar();
        tipoTramite.Activo.Should().BeFalse();

        tipoTramite.Activar();
        tipoTramite.Activo.Should().BeTrue();
    }
}
