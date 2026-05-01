using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class SubscriptionPlan
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public SubscriptionType Type { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    // Target audience
    public bool IsForBarbero { get; set; }
    public bool IsForBarberia { get; set; }
    
    // Pricing
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Duration in days
    public int DurationDays { get; set; }
    
    // Features and limits
    public bool CanReceiveReservations { get; set; }
    public bool CanViewStatistics { get; set; }
    public bool CanSellProducts { get; set; }
    public bool CanEditProfile { get; set; }
    public bool CanViewServices { get; set; }
    
    // For barberias: max affiliated barbers
    public int? MaxBarberosLimit { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
