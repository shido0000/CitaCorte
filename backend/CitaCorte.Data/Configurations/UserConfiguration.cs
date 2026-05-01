using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.Phone).HasMaxLength(20);
        
        builder.HasOne(u => u.Barbero)
            .WithOne(b => b.User)
            .HasForeignKey<Barbero>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(u => u.Barberia)
            .WithOne(b => b.User)
            .HasForeignKey<Barberia>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(u => u.Comercial)
            .WithOne(c => c.User)
            .HasForeignKey<Comercial>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(u => u.Cliente)
            .WithOne(cl => cl.User)
            .HasForeignKey<Cliente>(cl => cl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(u => u.Admin)
            .WithOne(a => a.User)
            .HasForeignKey<Admin>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
