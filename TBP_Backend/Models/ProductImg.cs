using System.ComponentModel.DataAnnotations;

namespace TBP_Backend.Models
{
    public class ProductImg
    {
        [Key]
        public int ImageId { get; set; }

        public int ProductId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public Product Product { get; set; }
    }
}
