namespace MyShoppingCart.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }

        public decimal GetPriceWithTax(decimal taxRate)
        {
            return Price * (1 + taxRate);
        }
    }
}
