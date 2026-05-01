namespace CitaCorte.Data.Entities;

public class Statistic
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int TotalReservations { get; set; }
    public int CompletedReservations { get; set; }
    public int CancelledReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public int NewClients { get; set; }
    public int TotalProductsSold { get; set; }
    public decimal ProductsRevenue { get; set; }
    
    // Can belong to a Barbero or a Barberia
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
}
