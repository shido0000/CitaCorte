using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class ServicioConfig : IEntityTypeConfiguration<Servicio>
{
    public void Configure(EntityTypeBuilder<Servicio> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Nombre)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(s => s.Precio)
            .IsRequired()
            .HasPrecision(10, 2);
        
        builder.Property(s => s.DuracionMinutos)
            .IsRequired();
        
        builder.HasOne(s => s.Barbero)
            .WithMany(b => b.Servicios)
            .HasForeignKey(s => s.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(s => s.Barberia)
            .WithMany(b => b.Servicios)
            .HasForeignKey(s => s.BarberiaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
