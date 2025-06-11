using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAPIEmpacadora.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModeloEmpacadora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Variedad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estatus = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Imagen = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RFC = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Estatus = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DireccionFiscal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SituacionFiscal = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PedidosProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEstimada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsuarioRecepcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosProveedor_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CantidadesPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CantidadCajas = table.Column<int>(type: "int", nullable: false),
                    PesoPorCaja = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PesoBruto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PesoTara = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PesoTarima = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PesoPatin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PesoNeto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IdPedidoProveedor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CantidadesPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CantidadesPedido_PedidosProveedor_IdPedidoProveedor",
                        column: x => x.IdPedidoProveedor,
                        principalTable: "PedidosProveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductosPedido",
                columns: table => new
                {
                    IdPedidoProveedor = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosPedido", x => new { x.IdPedidoProveedor, x.IdProducto });
                    table.ForeignKey(
                        name: "FK_ProductosPedido_PedidosProveedor_IdPedidoProveedor",
                        column: x => x.IdPedidoProveedor,
                        principalTable: "PedidosProveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductosPedido_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CantidadesPedido_IdPedidoProveedor",
                table: "CantidadesPedido",
                column: "IdPedidoProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosProveedor_IdProveedor",
                table: "PedidosProveedor",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosPedido_IdProducto",
                table: "ProductosPedido",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CantidadesPedido");

            migrationBuilder.DropTable(
                name: "ProductosPedido");

            migrationBuilder.DropTable(
                name: "PedidosProveedor");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Data1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PackagingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Variety = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code",
                table: "Products",
                column: "Code",
                unique: true);
        }
    }
}
