using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        // public DbSet<Product> Products { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<PedidoProveedor> PedidosProveedor { get; set; }
        public DbSet<ProductoPedido> ProductosPedido { get; set; }
        public DbSet<CantidadPedido> CantidadesPedido { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Tarima> Tarimas { get; set; }
        public DbSet<Clasificacion> Clasificaciones { get; set; }
        public DbSet<TarimaClasificacion> TarimaClasificaciones { get; set; }
        public DbSet<CajaCliente> CajaClientes { get; set; }
        public DbSet<PedidoCliente> PedidosCliente { get; set; }
        public DbSet<Merma> Mermas { get; set; }
        public DbSet<Retorno> Retornos { get; set; }
        public DbSet<PedidoTarima> PedidoTarimas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la tabla Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Índices
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();

                // Relaciones
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Profile)
                    .WithOne(e => e.User)
                    .HasForeignKey<UserProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la tabla Roles
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Índices
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuración de la tabla Permissions
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Module).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();

                // Índices
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuración de la tabla RolePermissions
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });
                entity.Property(e => e.CreatedAt).IsRequired();

                // Relaciones
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la tabla UserProfiles
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.Gender).HasMaxLength(20);
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            // Configuración de la tabla Proveedores
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RFC).HasMaxLength(13);
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.DireccionFiscal).HasMaxLength(200);
                entity.Property(e => e.SituacionFiscal).HasMaxLength(250);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);
            });

            // Configuración de la tabla Productos
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Variedad).HasMaxLength(100);
                entity.Property(e => e.UnidadMedida).HasMaxLength(20);
                entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.Imagen).HasMaxLength(250);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).HasMaxLength(50);
                entity.Property(e => e.UsuarioModificacion).HasMaxLength(50).IsRequired(false);
            });

            // Configuración de la tabla PedidoProveedor
            modelBuilder.Entity<PedidoProveedor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasMaxLength(300);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.FechaEstimada).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UsuarioRecepcion).HasMaxLength(50);

                entity.HasOne(e => e.Proveedor)
                    .WithMany(p => p.PedidosProveedor)
                    .HasForeignKey(e => e.IdProveedor);
            });

            // Configuración de la tabla ProductoPedido (muchos a muchos)
            modelBuilder.Entity<ProductoPedido>(entity =>
            {
                entity.HasKey(e => new { e.IdPedidoProveedor, e.IdProducto });

                entity.HasOne(pp => pp.PedidoProveedor)
                    .WithMany(p => p.ProductosPedido)
                    .HasForeignKey(pp => pp.IdPedidoProveedor);

                entity.HasOne(pp => pp.Producto)
                    .WithMany(p => p.ProductosPedido)
                    .HasForeignKey(pp => pp.IdProducto);
            });

            // Configuración de la tabla CantidadPedido
            modelBuilder.Entity<CantidadPedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CantidadCajas).IsRequired();
                entity.Property(e => e.PesoPorCaja).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.PesoBruto).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.PesoTara).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PesoTarima).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PesoPatin).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PesoNeto).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.PedidoProveedor)
                    .WithMany(p => p.CantidadesPedido)
                    .HasForeignKey(e => e.IdPedidoProveedor);
            });

            // Configuración de la tabla Clientes
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RazonSocial).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Rfc).IsRequired().HasMaxLength(13);
                entity.Property(e => e.ConstanciaFiscal).HasMaxLength(250);
                entity.Property(e => e.RepresentanteComercial).HasMaxLength(50);
                entity.Property(e => e.TipoCliente).HasMaxLength(50);
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);
            });

            // Configuración de la tabla Sucursales
            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Direccion).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EncargadoAlmacen).HasMaxLength(50);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);

                entity.HasOne(s => s.Cliente)
                    .WithMany(c => c.Sucursales)
                    .HasForeignKey(s => s.IdCliente);
            });

            // Configuración de la tabla Tarimas
            modelBuilder.Entity<Tarima>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo).IsUnique();
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Estatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.FechaActualizacion);
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UsuarioModificacion).HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasMaxLength(200);
                entity.Property(e => e.UPC).HasMaxLength(50);
                entity.Property(e => e.Peso).HasColumnType("decimal(18,2)");
            });

             // Configuración de la tabla Clasificacion
            modelBuilder.Entity<Clasificacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Lote).IsUnique();
                entity.Property(e => e.Lote).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PesoTotal).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);

                entity.HasOne(c => c.PedidoProveedor)
                    .WithMany(p => p.Clasificaciones)
                    .HasForeignKey(c => c.IdPedidoProveedor);
            });

            // Configuración de la tabla TarimaClasificacion (muchos a muchos)
            modelBuilder.Entity<TarimaClasificacion>(entity =>
            {
                entity.HasKey(tc => new { tc.IdTarima, tc.IdClasificacion });
                entity.Property(tc => tc.Peso).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(tc => tc.Tipo).IsRequired().HasMaxLength(20);
                entity.Property(tc => tc.Cantidad).HasColumnType("decimal(18,2)");
                entity.HasOne(tc => tc.Tarima)
                    .WithMany(t => t.TarimasClasificaciones)
                    .HasForeignKey(tc => tc.IdTarima);
                entity.HasOne(tc => tc.Clasificacion)
                    .WithMany(c => c.TarimasClasificaciones)
                    .HasForeignKey(tc => tc.IdClasificacion);
            });

            // Configuración de la tabla CajaCliente
            modelBuilder.Entity<CajaCliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Peso).IsRequired().HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Precio).IsRequired().HasColumnType("decimal(18, 2)");

                // Relación con Cliente
                entity.HasOne(cc => cc.Cliente)
                    .WithMany(c => c.CajasCliente)
                    .HasForeignKey(cc => cc.IdCliente)
                    .OnDelete(DeleteBehavior.Cascade); // O Restrict, según la lógica de negocio. Cascade parece apropiado aquí.
            });

            // Configuración de la tabla PedidoCliente
            modelBuilder.Entity<PedidoCliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Observaciones).HasMaxLength(300);
                entity.Property(e => e.Estatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FechaEmbarque);
                entity.Property(e => e.FechaModificacion);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UsuarioModificacion).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.Activo).IsRequired();
                entity.HasOne(e => e.Sucursal)
                    .WithMany()
                    .HasForeignKey(e => e.IdSucursal)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Cliente)
                    .WithMany()
                    .HasForeignKey(e => e.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la tabla Merma
            modelBuilder.Entity<Merma>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Peso).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Observaciones).HasMaxLength(300);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Clasificacion)
                    .WithMany(c => c.Mermas)
                    .HasForeignKey(e => e.IdClasificacion);
            });

            // Configuración de la tabla Retorno
            modelBuilder.Entity<Retorno>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Numero).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Peso).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Observaciones).HasMaxLength(300);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistro).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Clasificacion)
                    .WithMany(c => c.RetornosDetalle)
                    .HasForeignKey(e => e.IdClasificacion);
            });

            // Configuración de la tabla PedidoTarima (muchos a muchos)
            modelBuilder.Entity<PedidoTarima>(entity =>
            {
                entity.HasKey(e => new { e.IdPedidoCliente, e.IdTarima });
                entity.HasOne(pt => pt.PedidoCliente)
                    .WithMany(pc => pc.PedidoTarimas)
                    .HasForeignKey(pt => pt.IdPedidoCliente);
                entity.HasOne(pt => pt.Tarima)
                    .WithMany(t => t.PedidoTarimas)
                    .HasForeignKey(pt => pt.IdTarima);
            });
        }
    }
} 