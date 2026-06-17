using Microsoft.EntityFrameworkCore;
using SaleApi.Data;
using SaleApi.Models;

namespace SaleApi.Repositories
{
    public class RandomRepository : IRandomRepository
    {
        SaleContextDB _context;

        public RandomRepository(SaleContextDB saleContextDB)
        {
            _context = saleContextDB;
        }

        public async Task<IEnumerable<int>> GetOrdersForGift(int giftId)
        {
            return await _context.Orders
                .Where(o => o.IdGift == giftId)
                .Select(o => o.Id)
                .ToListAsync();
        }

        public async Task<bool> IsGiftRandom(int giftId)
        {
            return await _context.Winners.AnyAsync(w => w.IdGift == giftId);
        }

        public async Task SaveWinner(Winner winner, int orderId)
        {
            _context.Winners.Add(winner);
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                order.Win = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }

        public async Task<bool> IsGiftDrawnAsync(int giftId)
        {
            return await _context.Winners.AnyAsync(w => w.IdGift == giftId);
        }

        public async Task<IEnumerable<Winner>> GetDrawnGiftIdsAsync()
        {
            return await _context.Winners.Include(w => w.User).Include(w => w.Gift).ToListAsync();
        }
    }
}
