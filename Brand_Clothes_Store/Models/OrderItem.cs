using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brand_Clothes_Store.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Variant")]
        public int VariantId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Range(0, 100000)]
        [Display(Name = "Unit Price")]
        public double UnitPrice { get; set; }

        [Display(Name = "Total Price")]
        public double TotalPrice { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public ProductVariant Variant { get; set; } = null!;
    }
}
