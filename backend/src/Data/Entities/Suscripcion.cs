namespace CitaCorte.Data.Entities;

public class Suscripcion
{
    public int Id { get; set; }
    public int PlanSuscripcionId { get; set; }
    public PlanSuscripcion PlanSuscripcion { get; set; } = null!;
    
    // Puede ser de un barbero o una barbería
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
    
    public EstadoSuscripcionEnum Estado { get; set; } = EstadoSuscripcionEnum.Pendiente;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime? FechaAprobacion { get; set; }
    public int? AprobadoPorUsuarioId { get; set; } // Admin o Comercial que aprobó
    public Usuario? AprobadoPorUsuario { get; set; }
    public string? MotivoRechazo { get; set; }
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
