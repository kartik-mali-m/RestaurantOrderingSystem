namespace RestaurantOrderingSystem.ViewModels.Customer
{
    public class CartVM
    {
        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; } = string.Empty;

        public List<CartItemVM> Items { get; set; }
            = new List<CartItemVM>();

        public decimal TotalAmount =>
            Items.Sum(x => x.TotalPrice);
    }

    public class CartItemVM
    {
        public int MenuItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal TotalPrice =>
            Price * Quantity;
    }
}