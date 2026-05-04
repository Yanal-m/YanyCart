namespace MyShoppingCart.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public List<CartItem> Items { get; set; } = new();
    }
}
