namespace CitaCorte.Data.Entities;

public class Cliente
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Navigation properties
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
