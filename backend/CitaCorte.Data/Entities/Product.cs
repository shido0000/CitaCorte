namespace CitaCorte.Data.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int BarberoId { get; set; }
    public Barbero Barbero { get; set; } = null!;
    
    // Navigation properties
    public ICollection<ProductSale> Sales { get; set; } = new List<ProductSale>();
}
