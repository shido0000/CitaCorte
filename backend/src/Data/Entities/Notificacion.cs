namespace CitaCorte.Data.Entities;

public class Notificacion
{
    public int Id { get; set; }
    
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? Tipo { get; set; } // Ej: "SUSCRIPCION", "RESERVA", "AFILIACION", etc.
    public string? EntidadRelacionada { get; set; } // Ej: "Reserva:123", "SolicitudAfiliacion:456"
    
    public bool Leida { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaLectura { get; set; }
}
