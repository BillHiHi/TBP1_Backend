using System.ComponentModel.DataAnnotations;

namespace TBP_Backend.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }

    }
}
