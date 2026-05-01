using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class Reservation
{
    [Key]
    public int Id { get; set; }
    
    // Client who made the reservation
    [Required]
    public int ClientUserId { get; set; }
    public User ClientUser { get; set; } = null!;
    
    // Service being reserved
    [Required]
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    
    // Who provides the service - can be barbero or barberia
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
    
    // Reservation date and time
    [Required]
    public DateTime StartDateTime { get; set; }
    
    [Required]
    public DateTime EndDateTime { get; set; }
    
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}
