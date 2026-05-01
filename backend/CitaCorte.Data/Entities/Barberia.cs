namespace CitaCorte.Data.Entities;

public class Barberia
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public SubscriptionType CurrentSubscription { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime SubscriptionEndDate { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Pending;
    public int MaxAffiliatedBarbers { get; set; }
    public bool IsActive { get; set; } = false; // Only active if subscription is approved
    
    // Navigation properties
    public ICollection<Barbero> AffiliatedBarbers { get; set; } = new List<Barbero>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<BarberiaSubscriptionChange> SubscriptionChanges { get; set; } = new List<BarberiaSubscriptionChange>();
    public ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();
}
