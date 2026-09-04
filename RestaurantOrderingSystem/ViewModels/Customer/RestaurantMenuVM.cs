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

    //public class PublicMenuItemVM
    //{
    //    public int MenuItemId { get; set; }

    //    public string Name { get; set; } = string.Empty;

    //    public string? Description { get; set; }

    //    public decimal Price { get; set; }

    //    public string? ImagePath { get; set; }

    //    public bool IsAvailable { get; set; }
    //}

    public class PublicMenuItemVM
    {
        public int MenuItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Original menu price
        public decimal Price { get; set; }

        // Final price after Offer OR Discount
        public decimal FinalPrice { get; set; }

        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; }

        // ============================
        // OFFER
        // ============================

        public bool HasOffer { get; set; }

        public string? OfferName { get; set; }

        public decimal OfferPercentage { get; set; }

        // ============================
        // DISCOUNT
        // ============================

        public bool HasDiscount { get; set; }

        public string? DiscountName { get; set; }

        public decimal DiscountPercentage { get; set; }
    }
}