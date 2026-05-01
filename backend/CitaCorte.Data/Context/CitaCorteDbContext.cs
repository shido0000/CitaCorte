using Microsoft.EntityFrameworkCore;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Context;

public class CitaCorteDbContext : DbContext
{
    public CitaCorteDbContext(DbContextOptions<CitaCorteDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Barbero> Barberos { get; set; }
    public DbSet<Barberia> Barberias { get; set; }
    public DbSet<Comercial> Comerciales { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ProductSale> ProductSales { get; set; }
    public DbSet<Statistic> Statistics { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<BarberoSubscriptionChange> BarberoSubscriptionChanges { get; set; }
    public DbSet<BarberiaSubscriptionChange> BarberiaSubscriptionChanges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CitaCorteDbContext).Assembly);

        // Global query filters for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);
    }
}
