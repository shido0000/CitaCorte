namespace CitaCorte.Data.Entities;

public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SubscriptionType Type { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; } // 30, 90, 365, etc.
    public bool IsForBarberia { get; set; } // true for Barberia, false for Barbero
    public int MaxAffiliatedBarbers { get; set; } // Only for Barberia plans
    public bool CanReceiveReservations { get; set; }
    public bool CanSellProducts { get; set; }
    public bool HasStatistics { get; set; }
    public int AdminId { get; set; }
    public Admin Admin { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
