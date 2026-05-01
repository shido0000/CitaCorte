using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.Apellido)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.Telefono)
            .HasMaxLength(20);
        
        builder.Property(u => u.Rol)
            .IsRequired();
        
        // Un usuario puede tener máximo un perfil de barbero o barbería
        builder.HasOne(u => u.Barbero)
            .WithOne(b => b.Usuario)
            .HasForeignKey<Barbero>(b => b.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(u => u.Barberia)
            .WithOne(b => b.Usuario)
            .HasForeignKey<Barberia>(b => b.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.Notificaciones)
            .WithOne(n => n.Usuario)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
