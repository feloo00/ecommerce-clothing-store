using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brand_Clothes_Store.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [StringLength(30)]
        public string? Color { get; set; }

        [StringLength(10)]
        public string? Size { get; set; }

        [Range(0, 100000)]
        public double? Price { get; set; }

        [Range(0, 1000)]
        public int Stock { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [ValidateNever]
        public Product Product { get; set; } = null!;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
