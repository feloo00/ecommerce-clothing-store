namespace Brand_Clothes_Store.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public int ProductId { get; set; } 

        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}
