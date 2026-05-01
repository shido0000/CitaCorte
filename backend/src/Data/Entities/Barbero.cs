using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitaCorte.API.Data.Entities;

public class Barbero
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    
    // Current subscription
    public SubscriptionType CurrentSubscription { get; set; } = SubscriptionType.Free;
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Pending;
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    
    // Affiliation to a barbershop
    public int? BarberiaId { get; set; }
    [ForeignKey(nameof(BarberiaId))]
    public Barberia? Barberia { get; set; }
    
    public AffiliationStatus AffiliationStatus { get; set; } = AffiliationStatus.Rejected;
    public DateTime? AffiliationRequestedAt { get; set; }
    
    // Profile info
    [MaxLength(500)]
    public string? Bio { get; set; }
    
    [MaxLength(256)]
    public string? ProfileImageUrl { get; set; }
    
    [MaxLength(100)]
    public string? Specialties { get; set; }
    
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    
    // Navigation properties
    public List<Service> Services { get; set; } = new();
    public List<Product>? Products { get; set; }
    public List<BarberoStatistic> Statistics { get; set; } = new();
    public List<AffiliationRequest> SentAffiliationRequests { get; set; } = new();
}
