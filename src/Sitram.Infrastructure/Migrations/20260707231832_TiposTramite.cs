using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sitram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TiposTramite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposTramite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AreaResponsable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tasa = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTramite", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasosFlujo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    RolResponsableId = table.Column<int>(type: "int", nullable: false),
                    TipoTramiteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasosFlujo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasosFlujo_TiposTramite_TipoTramiteId",
                        column: x => x.TipoTramiteId,
                        principalTable: "TiposTramite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequisitosDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Obligatorio = table.Column<bool>(type: "bit", nullable: false),
                    TipoTramiteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosDocumento_TiposTramite_TipoTramiteId",
                        column: x => x.TipoTramiteId,
                        principalTable: "TiposTramite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tramites_TipoTramiteId",
                table: "Tramites",
                column: "TipoTramiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PasosFlujo_TipoTramiteId",
                table: "PasosFlujo",
                column: "TipoTramiteId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosDocumento_TipoTramiteId",
                table: "RequisitosDocumento",
                column: "TipoTramiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tramites_TiposTramite_TipoTramiteId",
                table: "Tramites",
                column: "TipoTramiteId",
                principalTable: "TiposTramite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tramites_TiposTramite_TipoTramiteId",
                table: "Tramites");

            migrationBuilder.DropTable(
                name: "PasosFlujo");

            migrationBuilder.DropTable(
                name: "RequisitosDocumento");

            migrationBuilder.DropTable(
                name: "TiposTramite");

            migrationBuilder.DropIndex(
                name: "IX_Tramites_TipoTramiteId",
                table: "Tramites");
        }
    }
}
