using System.ComponentModel.DataAnnotations;

namespace TBP_Backend.Dto
{
    public class CheckoutItemDto
    {
        [Required]
        public int VariantId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
