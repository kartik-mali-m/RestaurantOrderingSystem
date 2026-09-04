using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.ViewModels.Customer
{
    public class CheckoutVM
    {
        // ==========================================
        // RESTAURANT
        // ==========================================

        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; }
            = string.Empty;


        // ==========================================
        // CUSTOMER DETAILS
        // ==========================================

        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
            = string.Empty;


        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(15)]
        public string CustomerPhone { get; set; }
            = string.Empty;


        // ==========================================
        // ORDER TYPE
        // Dine-In or Parcel
        // ==========================================

        [Required(ErrorMessage = "Please select an order type")]
        public string OrderType { get; set; }
            = string.Empty;


        // ==========================================
        // TABLE
        // Required only for Dine-In
        // ==========================================

        public int? TableId { get; set; }


        // ==========================================
        // AVAILABLE TABLES
        // ==========================================

        public List<RestaurantTableVM> AvailableTables { get; set; }
            = new List<RestaurantTableVM>();


        // ==========================================
        // CART ITEMS
        // ==========================================

        public List<CartItemVM> Items { get; set; }
            = new List<CartItemVM>();


        // ==========================================
        // TOTAL
        // ==========================================

        public decimal TotalAmount =>
            Items.Sum(x => x.TotalPrice);
    }


    public class RestaurantTableVM
    {
        public int TableId { get; set; }

        public string TableNumber { get; set; }
            = string.Empty;

        public bool IsAvailable { get; set; }
    }
}