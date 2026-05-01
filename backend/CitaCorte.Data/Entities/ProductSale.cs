namespace CitaCorte.Data.Entities;

public class ProductSale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
}
