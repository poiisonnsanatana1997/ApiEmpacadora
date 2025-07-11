using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPIEmpacadora.Migrations
{
    /// <inheritdoc />
    public partial class AddClasificacionSizeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "L",
                table: "Clasificaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "M",
                table: "Clasificaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Retornos",
                table: "Clasificaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "S",
                table: "Clasificaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "XL",
                table: "Clasificaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "L",
                table: "Clasificaciones");

            migrationBuilder.DropColumn(
                name: "M",
                table: "Clasificaciones");

            migrationBuilder.DropColumn(
                name: "Retornos",
                table: "Clasificaciones");

            migrationBuilder.DropColumn(
                name: "S",
                table: "Clasificaciones");

            migrationBuilder.DropColumn(
                name: "XL",
                table: "Clasificaciones");
        }
    }
}
