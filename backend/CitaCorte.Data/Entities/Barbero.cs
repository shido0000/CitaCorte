namespace CitaCorte.Data.Entities;

public class Barbero
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Address { get; set; }
    public SubscriptionType CurrentSubscription { get; set; } = SubscriptionType.Free;
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime SubscriptionEndDate { get; set; }
    public bool CanReceiveReservations => CurrentSubscription != SubscriptionType.Free;
    public bool CanSellProducts => CurrentSubscription == SubscriptionType.Premium;
    
    // Affiliation with Barberia
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
    public AffiliationStatus AffiliationStatus { get; set; } = AffiliationStatus.Pending;
    public DateTime? AffiliationRequestedAt { get; set; }
    public DateTime? AffiliationApprovedAt { get; set; }
    
    // Navigation properties
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<BarberoSubscriptionChange> SubscriptionChanges { get; set; } = new List<BarberoSubscriptionChange>();
    public ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();
}
