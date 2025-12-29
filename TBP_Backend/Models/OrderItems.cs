using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TBP_Backend.Models
{
    public class OrderItems
    {
        [Key]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int VariantId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
