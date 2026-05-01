using System.ComponentModel.DataAnnotations;

namespace CitaCorte.API.Data.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    public UserRole Role { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Barbero? BarberoProfile { get; set; }
    public Barberia? BarberiaProfile { get; set; }
    public List<Notification> Notifications { get; set; } = new();
    public List<Reservation> Reservations { get; set; } = new();
}
