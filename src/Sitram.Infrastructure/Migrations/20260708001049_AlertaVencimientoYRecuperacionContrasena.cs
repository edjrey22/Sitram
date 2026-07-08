using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sitram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlertaVencimientoYRecuperacionContrasena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertaVencimientoEnviada",
                table: "Tramites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLimiteSubsanacionUtc",
                table: "Tramites",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertaVencimientoEnviada",
                table: "Tramites");

            migrationBuilder.DropColumn(
                name: "FechaLimiteSubsanacionUtc",
                table: "Tramites");
        }
    }
}
