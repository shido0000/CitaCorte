namespace CitaCorte.Data.Entities;

public class EstadisticaBarbero
{
    public int Id { get; set; }
    
    public int BarberoId { get; set; }
    public Barbero Barbero { get; set; } = null!;
    
    // Periodo de la estadística
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    
    // Métricas
    public int TotalReservas { get; set; }
    public int ReservasConfirmadas { get; set; }
    public int ReservasRechazadas { get; set; }
    public int ReservasCompletadas { get; set; }
    public int TotalClientesAtendidos { get; set; }
    public decimal IngresosTotales { get; set; }
    public int ProductosVendidos { get; set; }
    public decimal IngresosProductos { get; set; }
    
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
}
