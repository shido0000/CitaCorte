using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitaCorte.API.Data.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public int StockQuantity { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    // Only barberos with Premium subscription can have products
    [Required]
    public int BarberoId { get; set; }
    
    [ForeignKey(nameof(BarberoId))]
    public Barbero Barbero { get; set; } = null!;
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
