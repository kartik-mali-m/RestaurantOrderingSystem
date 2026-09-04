using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.ViewModels.Offer
{
    public class DiscountVM
    {
        public int Id { get; set; }


        // =============================================
        // MENU ITEM
        // =============================================

        [Required(ErrorMessage = "Please select a menu item.")]
        public int MenuItemId { get; set; }


        // =============================================
        // DISCOUNT DETAILS
        // =============================================

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;


        [Required]
        [Range(0.01, 100)]
        public decimal DiscountPercentage { get; set; }


        // =============================================
        // STATUS
        // =============================================

        public bool IsActive { get; set; } = true;
    }
}