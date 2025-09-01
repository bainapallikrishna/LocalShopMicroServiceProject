using System.ComponentModel.DataAnnotations;

namespace LocalShop.Shared.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public string Image { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }

    public class CreateProductRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public string Image { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }

    public class UpdateProductRequest
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public string? Image { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }
    }
}
