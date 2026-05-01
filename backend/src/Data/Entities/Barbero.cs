namespace CitaCorte.Data.Entities;

public class Barbero
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public string? Descripcion { get; set; }
    public string? Especialidades { get; set; } // Separadas por coma
    public decimal? CalificacionPromedio { get; set; }
    public int TotalReservasCompletadas { get; set; } = 0;
    
    // Afiliación opcional a una barbería
    public int? BarberiaAfiliadaId { get; set; }
    public Barberia? BarberiaAfiliada { get; set; }
    
    // Suscripción actual activa
    public int? SuscripcionActivaId { get; set; }
    public Suscripcion? SuscripcionActiva { get; set; }
    
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public ICollection<Reserva> ReservasComoBarbero { get; set; } = new List<Reserva>();
    public ICollection<SolicitudAfiliacion> SolicitudesEnviadas { get; set; } = new List<SolicitudAfiliacion>();
    public ICollection<EstadisticaBarbero> Estadisticas { get; set; } = new List<EstadisticaBarbero>();
}
