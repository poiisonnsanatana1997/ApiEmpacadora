using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPIEmpacadora.Migrations
{
    /// <inheritdoc />
    public partial class MakeNombreAndPrecioNullableInCajaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hacer el campo Nombre nullable
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CajaClientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Hacer el campo Precio nullable
            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "CajaClientes",
                type: "decimal(18, 2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir el campo Nombre a required
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CajaClientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Revertir el campo Precio a required
            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "CajaClientes",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
