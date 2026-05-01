using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class ProductoConfig : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Precio)
            .IsRequired()
            .HasPrecision(10, 2);
        
        builder.Property(p => p.Stock)
            .IsRequired();
        
        builder.HasOne(p => p.Barbero)
            .WithMany(b => b.Productos)
            .HasForeignKey(p => p.BarberoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
