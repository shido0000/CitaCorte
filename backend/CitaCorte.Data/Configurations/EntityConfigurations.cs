using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Configurations;

public class BarberoConfiguration : IEntityTypeConfiguration<Barbero>
{
    public void Configure(EntityTypeBuilder<Barbero> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Bio).HasMaxLength(500);
        builder.Property(b => b.ProfileImageUrl).HasMaxLength(500);
        builder.Property(b => b.Address).HasMaxLength(500);
        
        // Affiliation with Barberia - when approved, reservations redirect to barberia
        builder.HasOne(b => b.Barberia)
            .WithMany(br => br.AffiliatedBarbers)
            .HasForeignKey(b => b.BarberiaId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // Subscription dates
        builder.Property(b => b.SubscriptionStartDate).IsRequired();
        builder.Property(b => b.SubscriptionEndDate).IsRequired();
        
        // Index for searching barberos by location
        builder.HasIndex(b => new { b.Latitude, b.Longitude });
    }
}

public class BarberiaConfiguration : IEntityTypeConfiguration<Barberia>
{
    public void Configure(EntityTypeBuilder<Barberia> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Description).HasMaxLength(1000);
        builder.Property(b => b.Address).IsRequired().HasMaxLength(500);
        builder.Property(b => b.Phone).IsRequired().HasMaxLength(20);
        builder.Property(b => b.ProfileImageUrl).HasMaxLength(500);
        
        // Subscription is required for barberia (no free plan)
        builder.Property(b => b.CurrentSubscription).IsRequired();
        builder.Property(b => b.SubscriptionStartDate).IsRequired();
        builder.Property(b => b.SubscriptionEndDate).IsRequired();
        builder.Property(b => b.SubscriptionStatus).IsRequired();
        builder.Property(b => b.MaxAffiliatedBarbers).IsRequired();
        
        // Index for searching barberias by location
        builder.HasIndex(b => new { b.Latitude, b.Longitude });
    }
}

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        
        // Validate no overlapping dates - this is enforced at service level
        builder.Property(r => r.StartDateTime).IsRequired();
        builder.Property(r => r.EndDateTime).IsRequired();
        builder.HasIndex(r => new { r.BarberoId, r.StartDateTime, r.EndDateTime });
        builder.HasIndex(r => new { r.BarberiaId, r.StartDateTime, r.EndDateTime });
        
        builder.HasOne(r => r.Cliente)
            .WithMany(c => c.Reservations)
            .HasForeignKey(r => r.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(r => r.Service)
            .WithMany(s => s.Reservations)
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(r => r.Barbero)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BarberoId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(r => r.Barberia)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BarberiaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.Price).IsRequired();
        builder.Property(s => s.DurationMinutes).IsRequired();
        
        // Service can belong to either Barbero or Barberia
        builder.HasOne(s => s.Barbero)
            .WithMany(b => b.Services)
            .HasForeignKey(s => s.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(s => s.Barberia)
            .WithMany(b => b.Services)
            .HasForeignKey(s => s.BarberiaId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Check constraint: must have either BarberoId or BarberiaId
        builder.HasCheckConstraint("CK_Service_BarberoOrBarberia", "([BarberoId] IS NOT NULL AND [BarberiaId] IS NULL) OR ([BarberoId] IS NULL AND [BarberiaId] IS NOT NULL)");
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
        
        builder.HasOne(p => p.Barbero)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
        builder.Property(sp => sp.Description).HasMaxLength(500);
        builder.Property(sp => sp.Price).IsRequired();
        builder.Property(sp => sp.DurationDays).IsRequired();
        builder.HasIndex(sp => sp.Type);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
