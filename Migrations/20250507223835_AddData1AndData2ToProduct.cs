using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPIEmpacadora.Migrations
{
    /// <inheritdoc />
    public partial class AddData1AndData2ToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Data1",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Data2",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Data2",
                table: "Products");
        }
    }
}
