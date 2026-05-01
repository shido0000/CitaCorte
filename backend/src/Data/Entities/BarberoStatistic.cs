using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class BarberoStatistic
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int BarberoId { get; set; }
    public Barbero Barbero { get; set; } = null!;
    
    // Date of the statistic record
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    
    // Metrics
    public int TotalReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int CancelledReservations { get; set; }
    public int CompletedReservations { get; set; }
    
    public decimal TotalRevenue { get; set; }
    
    public int NewClients { get; set; }
    public int ReturningClients { get; set; }
    
    public int ProductsSold { get; set; }
    public decimal ProductsRevenue { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
