using System.ComponentModel.DataAnnotations;

namespace TBP_Backend.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public string UserId { get; set; } 

        public DateTime CreatedAt { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}
