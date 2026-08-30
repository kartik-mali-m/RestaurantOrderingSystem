using System.ComponentModel.DataAnnotations;
using RestaurantOrderingSystem.Models.Offer;

namespace RestaurantOrderingSystem.ViewModels.Offer
{
    public class OfferVM
    {
        public int Id { get; set; }


        // =============================================
        // OFFER DETAILS
        // =============================================

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;


        [StringLength(500)]
        public string? Description { get; set; }


        // =============================================
        // WHERE OFFER APPLIES
        // =============================================

        [Required]
        public OfferTargetType TargetType { get; set; }


        // Used only for Category Offer
        public int? CategoryId { get; set; }


        // Used only for Menu Item Offer
        public int? MenuItemId { get; set; }


        // =============================================
        // DISCOUNT PERCENTAGE
        // =============================================

        [Required]
        [Range(0.01, 100)]
        public decimal DiscountPercentage { get; set; }


        // =============================================
        // OFFER TIME
        // =============================================

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }


        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }


        // =============================================
        // STATUS
        // =============================================

        public bool IsActive { get; set; } = true;
    }
}