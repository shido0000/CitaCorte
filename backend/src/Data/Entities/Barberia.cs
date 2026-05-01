namespace CitaCorte.Data.Entities;

public class Barberia
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public string? NombreComercial { get; set; }
    public string? Descripcion { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
    public string? CodigoPostal { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? FotoPerfilUrl { get; set; }
    public string? FotoPortadaUrl { get; set; }
    public decimal? CalificacionPromedio { get; set; }
    public int TotalReservasCompletadas { get; set; } = 0;
    
    // Suscripción actual activa
    public int? SuscripcionActivaId { get; set; }
    public Suscripcion? SuscripcionActiva { get; set; }
    
    // Estado de aprobación de suscripción (para saber si puede afiliar)
    public bool SuscripcionAprobada { get; set; } = false;
    
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    public ICollection<Barbero> BarberosAfiliados { get; set; } = new List<Barbero>();
    public ICollection<SolicitudAfiliacion> SolicitudesRecibidas { get; set; } = new List<SolicitudAfiliacion>();
    public ICollection<Reserva> ReservasComoBarberia { get; set; } = new List<Reserva>();
    public ICollection<EstadisticaBarberia> Estadisticas { get; set; } = new List<EstadisticaBarberia>();
}
