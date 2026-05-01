namespace CitaCorte.Data.Entities;

public class SolicitudAfiliacion
{
    public int Id { get; set; }
    
    public int BarberoId { get; set; }
    public Barbero Barbero { get; set; } = null!;
    
    public int BarberiaId { get; set; }
    public Barberia Barberia { get; set; } = null!;
    
    public EstadoSolicitudAfiliacionEnum Estado { get; set; } = EstadoSolicitudAfiliacionEnum.Pendiente;
    public string? MensajeSolicitud { get; set; }
    public string? MotivoRechazo { get; set; }
    
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime? FechaRespuesta { get; set; }
    public int? RespondidoPorUsuarioId { get; set; } // Usuario de la barbería que respondió
}
