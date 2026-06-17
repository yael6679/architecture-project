using Microsoft.EntityFrameworkCore;
using SaleApi.Data;
using SaleApi.Models;

namespace SaleApi.Repositories
{
    public class DonerRepository : IDonerRepository
    {
        SaleContextDB _context;

        public DonerRepository(SaleContextDB saleContextDB)
        {
            _context = saleContextDB;
        }

        public async Task<IEnumerable<Doner>> GetAllDoner()
        {
            return await _context.Doners.ToListAsync();
        }

        public async Task<Doner> NewDoner(Doner doner)
        {
            _context.Doners.Add(doner);
            await _context.SaveChangesAsync();
            return doner;
        }

        public async Task DeleteDoner(int id)
        {
            var doner = await _context.Doners.FindAsync(id);
            if (doner != null)
            {
                _context.Doners.Remove(doner);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Doner> UpdateDoner(Doner doner)
        {
            _context.Doners.Update(doner);
            await _context.SaveChangesAsync();
            return doner;
        }

        public async Task<Doner?> GetDonerById(int id)
        {
            return await _context.Doners.FindAsync(id);
        }

        public async Task AddGiftToDoner(int donerId, Gift gift)
        {
            var doner = await _context.Doners.Include(d => d.Gifts).FirstOrDefaultAsync(d => d.Id == donerId);
            if (doner != null)
            {
                doner.Gifts.Add(gift);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Doner>> GetAllDonerWithGift()
        {
            return await _context.Doners.Include(d => d.Gifts).ToListAsync();
        }

        public async Task<Doner?> GetDonerByIdWithGift(int id)
        {
            return await _context.Doners.Include(d => d.Gifts)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Doner?>> GetDonerByName(string name)
        {
            var term = name.Trim();
            return await _context.Doners
                .Include(d => d.Gifts)
                .Where(d =>
                    d.FirstName.Contains(term) ||
                    d.LastName.Contains(term) ||
                    (d.FirstName + " " + d.LastName).Contains(term) ||
                    (d.LastName + " " + d.FirstName).Contains(term))
                .ToListAsync();
        }

        public async Task<IEnumerable<Doner?>> GetDonerByMail(string email)
        {
            var term = email.Trim();
            return await _context.Doners
                .Include(d => d.Gifts)
                .Where(d => d.Email.Contains(term))
                .ToListAsync();
        }

        public async Task<IEnumerable<Doner?>> GetDonerByGift(string giftName)
        {
            var term = giftName.Trim();
            return await _context.Doners
                .Include(g => g.Gifts)
                .Where(g => g.Gifts.Any(gift => gift.Name.Contains(term)))
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Doners.AnyAsync(u => u.Email == email);
        }
    }
}
