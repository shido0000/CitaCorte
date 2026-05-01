namespace CitaCorte.Data.Entities;

public class EstadisticaReserva
{
    public int Id { get; set; }
    
    public int ReservaId { get; set; }
    public Reserva Reserva { get; set; } = null!;
    
    // Datos snapshot al momento de la reserva
    public decimal PrecioAlMomento { get; set; }
    public string EstadoActual { get; set; } = string.Empty;
    
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
