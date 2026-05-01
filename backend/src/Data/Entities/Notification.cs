using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class Notification
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required]
    public NotificationType Type { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;
    
    public bool IsRead { get; set; } = false;
    
    // Optional reference to related entity
    public int? ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    
    public int? AffiliationRequestId { get; set; }
    public AffiliationRequest? AffiliationRequest { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
