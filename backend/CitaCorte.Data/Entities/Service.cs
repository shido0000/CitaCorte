namespace CitaCorte.Data.Entities;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Can belong to a Barbero or a Barberia
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
    
    // Navigation properties
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
