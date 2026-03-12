namespace Brand_Clothes_Store.Models
{
    public class CartItemDto
    {
        public int VariantId { get; set; }
        public string? ProductName { get; set; }
        public double? Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; } = 1;
    }

}
