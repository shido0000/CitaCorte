namespace CitaCorte.Data.Entities;

public class Comercial
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Navigation properties
    public ICollection<BarberoSubscriptionChange> ApprovedBarberoChanges { get; set; } = new List<BarberoSubscriptionChange>();
    public ICollection<BarberiaSubscriptionChange> ApprovedBarberiaChanges { get; set; } = new List<BarberiaSubscriptionChange>();
}
