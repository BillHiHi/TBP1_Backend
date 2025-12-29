using System.ComponentModel.DataAnnotations;

namespace TBP_Backend.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        public int CartID { get; set; }
        public int VariantID { get; set; }
        public int Quantity { get; set; }

        public Cart Cart { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
