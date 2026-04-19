namespace Printawyapis.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }

        public string Size { get; set; }

        public string FrontDesign { get; set; }
        public string BackDesign { get; set; }

        public int Quantity { get; set; }
    }
}