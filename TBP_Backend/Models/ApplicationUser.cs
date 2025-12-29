using Microsoft.AspNetCore.Identity;

namespace TBP_Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

    }
}
