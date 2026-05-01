namespace CitaCorte.Data.Entities;

public class BarberiaSubscriptionChange
{
    public int Id { get; set; }
    public int BarberiaId { get; set; }
    public Barberia Barberia { get; set; } = null!;
    public SubscriptionType RequestedSubscription { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public Admin? ReviewedByAdmin { get; set; }
    public int? ReviewedByComercialId { get; set; }
    public Comercial? ReviewedByComercial { get; set; }
    public string? RejectionReason { get; set; }
}
