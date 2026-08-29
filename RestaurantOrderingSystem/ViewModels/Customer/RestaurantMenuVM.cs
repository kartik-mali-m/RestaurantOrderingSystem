namespace RestaurantOrderingSystem.ViewModels.Customer
{
    public class RestaurantMenuVM
    {
        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; } = string.Empty;

        public string? RestaurantLogo { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public List<CategoryMenuVM> Categories { get; set; }
            = new List<CategoryMenuVM>();
    }

    public class CategoryMenuVM
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<PublicMenuItemVM> MenuItems { get; set; }
            = new List<PublicMenuItemVM>();
    }

    public class PublicMenuItemVM
    {
        public int MenuItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; }
    }
}