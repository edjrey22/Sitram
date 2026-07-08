using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sitram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Documentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RutaAlmacenamiento = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    HashSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SubidoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TramiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TramiteId",
                table: "Documentos",
                column: "TramiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documentos");
        }
    }
}
