using Microsoft.AspNetCore.Identity;

namespace Elite.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
