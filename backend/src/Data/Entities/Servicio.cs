namespace CitaCorte.Data.Entities;

public class Servicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int DuracionMinutos { get; set; }
    public bool Activo { get; set; } = true;
    
    // Puede pertenecer a un barbero o una barbería
    public int? BarberoId { get; set; }
    public Barbero? Barbero { get; set; }
    
    public int? BarberiaId { get; set; }
    public Barberia? Barberia { get; set; }
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
