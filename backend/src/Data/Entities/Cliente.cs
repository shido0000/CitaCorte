namespace CitaCorte.Data.Entities;

public class Cliente
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
