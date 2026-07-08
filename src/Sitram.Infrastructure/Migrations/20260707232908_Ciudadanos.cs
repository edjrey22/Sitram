using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sitram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Ciudadanos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ciudadanos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dni = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    Correo = table.Column<byte[]>(type: "varbinary(256)", nullable: false),
                    Telefono = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EstaAnonimizado = table.Column<bool>(type: "bit", nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudadanos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consentimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Finalidad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Otorgado = table.Column<bool>(type: "bit", nullable: false),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevocadoUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CiudadanoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consentimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consentimientos_Ciudadanos_CiudadanoId",
                        column: x => x.CiudadanoId,
                        principalTable: "Ciudadanos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ciudadanos_Dni",
                table: "Ciudadanos",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consentimientos_CiudadanoId",
                table: "Consentimientos",
                column: "CiudadanoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consentimientos");

            migrationBuilder.DropTable(
                name: "Ciudadanos");
        }
    }
}
