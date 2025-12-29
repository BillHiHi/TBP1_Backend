using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TBP_Backend.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required, MaxLength(200)]
        public string ProductName { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? OldPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        // Navigation
        public Category Category { get; set; }
        public ICollection<ProductImg> ProductImg { get; set; }
        public ICollection<ProductVariant> ProductVariant { get; set; }
    }
}
