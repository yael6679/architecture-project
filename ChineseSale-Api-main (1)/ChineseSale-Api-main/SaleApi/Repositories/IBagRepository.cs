using SaleApi.Models;

namespace SaleApi.Repositories
{
    public interface IBagRepository
    {
        Task<IEnumerable<Bag>> GetAllBag();
        Task<Bag> NewGiftToBag(Bag bag);
        Task DeleteBag(int id);
        Task<Bag?> GetBagById(int id);
        Task<IEnumerable<Bag>> GetBagByUser(int userId);
        Task<IEnumerable<Bag>> GetBagByGift(int giftId);
        Task ClearUserBag(int userId);
        Task RemoveGiftFromAllBags(int giftId);
    }
}
