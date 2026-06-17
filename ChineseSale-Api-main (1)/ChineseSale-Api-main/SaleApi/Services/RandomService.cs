using SaleApi.Models;
using SaleApi.Repositories;

namespace SaleApi.Services
{
    public class RandomService : IRandomService
    {
        private readonly IRandomRepository _randomRepo;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IGiftRepository _giftRepository;
        private readonly IBagRepository _bagRepository;

        public RandomService(IRandomRepository randomRepo, IEmailService emailService, IUserRepository userRepository, IGiftRepository giftRepository, IBagRepository bagRepository)
        {
            _randomRepo = randomRepo;
            _emailService = emailService;
            _userRepository = userRepository;
            _giftRepository = giftRepository;
            _bagRepository = bagRepository;
        }

        public async Task<int?> PickWinner(int giftId)
        {
            var ticketIds = await _randomRepo.GetOrdersForGift(giftId);

            if (ticketIds == null || !ticketIds.Any())
            {
                return null;
            }

            Random random = new Random();

            var list = ticketIds.ToList();
            int randomIndex = random.Next(list.Count);
            return list[randomIndex];
        }

        public async Task<Winner?> ExecuteDraw(int giftId)
        {
            if (await _randomRepo.IsGiftRandom(giftId))
            {
                throw new Exception("הגרלה עבור מתנה זו כבר בוצעה בעבר.");
            }

            int? winOrderId = await PickWinner(giftId);
            if (winOrderId == null)
            {
                throw new KeyNotFoundException("לא ניתן לבצע הגרלה: אין אף משתתף שרכש כרטיס למתנה זו.");
            }

            if (winOrderId == null)
            {
                return null;
            }

            var winOrder = await _randomRepo.GetOrderById(winOrderId.Value);

            var winner = new Winner
            {
                IdGift = giftId,
                IdUser = winOrder.IdUser
            };

            await _randomRepo.SaveWinner(winner, winOrder.Id);
            await _bagRepository.RemoveGiftFromAllBags(giftId);

            return winner;
        }

        public async Task<bool> IsGiftDrawnAsync(int giftId)
        {
            return await _randomRepo.IsGiftDrawnAsync(giftId);
        }

        public async Task<IEnumerable<Winner>> GetAllWinnersAsync()
        {
            return await _randomRepo.GetDrawnGiftIdsAsync();
        }
    }
}
