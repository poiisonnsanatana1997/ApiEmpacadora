using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPIEmpacadora.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTarimaResumenDiario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TarimaResumenDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CantidadTarimas = table.Column<int>(type: "int", nullable: false),
                    CantidadTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PesoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TarimasCompletas = table.Column<int>(type: "int", nullable: false),
                    TarimasParciales = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioUltimaActualizacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarimaResumenDiarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarimaResumenDiarios_Fecha",
                table: "TarimaResumenDiarios",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_TarimaResumenDiarios_Fecha_Tipo",
                table: "TarimaResumenDiarios",
                columns: new[] { "Fecha", "Tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarimaResumenDiarios_Tipo",
                table: "TarimaResumenDiarios",
                column: "Tipo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TarimaResumenDiarios");
        }
    }
}
