using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Offer;
using RestaurantOrderingSystem.Repositories.Interfaces;

namespace RestaurantOrderingSystem.Repositories.Implementations
{
    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext _context;

        public OfferRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================================
        // GET ALL OFFERS FOR RESTAURANT
        // =============================================

        public async Task<List<Offer>> GetByRestaurantIdAsync(
            int restaurantId)
        {
            return await _context.Offers
                .Include(x => x.Category)
                .Include(x => x.MenuItem)
                .Where(x =>
                    x.RestaurantId == restaurantId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }


        // =============================================
        // GET OFFER BY ID
        // =============================================

        public async Task<Offer?> GetByIdAsync(
            int offerId,
            int restaurantId)
        {
            return await _context.Offers
                .Include(x => x.Category)
                .Include(x => x.MenuItem)
                .FirstOrDefaultAsync(x =>
                    x.Id == offerId &&
                    x.RestaurantId == restaurantId);
        }


        // =============================================
        // ADD OFFER
        // =============================================

        public async Task AddAsync(
            Offer offer)
        {
            await _context.Offers.AddAsync(offer);
        }


        // =============================================
        // UPDATE OFFER
        // =============================================

        public Task UpdateAsync(
            Offer offer)
        {
            _context.Offers.Update(offer);

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