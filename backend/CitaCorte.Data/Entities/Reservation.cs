namespace CitaCorte.Data.Entities;

public class Reservation
{
    public int Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Client who made the reservation
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    
    // Service being reserved
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    
    // Barbero or Barberia providing the service
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
}
