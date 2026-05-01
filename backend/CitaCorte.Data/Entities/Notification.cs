namespace CitaCorte.Data.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Related entities (optional)
    public int? ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    public int? BarberoSubscriptionChangeId { get; set; }
    public BarberoSubscriptionChange? BarberoSubscriptionChange { get; set; }
    public int? BarberiaSubscriptionChangeId { get; set; }
    public BarberiaSubscriptionChange? BarberiaSubscriptionChange { get; set; }
}
