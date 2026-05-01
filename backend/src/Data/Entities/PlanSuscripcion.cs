namespace CitaCorte.Data.Entities;

public class PlanSuscripcion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // Free, Popular, Premium, etc.
    public TipoPlanEnum TipoPlan { get; set; }
    public string? Descripcion { get; set; }
    public decimal PrecioMensual { get; set; }
    public int DuracionDias { get; set; } // Duración del plan en días
    public bool EsParaBarbero { get; set; } // true si es para barbero, false si es para barbería
    public bool EsParaBarberia { get; set; } // true si es para barbería
    
    // Límites y características
    public int? MaxBarberosAfiliados { get; set; } // Solo para planes de barbería
    public bool PermiteReservas { get; set; } = false;
    public bool PermiteEstadisticas { get; set; } = false;
    public bool PermiteProductos { get; set; } = false;
    
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}
