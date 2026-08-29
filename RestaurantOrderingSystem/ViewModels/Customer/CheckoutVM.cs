using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.ViewModels.Customer
{
    public class CheckoutVM
    {
        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        // Dine-In or Parcel
        public string OrderType { get; set; } = string.Empty;

        // Required only for Dine-In
        public int? TableId { get; set; }

        public List<RestaurantTableVM> AvailableTables { get; set; }
            = new List<RestaurantTableVM>();
    }

    public class RestaurantTableVM
    {
        public int TableId { get; set; }

        public string TableNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
    }
}