using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class SuscripcionConfig : IEntityTypeConfiguration<Suscripcion>
{
    public void Configure(EntityTypeBuilder<Suscripcion> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Estado)
            .IsRequired();
        
        builder.Property(s => s.FechaInicio)
            .IsRequired();
        
        builder.Property(s => s.FechaFin)
            .IsRequired();
        
        builder.Property(s => s.FechaSolicitud)
            .IsRequired();
        
        builder.HasOne(s => s.PlanSuscripcion)
            .WithMany(p => p.Suscripciones)
            .HasForeignKey(s => s.PlanSuscripcionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Barbero)
            .WithOne(b => b.SuscripcionActiva)
            .HasForeignKey<Suscripcion>(s => s.BarberoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Barberia)
            .WithOne(b => b.SuscripcionActiva)
            .HasForeignKey<Suscripcion>(s => s.BarberiaId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Índice para verificar vencimientos
        builder.HasIndex(s => s.FechaFin);
        builder.HasIndex(s => s.Estado);
    }
}
