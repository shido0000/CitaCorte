using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class Barberia
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    // Subscription info - no free plan for barberias
    public SubscriptionType CurrentSubscription { get; set; } = SubscriptionType.Popular;
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Pending;
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    
    // Max affiliated barbers based on subscription
    public int MaxBarberos { get; set; } = 0;
    public int CurrentBarberosCount { get; set; } = 0;
    
    // Profile info
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(300)]
    public string? Address { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(50)]
    public string? State { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(256)]
    public string? LogoUrl { get; set; }
    
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public List<Barbero> AffiliatedBarberos { get; set; } = new();
    public List<Service> Services { get; set; } = new();
    public List<Reservation> Reservations { get; set; } = new();
    public List<AffiliationRequest> ReceivedAffiliationRequests { get; set; } = new();
    public List<BarberiaStatistic> Statistics { get; set; } = new();
}
