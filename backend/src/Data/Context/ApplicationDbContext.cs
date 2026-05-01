using Microsoft.EntityFrameworkCore;
using CitaCorte.API.Data.Entities;

namespace CitaCorte.API.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Barbero> Barberos { get; set; }
    public DbSet<Barberia> Barberias { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<AffiliationRequest> AffiliationRequests { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<BarberoStatistic> BarberoStatistics { get; set; }
    public DbSet<BarberiaStatistic> BarberiaStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.BarberoProfile)
            .WithOne(b => b.User)
            .HasForeignKey<Barbero>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.BarberiaProfile)
            .WithOne(b => b.User)
            .HasForeignKey<Barberia>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Barbero - Barberia relationship
        modelBuilder.Entity<Barbero>()
            .HasOne(b => b.Barberia)
            .WithMany(br => br.AffiliatedBarberos)
            .HasForeignKey(b => b.BarberiaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Service relationships
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Barbero)
            .WithMany(b => b.Services)
            .HasForeignKey(s => s.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Service>()
            .HasOne(s => s.Barberia)
            .WithMany(b => b.Services)
            .HasForeignKey(s => s.BarberiaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Reservation relationships
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.ClientUser)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.ClientUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Barbero)
            .WithMany()
            .HasForeignKey(r => r.BarberoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Barberia)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BarberiaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure AffiliationRequest relationships
        modelBuilder.Entity<AffiliationRequest>()
            .HasOne(ar => ar.Barbero)
            .WithMany(b => b.SentAffiliationRequests)
            .HasForeignKey(ar => ar.BarberoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AffiliationRequest>()
            .HasOne(ar => ar.Barberia)
            .WithMany(b => b.ReceivedAffiliationRequests)
            .HasForeignKey(ar => ar.BarberiaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Notification relationships
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Statistics relationships
        modelBuilder.Entity<BarberoStatistic>()
            .HasOne(bs => bs.Barbero)
            .WithMany(b => b.Statistics)
            .HasForeignKey(bs => bs.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BarberiaStatistic>()
            .HasOne(bs => bs.Barberia)
            .WithMany(b => b.Statistics)
            .HasForeignKey(bs => bs.BarberiaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.BarberoId, r.StartDateTime, r.EndDateTime });

        modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.BarberiaId, r.StartDateTime, r.EndDateTime });

        modelBuilder.Entity<SubscriptionPlan>()
            .HasIndex(sp => sp.Type);
    }
}
