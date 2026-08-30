using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Offer;
using RestaurantOrderingSystem.Repositories.Interfaces;

namespace RestaurantOrderingSystem.Repositories.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =============================================
        // GET ALL DISCOUNTS FOR RESTAURANT
        // =============================================

        public async Task<List<Discount>> GetByRestaurantIdAsync(
            int restaurantId)
        {
            return await _context.Discounts
                .Include(x => x.MenuItem)
                .Where(x =>
                    x.RestaurantId == restaurantId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }


        // =============================================
        // GET DISCOUNT BY ID
        // =============================================

        public async Task<Discount?> GetByIdAsync(
            int discountId,
            int restaurantId)
        {
            return await _context.Discounts
                .Include(x => x.MenuItem)
                .FirstOrDefaultAsync(x =>
                    x.Id == discountId &&
                    x.RestaurantId == restaurantId);
        }


        // =============================================
        // GET ACTIVE DISCOUNT FOR MENU ITEM
        // =============================================

        public async Task<Discount?>
            GetActiveByMenuItemIdAsync(
                int restaurantId,
                int menuItemId)
        {
            return await _context.Discounts
                .FirstOrDefaultAsync(x =>
                    x.RestaurantId == restaurantId &&
                    x.MenuItemId == menuItemId &&
                    x.IsActive);
        }


        // =============================================
        // ADD DISCOUNT
        // =============================================

        public async Task AddAsync(
            Discount discount)
        {
            await _context.Discounts.AddAsync(discount);
        }


        // =============================================
        // UPDATE DISCOUNT
        // =============================================

        public Task UpdateAsync(
            Discount discount)
        {
            _context.Discounts.Update(discount);

            return Task.CompletedTask;
        }


        // =============================================
        // SAVE CHANGES
        // =============================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}