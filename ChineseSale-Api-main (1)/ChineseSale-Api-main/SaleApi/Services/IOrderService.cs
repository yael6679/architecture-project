using static SaleApi.Dto.GiftDto;
using static SaleApi.Dto.OrderDto;

namespace SaleApi.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<GetOrderDto>> GetAllOrders();
        Task<GetOrderDto?> AddOrder(AddOrderDto dto);
        Task<IEnumerable<GetOrderDto>> GetOrdersByGiftId(int giftId);
        Task<IEnumerable<object>> GetUserHistoryAsync(int userId);
        Task<IEnumerable<GetGiftDto>> GetOrdersSortedByPopularity();
        Task<IEnumerable<GetGiftDto>> GetOrdersSortedByPrice();
    }
}
