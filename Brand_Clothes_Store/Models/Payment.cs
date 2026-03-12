using Brand_Clothes_Store.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brand_Clothes_Store.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        
        public PaymentMethod paymentMethod { get; set; } = PaymentMethod.Cash; // Cash / Card / etc.

        [Range(0, 100000)]
        public double Amount { get; set; }

        
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; // Pending / Paid / Failed

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
