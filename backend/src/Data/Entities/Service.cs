using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class Service
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    // Duration in minutes
    [Required]
    public int DurationMinutes { get; set; }
    
    // Owner - can be either barbero or barberia
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
