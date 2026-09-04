using RestaurantOrderingSystem.Models.Base;
using RestaurantOrderingSystem.Models.Menu;

namespace RestaurantOrderingSystem.Models.Offer
{
    public class Offer : BaseEntity
    {
        // =============================================
        // RESTAURANT
        // =============================================

        public int RestaurantId { get; set; }


        // =============================================
        // OFFER DETAILS
        // =============================================

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }


        // =============================================
        // OFFER TARGET
        // =============================================

        public OfferTargetType TargetType { get; set; }


        // =============================================
        // OPTIONAL TARGETS
        // =============================================

        // Used when TargetType = Category
        public int? CategoryId { get; set; }


        // Used when TargetType = MenuItem
        public int? MenuItemId { get; set; }


        // =============================================
        // DISCOUNT
        // =============================================

        public decimal DiscountPercentage { get; set; }


        // =============================================
        // OFFER TIME
        // =============================================

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


        // =============================================
        // STATUS
        // =============================================

        public bool IsActive { get; set; } = true;


        // =============================================
        // NAVIGATION PROPERTIES
        // =============================================

        public Models.Restaurant.Restaurant Restaurant { get; set; }
            = null!;

        public Category? Category { get; set; }

        public MenuItem? MenuItem { get; set; }
    }
}