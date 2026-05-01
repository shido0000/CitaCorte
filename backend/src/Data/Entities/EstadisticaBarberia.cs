namespace CitaCorte.Data.Entities;

public class EstadisticaBarberia
{
    public int Id { get; set; }
    
    public int BarberiaId { get; set; }
    public Barberia Barberia { get; set; } = null!;
    
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
    public int BarberosActivos { get; set; }
    public int NuevosBarberos { get; set; }
    
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
}
