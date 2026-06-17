using Microsoft.EntityFrameworkCore;
using SaleApi.Data;
using SaleApi.Models;

namespace SaleApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SaleContextDB _context;

        public OrderRepository(SaleContextDB saleContextDB) => _context = saleContextDB;

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.Gift)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<Order> AddOrder(Order order)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == order.IdUser);
            var giftExists = await _context.Gifts.AnyAsync(g => g.Id == order.IdGift);

            if (!userExists || !giftExists)
                throw new Exception("User or Gift not found in database.");

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByGiftId(int giftId)
        {
            return await _context.Orders
                .Where(o => o.IdGift == giftId)
                .Include(o => o.User)
                .Include(o => o.Gift)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            return await _context.Orders
                .Where(o => o.IdUser == userId)
                .Include(o => o.Gift)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersSortedByPopularity()
        {
            return await _context.Orders
                .Include(o => o.Gift)
                .Include(o => o.User)
                .GroupBy(o => o.IdGift)
                .OrderByDescending(g => g.Count())
                .SelectMany(g => g)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersSortedByPrice()
        {
            return await _context.Orders
                .Include(o => o.Gift)
                .Include(o => o.User)
                .OrderByDescending(o => o.Gift.Price)
                .ToListAsync();
        }
    }
}
