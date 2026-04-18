using Microsoft.AspNetCore.Identity;

namespace CinemaApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public string? Address { get; set; }
    }
}
