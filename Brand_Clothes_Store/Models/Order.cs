using Brand_Clothes_Store.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brand_Clothes_Store.Models
{
    public class Order
    {
        public int Id { get; set; }

        public OrderStatus orderStatus { get; set; } = OrderStatus.Processing; 

        [Range(0, 100000)]
        [Display(Name = "Total Price")]
        public double TotalPrice { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Navigation
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public PaymentMethod? Payment { get; set; }
    }
}
