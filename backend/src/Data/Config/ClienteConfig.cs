using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class ClienteConfig : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.UsuarioId)
            .IsRequired();
        
        builder.HasOne(c => c.Usuario)
            .WithOne()
            .HasForeignKey<Cliente>(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(c => c.UsuarioId)
            .IsUnique();
    }
}
