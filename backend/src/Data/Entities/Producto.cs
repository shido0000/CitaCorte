namespace CitaCorte.Data.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;
    
    // Solo los barberos con plan Premium pueden tener productos
    public int BarberoId { get; set; }
    public Barbero Barbero { get; set; } = null!;
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
