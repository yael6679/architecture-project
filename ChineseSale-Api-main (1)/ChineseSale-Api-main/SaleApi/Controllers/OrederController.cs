using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleApi.Dto;
using SaleApi.Models;
using SaleApi.Services;
using static SaleApi.Dto.GiftDto;
using static SaleApi.Dto.OrderDto;

namespace SaleApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService,
        IKafkaProducerService kafkaProducer,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<GetOrderDto>> CreateOrder([FromBody] AddOrderDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var created = await _orderService.AddOrder(dto);
            if (created == null)
            {
                return BadRequest("Failed to create order.");
            }

            var orderEvent = new OrderEventMessage
            {
                EventType = "OrderCreated",
                UserId = dto.IdUser,
                OrderId = created.Id,
                GiftId = dto.IdGift,
                OccurredAt = DateTime.UtcNow
            };

            await _kafkaProducer.PublishOrderEventAsync(orderEvent);

            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for user {UserId}", dto.IdUser);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
    {
        try
        {
            var ord = await _orderService.GetAllOrders();
            return Ok(ord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all orders");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("history/{userId}")]
    public async Task<IActionResult> GetHistory(int userId)
    {
        try
        {
            var history = await _orderService.GetUserHistoryAsync(userId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order history for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("by-gift/{giftId}")]
    public async Task<IActionResult> GetOrdersByGiftId(int giftId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByGiftId(giftId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders for gift {GiftId}", giftId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("sort/popularity")]
    public async Task<ActionResult<IEnumerable<GetGiftDto>>> GetOrdersSortedByPopularity()
    {
        try
        {
            var popularGifts = await _orderService.GetOrdersSortedByPopularity();
            return Ok(popularGifts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sorting orders by popularity");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("sort/price")]
    public async Task<ActionResult<IEnumerable<GetGiftDto>>> GetOrdersSortedByPrice()
    {
        try
        {
            var gifts = await _orderService.GetOrdersSortedByPrice();
            return Ok(gifts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sorting orders by price");
            return StatusCode(500, "Internal server error");
        }
    }
}
