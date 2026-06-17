using Microsoft.EntityFrameworkCore;
using SaleApi.Data;
using SaleApi.Models;

namespace SaleApi.Repositories
{
    public class BagRepository : IBagRepository
    {
        SaleContextDB _context;

        public BagRepository(SaleContextDB saleContextDB)
        {
            _context = saleContextDB;
        }

        public async Task<IEnumerable<Bag>> GetAllBag()
        {
            return await _context.Bags.Include(b => b.Gift).ToListAsync();
        }

        public async Task<Bag> NewGiftToBag(Bag bag)
        {
            var giftExists = await _context.Gifts.AnyAsync(g => g.Id == bag.IdGift);
            if (!giftExists) return null;

            var existingItem = await _context.Bags
                .FirstOrDefaultAsync(b => b.IdUser == bag.IdUser && b.IdGift == bag.IdGift);

            if (existingItem != null)
            {
                existingItem.Quantity += bag.Quantity;
                _context.Bags.Update(existingItem);
                await _context.SaveChangesAsync();
                return await _context.Bags.Include(b => b.Gift).FirstOrDefaultAsync(b => b.Id == existingItem.Id);
            }

            var newEntry = new Bag
            {
                IdUser = bag.IdUser,
                IdGift = bag.IdGift,
                Quantity = bag.Quantity
            };

            _context.Bags.Add(newEntry);
            await _context.SaveChangesAsync();

            return await _context.Bags.Include(b => b.Gift).FirstOrDefaultAsync(b => b.Id == newEntry.Id);
        }

        public async Task ClearUserBag(int userId)
        {
            var userItems = await _context.Bags.Where(b => b.IdUser == userId).ToListAsync();
            if (userItems.Any())
            {
                _context.Bags.RemoveRange(userItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBag(int id)
        {
            var bag = await _context.Bags.FindAsync(id);
            if (bag != null)
            {
                _context.Bags.Remove(bag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Bag?> GetBagById(int id)
        {
            return await _context.Bags.Include(b => b.Gift)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Bag?>> GetBagByUser(int id)
        {
            return await _context.Bags
                .Include(b => b.Gift)
                .Where(b => b.IdUser == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bag?>> GetBagByGift(int id)
        {
            return await _context.Bags.Where(b => b.IdGift == id).ToArrayAsync();
        }

        public async Task RemoveGiftFromAllBags(int giftId)
        {
            var itemsToRemove = _context.Bags.Where(b => b.IdGift == giftId);
            _context.Bags.RemoveRange(itemsToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
