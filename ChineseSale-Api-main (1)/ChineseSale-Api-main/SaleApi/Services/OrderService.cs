using SaleApi.Dto;
using SaleApi.Models;
using SaleApi.Repositories;
using static SaleApi.Dto.GiftDto;
using static SaleApi.Dto.OrderDto;

namespace SaleApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBagRepository _bagRepository;

        public OrderService(IOrderRepository orderRepository, IBagRepository bagRepository)
        {
            _orderRepository = orderRepository;
            _bagRepository = bagRepository;
        }

        public async Task<IEnumerable<GetOrderDto>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();
            return orders.Select(o => MapToDto(o));
        }

        public async Task<GetOrderDto?> AddOrder(AddOrderDto dto)
        {
            var order = new Order { IdUser = dto.IdUser, IdGift = dto.IdGift, Win = false };
            var created = await _orderRepository.AddOrder(order);
            return created != null ? MapToDto(created) : null;
        }

        public async Task<IEnumerable<GetOrderDto>> GetOrdersByGiftId(int giftId)
        {
            var orders = await _orderRepository.GetOrdersByGiftId(giftId);
            return orders.Select(o => MapToDto(o));
        }

        private GetOrderDto MapToDto(Order o) => new GetOrderDto
        {
            Id = o.Id,
            IdUser = o.IdUser,
            Win = o.Win,
            User = o.User != null ? new UserShortDto { FirstName = o.User.FirstName, LastName = o.User.LastName } : null,
            Gift = o.Gift != null ? new OrderShortDto { Id = o.Gift.Id, Name = o.Gift.Name, Price = (int)o.Gift.Price } : null
        };

        public async Task<IEnumerable<object>> GetUserHistoryAsync(int userId)
        {
            var allOrders = await _orderRepository.GetOrdersByUserId(userId);

            var groupedOrders = allOrders
                .GroupBy(o => o.OrderGroupId)
                .Select(group => new
                {
                    OrderNumber = group.Key,
                    TotalAmount = group.Sum(o => o.Gift.Price),
                    Items = group.Select(o => new
                    {
                        GiftName = o.Gift.Name,
                        Price = o.Gift.Price,
                        Img = o.Gift.Img
                    }).ToList()
                })
                .OrderByDescending(g => g.OrderNumber)
                .ToList();

            return groupedOrders;
        }

        public async Task<IEnumerable<GetGiftDto>> GetOrdersSortedByPopularity()
        {
            var allOrders = await _orderRepository.GetAllOrders();

            var popularGifts = allOrders
                .Where(o => o.Gift != null)
                .GroupBy(o => o.IdGift)
                .OrderByDescending(g => g.Count())
                .Select(g => new GetGiftDto
                {
                    Id = g.First().Gift.Id,
                    Name = g.First().Gift.Name,
                    Description = g.First().Gift.Description,
                    Price = g.First().Gift.Price,
                    Img = g.First().Gift.Img
                })
                .ToList();

            return popularGifts;
        }

        public async Task<IEnumerable<GetGiftDto>> GetOrdersSortedByPrice()
        {
            var allOrders = await _orderRepository.GetAllOrders();

            var sortedGifts = allOrders
                .Where(o => o.Gift != null)
                .Select(o => o.Gift)
                .GroupBy(g => g.Id)
                .Select(group => group.First())
                .OrderByDescending(g => g.Price)
                .Select(g => new GetGiftDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    Img = g.Img
                })
                .ToList();

            return sortedGifts;
        }
    }
}
