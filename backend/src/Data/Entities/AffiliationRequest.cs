using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class AffiliationRequest
{
    [Key]
    public int Id { get; set; }
    
    // Barbero requesting affiliation
    [Required]
    public int BarberoId { get; set; }
    public Barbero Barbero { get; set; } = null!;
    
    // Barberia receiving the request
    [Required]
    public int BarberiaId { get; set; }
    public Barberia Barberia { get; set; } = null!;
    
    public AffiliationStatus Status { get; set; } = AffiliationStatus.Pending;
    
    [MaxLength(500)]
    public string? Message { get; set; }
    
    [MaxLength(500)]
    public string? RejectionReason { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
}
