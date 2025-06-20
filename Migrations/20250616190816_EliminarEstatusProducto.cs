using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPIEmpacadora.Migrations
{
    /// <inheritdoc />
    public partial class EliminarEstatusProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estatus",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estatus",
                table: "Productos",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }
    }
}
