using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CitaCorte.Data.Entities;

namespace CitaCorte.Data.Config;

public class PlanSuscripcionConfig : IEntityTypeConfiguration<PlanSuscripcion>
{
    public void Configure(EntityTypeBuilder<PlanSuscripcion> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(p => p.TipoPlan)
            .IsRequired();
        
        builder.Property(p => p.Descripcion)
            .HasMaxLength(500);
        
        builder.Property(p => p.PrecioMensual)
            .HasPrecision(10, 2);
        
        builder.Property(p => p.DuracionDias)
            .IsRequired();
        
        // Datos semilla para planes de barbero
        builder.HasData(
            new PlanSuscripcion
            {
                Id = 1,
                Nombre = "Free",
                TipoPlan = TipoPlanEnum.Free,
                Descripcion = "Plan básico para darse a conocer. No permite recibir reservas.",
                PrecioMensual = 0,
                DuracionDias = 365,
                EsParaBarbero = true,
                EsParaBarberia = false,
                PermiteReservas = false,
                PermiteEstadisticas = false,
                PermiteProductos = false,
                Activo = true
            },
            new PlanSuscripcion
            {
                Id = 2,
                Nombre = "Popular",
                TipoPlan = TipoPlanEnum.Popular,
                Descripcion = "Permite recibir reservas y ver estadísticas básicas.",
                PrecioMensual = 19.99m,
                DuracionDias = 30,
                EsParaBarbero = true,
                EsParaBarberia = false,
                PermiteReservas = true,
                PermiteEstadisticas = true,
                PermiteProductos = false,
                Activo = true
            },
            new PlanSuscripcion
            {
                Id = 3,
                Nombre = "Premium",
                TipoPlan = TipoPlanEnum.Premium,
                Descripcion = "Todo lo del plan Popular más venta de productos.",
                PrecioMensual = 39.99m,
                DuracionDias = 30,
                EsParaBarbero = true,
                EsParaBarberia = false,
                PermiteReservas = true,
                PermiteEstadisticas = true,
                PermiteProductos = true,
                Activo = true
            }
        );
    }
}
