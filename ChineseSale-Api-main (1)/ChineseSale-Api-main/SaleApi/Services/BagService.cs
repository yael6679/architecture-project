using SaleApi.Dto;
using SaleApi.Models;
using SaleApi.Repositories;
using static SaleApi.Dto.BagDto;
using static SaleApi.Dto.GiftDto;

namespace SaleApi.Services
{
    public class BagService : IBagService
    {
        private readonly IBagRepository _bagRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRandomRepository _randomRepository;
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly ILogger<BagService> _logger;

        public BagService(
            IBagRepository bagRepository,
            IOrderRepository orderRepository,
            IRandomRepository randomRepository,
            IKafkaProducerService kafkaProducer,
            ILogger<BagService> logger)
        {
            _bagRepository = bagRepository;
            _orderRepository = orderRepository;
            _randomRepository = randomRepository;
            _kafkaProducer = kafkaProducer;
            _logger = logger;
        }

        public async Task<bool> ProcessCheckout(int userId)
        {
            var itemsInBag = await _bagRepository.GetBagByUser(userId);
            if (itemsInBag == null || !itemsInBag.Any()) return false;

            int newGroupId = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            var orderItems = itemsInBag
                .Where(item => item?.Gift != null)
                .Select(item => new OrderEventItem
                {
                    GiftId = item!.IdGift,
                    GiftName = item.Gift!.Name,
                    Quantity = item.Quantity,
                    Price = item.Gift.Price
                })
                .ToList();

            var totalPrice = orderItems.Sum(i => (decimal)i.Price * i.Quantity);

            foreach (var item in itemsInBag)
            {
                if (item == null) continue;

                for (int i = 0; i < item.Quantity; i++)
                {
                    var newOrder = new Order
                    {
                        IdUser = userId,
                        IdGift = item.IdGift,
                        Win = false,
                        OrderGroupId = newGroupId
                    };
                    await _orderRepository.AddOrder(newOrder);
                }
            }

            await _bagRepository.ClearUserBag(userId);

            await _kafkaProducer.PublishOrderEventAsync(new OrderEventMessage
            {
                EventType = "CheckoutCompleted",
                UserId = userId,
                OrderGroupId = newGroupId,
                Items = orderItems,
                TotalPrice = totalPrice,
                OccurredAt = DateTime.UtcNow
            });

            return true;
        }

        public async Task<IEnumerable<GetBagDto>> GetAllBag()
        {
            var allBags = await _bagRepository.GetAllBag();
            return allBags
                .Where(b => b.Gift != null)
                .Select(b => new GetBagDto
                {
                    Id = b.Id,
                    IdUser = b.IdUser,
                    IdGift = b.IdGift,
                    Gift = new GiftResponseDto
                    {
                        Id = b.Id,
                        Name = b.Gift!.Name,
                        Description = b.Gift.Description,
                        Img = b.Gift.Img,
                        Price = b.Gift.Price,
                        IdDoner = b.Gift.IdDoner,
                    }
                })
                .ToList();
        }

        public async Task<Bag> NewGiftToBag(CreateBagDto bagDto)
        {
            if (await _randomRepository.IsGiftDrawnAsync(bagDto.IdGift))
            {
                throw new Exception("לא ניתן להוסיף לסל: מתנה זו כבר הוגרלה!");
            }

            var bag = new Bag
            {
                IdUser = bagDto.IdUser,
                IdGift = bagDto.IdGift,
                Quantity = bagDto.Quantity > 0 ? bagDto.Quantity : 1
            };

            var result = await _bagRepository.NewGiftToBag(bag);

            if (result == null)
            {
                throw new Exception("המוצר לא נמצא במערכת");
            }

            return result;
        }

        public async Task DeleteBag(int id)
        {
            await _bagRepository.DeleteBag(id);
        }

        public async Task<Bag> GetBagById(int id)
        {
            var b = await _bagRepository.GetBagById(id);
            if (b == null) return null!;
            return b;
        }

        public async Task<IEnumerable<Bag>> GetBagByUser(int id)
        {
            return await _bagRepository.GetBagByUser(id);
        }

        public async Task<IEnumerable<Bag>> GetBagByGift(int id)
        {
            return await _bagRepository.GetBagByGift(id);
        }
    }
}
