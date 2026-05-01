namespace CitaCorte.Data.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Barbero? Barbero { get; set; }
    public Barberia? Barberia { get; set; }
    public Comercial? Comercial { get; set; }
    public Cliente? Cliente { get; set; }
    public Admin? Admin { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
