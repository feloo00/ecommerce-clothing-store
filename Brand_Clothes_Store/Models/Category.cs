using System.ComponentModel.DataAnnotations;

namespace Brand_Clothes_Store.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? Description { get; set; }

        // Navigation
        public ICollection<Product>? Products { get; set; }
    }
}
