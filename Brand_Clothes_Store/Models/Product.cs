using Brand_Clothes_Store.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brand_Clothes_Store.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }


        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 100000)]
        public double BasePrice { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ProductStatus Status { get; set; }=ProductStatus.Active;
        public bool IsFavorite { get; set; } = false;

        // Navigation
        public Category? Category { get; set; }
        public ICollection<ProductVariant>? Variants { get; set; }
        public List<Favorite> Favorites { get; set; }

    }
}
