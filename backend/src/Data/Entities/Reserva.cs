namespace CitaCorte.Data.Entities;

public class Reserva
{
    public int Id { get; set; }
    
    // Cliente que hace la reserva
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    
    // Servicio reservado
    public int ServicioId { get; set; }
    public Servicio Servicio { get; set; } = null!;
    
    // Barbero que atenderá (puede ser null si es reserva directa a barbería)
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    
    // Barbería donde se atenderá (si el barbero está afiliado o es reserva directa)
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
    
    // Fecha y hora de inicio y fin de la reserva
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    
    public EstadoReservaEnum Estado { get; set; } = EstadoReservaEnum.Pendiente;
    public string? NotasCliente { get; set; }
    public string? NotasInternas { get; set; }
    public decimal PrecioTotal { get; set; }
    
    public DateTime FechaReserva { get; set; } = DateTime.UtcNow;
    public DateTime? FechaConfirmacion { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public string? MotivoCancelacion { get; set; }

    // Navegación
    public ICollection<EstadisticaReserva> Estadisticas { get; set; } = new List<EstadisticaReserva>();
}
