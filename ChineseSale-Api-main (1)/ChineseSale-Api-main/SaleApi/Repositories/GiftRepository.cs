using Microsoft.EntityFrameworkCore;
using SaleApi.Data;
using SaleApi.Models;

namespace SaleApi.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        SaleContextDB _context;

        public GiftRepository(SaleContextDB saleContextDB)
        {
            _context = saleContextDB;
        }

        public async Task<IEnumerable<Gift>> GetAllGift()
        {
            try
            {
                return await _context.Gifts.Include(g => g.Doner)
                     .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllGift repository: {ex.Message}");
                throw;
            }
        }

        public async Task<Gift> NewGift(Gift gift)
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task DeleteGift(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift != null)
            {
                _context.Gifts.Remove(gift);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Gift?> GetGiftById(int id)
        {
            return await _context.Gifts
                .Include(c => c.Doner)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Gift> UpdateGift(Gift gift)
        {
            _context.Gifts.Update(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task<Doner?> GetGiftDoner(int id)
        {
            var giftWithDoner = await _context.Gifts
                .Include(g => g.Doner)
                .FirstOrDefaultAsync(g => g.Id == id);

            return giftWithDoner?.Doner;
        }

        public async Task<IEnumerable<Gift?>> GetGiftByDoner(string name)
        {
            return await _context.Gifts.Include(g => g.Doner)
                .Where(g => g.Doner.FirstName.StartsWith(name)
                || g.Doner.LastName.StartsWith(name)).ToArrayAsync();
        }

        public async Task<IEnumerable<Gift?>> GetGiftByName(string name)
        {
            return await _context.Gifts.Include(g => g.Doner)
                .Where(g => g.Name.StartsWith(name))
                .ToArrayAsync();
        }
    }
}
