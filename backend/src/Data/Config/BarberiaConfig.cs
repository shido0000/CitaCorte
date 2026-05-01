using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class BarberiaConfig : IEntityTypeConfiguration<Barberia>
{
    public void Configure(EntityTypeBuilder<Barberia> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.UsuarioId)
            .IsRequired();
        
        builder.HasOne(b => b.Usuario)
            .WithOne(u => u.Barberia)
            .HasForeignKey<Barberia>(b => b.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(b => b.Servicios)
            .WithOne(s => s.Barberia)
            .HasForeignKey(s => s.BarberiaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(b => b.UsuarioId)
            .IsUnique();
    }
}
