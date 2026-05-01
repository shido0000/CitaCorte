using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class BarberoConfig : IEntityTypeConfiguration<Barbero>
{
    public void Configure(EntityTypeBuilder<Barbero> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.UsuarioId)
            .IsRequired();
        
        builder.HasOne(b => b.Usuario)
            .WithOne(u => u.Barbero)
            .HasForeignKey<Barbero>(b => b.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(b => b.BarberiaAfiliada)
            .WithMany(br => br.BarberosAfiliados)
            .HasForeignKey(b => b.BarberiaAfiliadaId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(b => b.Servicios)
            .WithOne(s => s.Barbero)
            .HasForeignKey(s => s.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(b => b.Productos)
            .WithOne(p => p.Barbero)
            .HasForeignKey(p => p.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(b => b.UsuarioId)
            .IsUnique();
    }
}
