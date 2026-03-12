using Microsoft.AspNetCore.Identity;

namespace Brand_Clothes_Store.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        //public DateTime? DateOfBirth { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Order>? Orders { get; set; }
        public List<Favorite> Favorites { get; set; }
    }
}
