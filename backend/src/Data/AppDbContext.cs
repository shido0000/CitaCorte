using Microsoft.EntityFrameworkCore;
using CitaCorte.Data.Entities;
using CitaCorte.Data.Config;

namespace CitaCorte.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tablas principales
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<PlanSuscripcion> PlanesSuscripcion { get; set; }
    public DbSet<Suscripcion> Suscripciones { get; set; }
    public DbSet<Barbero> Barberos { get; set; }
    public DbSet<Barberia> Barberias { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Servicio> Servicios { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<SolicitudAfiliacion> SolicitudesAfiliacion { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<EstadisticaBarbero> EstadisticasBarbero { get; set; }
    public DbSet<EstadisticaBarberia> EstadisticasBarberia { get; set; }
    public DbSet<EstadisticaReserva> EstadisticasReserva { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones
        modelBuilder.ApplyConfiguration(new UsuarioConfig());
        modelBuilder.ApplyConfiguration(new PlanSuscripcionConfig());
        modelBuilder.ApplyConfiguration(new SuscripcionConfig());
        modelBuilder.ApplyConfiguration(new BarberoConfig());
        modelBuilder.ApplyConfiguration(new BarberiaConfig());
        modelBuilder.ApplyConfiguration(new ClienteConfig());
        modelBuilder.ApplyConfiguration(new ServicioConfig());
        modelBuilder.ApplyConfiguration(new ProductoConfig());
        modelBuilder.ApplyConfiguration(new ReservaConfig());
        modelBuilder.ApplyConfiguration(new SolicitudAfiliacionConfig());
        modelBuilder.ApplyConfiguration(new NotificacionConfig());

        // Índices para validación de solapamiento
        modelBuilder.Entity<Reserva>()
            .HasIndex(r => new { r.BarberoId, r.FechaInicio, r.FechaFin });
        
        modelBuilder.Entity<Reserva>()
            .HasIndex(r => new { r.ServicioId, r.FechaInicio, r.FechaFin });
    }
}
