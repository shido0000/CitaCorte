namespace CitaCorte.Data.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? FotoPerfilUrl { get; set; }
    public RolEnum Rol { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaUltimoAcceso { get; set; }

    // Navegación
    public Barbero? Barbero { get; set; }
    public Barberia? Barberia { get; set; }
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
}
